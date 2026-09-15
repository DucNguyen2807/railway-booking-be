using System.Globalization;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RailwayBooking.Domain.Enums.Status;
using RailwayBooking.Domain.Interfaces;
using RailwayBooking.Infrastructure.Entities.Booking;
using RailwayBooking.Infrastructure.Entities.Inventory;
using RailwayBooking.Infrastructure.Entities.Payments;

namespace RailwayBooking.Application.Services.Impl
{
    public class PaymentGatewayService : IPaymentGatewayService
    {
        private const string PayOsProvider = "payos";
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public PaymentGatewayService(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<PaymentStartResult> CreatePaymentAsync(long orderId, Guid userId, string provider, string clientIp, CancellationToken cancellationToken)
        {
            var normalizedProvider = NormalizeProvider(provider);
            if (normalizedProvider == null)
            {
                return new PaymentStartResult
                {
                    Message = "Unsupported payment provider"
                };
            }

            var order = await _unitOfWork.BookingOrderRepository
                .FirstOrDefaultAsync(x => x.Id == orderId && x.UserId == userId);

            if (order == null)
            {
                return new PaymentStartResult
                {
                    Message = "Order not found"
                };
            }

            if (order.Status == BookingOrderStatus.Confirmed)
            {
                return new PaymentStartResult
                {
                    Message = "Order already confirmed"
                };
            }

            if (order.Status == BookingOrderStatus.Cancelled)
            {
                return new PaymentStartResult
                {
                    Message = "Order already cancelled"
                };
            }

            if (order.Status != BookingOrderStatus.Pending && order.Status != BookingOrderStatus.PaymentProcessing)
            {
                return new PaymentStartResult
                {
                    Message = "Order is not payable"
                };
            }

            if (!order.HoldToken.HasValue)
            {
                return new PaymentStartResult
                {
                    Message = "Order has no hold token"
                };
            }

            var hold = await _unitOfWork.SeatHoldRepository
                .FirstOrDefaultAsync(x => x.HoldToken == order.HoldToken.Value && x.UserId == userId);

            if (hold == null)
            {
                return new PaymentStartResult
                {
                    Message = "Hold not found"
                };
            }

            if (hold.Status != HoldStatus.Active || hold.ExpiredAt <= DateTime.UtcNow)
            {
                return new PaymentStartResult
                {
                    Message = "Hold is no longer active"
                };
            }

            var providerBuild = await BuildPayOsPaymentLinkAsync(order, cancellationToken);
            if (!providerBuild.IsSuccess)
            {
                return new PaymentStartResult
                {
                    Message = providerBuild.Message,
                    OrderId = order.Id,
                    OrderCode = order.OrderCode,
                    Provider = PayOsProvider,
                    OrderStatus = order.Status.ToString()
                };
            }

            var existingPendingTransactions = await _unitOfWork.PaymentTransactionRepository
                .Get(x => x.OrderId == order.Id && x.PaymentProvider == PayOsProvider && x.Status == PaymentStatus.Pending)
                .ToListAsync(cancellationToken);

            foreach (var existingTransaction in existingPendingTransactions)
            {
                existingTransaction.Status = PaymentStatus.Failed;
                existingTransaction.RawResponse = JsonSerializer.Serialize(new
                {
                    note = "Replaced by a new payOS payment initialization request"
                }, JsonOptions);
                _unitOfWork.PaymentTransactionRepository.Update(existingTransaction);
            }

            var paymentTransaction = new PaymentTransaction
            {
                OrderId = order.Id,
                PaymentProvider = PayOsProvider,
                ProviderTransactionId = providerBuild.ProviderTransactionId,
                IdempotencyKey = $"{PayOsProvider}-{order.Id}-{Guid.NewGuid():N}",
                Amount = order.TotalAmount,
                Currency = string.IsNullOrWhiteSpace(order.Currency) ? "VND" : order.Currency,
                Status = PaymentStatus.Pending,
                RawResponse = providerBuild.RawPayload
            };

            order.Status = BookingOrderStatus.PaymentProcessing;

            await _unitOfWork.PaymentTransactionRepository.InsertAsync(paymentTransaction);
            _unitOfWork.BookingOrderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new PaymentStartResult
            {
                IsSuccess = true,
                Message = "payOS payment link created",
                OrderId = order.Id,
                OrderCode = order.OrderCode,
                Provider = PayOsProvider,
                PaymentUrl = providerBuild.PaymentUrl,
                ProviderTransactionId = providerBuild.ProviderTransactionId,
                OrderStatus = order.Status.ToString()
            };
        }

        public async Task<PaymentCallbackResult> HandlePayOsReturnAsync(IDictionary<string, string> callbackParams, CancellationToken cancellationToken)
        {
            var orderId = GetPayOsOrderCode(callbackParams);
            if (!orderId.HasValue)
            {
                return new PaymentCallbackResult
                {
                    Message = "Invalid payOS order code",
                    Provider = PayOsProvider
                };
            }

            var status = callbackParams.TryGetValue("status", out var value) ? value : string.Empty;
            if (string.Equals(status, "PAID", StringComparison.OrdinalIgnoreCase))
            {
                return await MarkPayOsPaymentSucceededAsync(
                    orderId.Value,
                    callbackParams.TryGetValue("id", out var linkId) ? linkId : null,
                    callbackParams,
                    cancellationToken);
            }

            await MarkPayOsPaymentFailedAsync(orderId.Value, callbackParams, cancellationToken);
            await MoveOrderBackToPendingAsync(orderId.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new PaymentCallbackResult
            {
                Message = $"payOS payment has not been completed. status={status}",
                OrderId = orderId.Value,
                Provider = PayOsProvider,
                PaymentStatus = PaymentStatus.Failed.ToString(),
                OrderStatus = BookingOrderStatus.Pending.ToString()
            };
        }

        public async Task<PaymentCallbackResult> HandlePayOsWebhookAsync(PayOsWebhookPayload payload, CancellationToken cancellationToken)
        {
            var checksumKey = _configuration["PaymentProviders:PayOS:ChecksumKey"];
            if (string.IsNullOrWhiteSpace(checksumKey))
            {
                return new PaymentCallbackResult
                {
                    Message = "payOS configuration is missing",
                    Provider = PayOsProvider
                };
            }

            if (payload.Data == null)
            {
                return new PaymentCallbackResult
                {
                    Message = "Missing payOS webhook data",
                    Provider = PayOsProvider
                };
            }

            var signedData = BuildPayOsSignatureData(payload.Data);
            var computedSignature = ComputeHmacSha256(checksumKey, signedData);
            if (!string.Equals(computedSignature, payload.Signature, StringComparison.OrdinalIgnoreCase))
            {
                return new PaymentCallbackResult
                {
                    Message = "Invalid payOS webhook signature",
                    OrderId = payload.Data.OrderCode,
                    Provider = PayOsProvider
                };
            }

            var isSuccess = payload.Success
                && string.Equals(payload.Code, "00", StringComparison.OrdinalIgnoreCase)
                && string.Equals(payload.Data.Code, "00", StringComparison.OrdinalIgnoreCase);

            if (isSuccess)
            {
                return await MarkPayOsPaymentSucceededAsync(
                    payload.Data.OrderCode,
                    payload.Data.PaymentLinkId,
                    payload,
                    cancellationToken);
            }

            await MarkPayOsPaymentFailedAsync(payload.Data.OrderCode, payload, cancellationToken);
            await MoveOrderBackToPendingAsync(payload.Data.OrderCode, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new PaymentCallbackResult
            {
                Message = $"payOS payment failed with code={payload.Code}, dataCode={payload.Data.Code}",
                OrderId = payload.Data.OrderCode,
                Provider = PayOsProvider,
                PaymentStatus = PaymentStatus.Failed.ToString(),
                OrderStatus = BookingOrderStatus.Pending.ToString()
            };
        }

        private async Task<ProviderBuildResult> BuildPayOsPaymentLinkAsync(BookingOrder order, CancellationToken cancellationToken)
        {
            var baseUrl = _configuration["PaymentProviders:PayOS:BaseUrl"] ?? "https://api-merchant.payos.vn";
            var clientId = _configuration["PaymentProviders:PayOS:ClientId"];
            var apiKey = _configuration["PaymentProviders:PayOS:ApiKey"];
            var checksumKey = _configuration["PaymentProviders:PayOS:ChecksumKey"];
            var returnUrl = _configuration["PaymentProviders:PayOS:ReturnUrl"];
            var cancelUrl = _configuration["PaymentProviders:PayOS:CancelUrl"];

            if (string.IsNullOrWhiteSpace(clientId)
                || string.IsNullOrWhiteSpace(apiKey)
                || string.IsNullOrWhiteSpace(checksumKey)
                || string.IsNullOrWhiteSpace(returnUrl)
                || string.IsNullOrWhiteSpace(cancelUrl))
            {
                return new ProviderBuildResult
                {
                    Message = "payOS configuration is missing"
                };
            }

            var amount = Convert.ToInt64(Math.Round(order.TotalAmount, 0, MidpointRounding.AwayFromZero));
            var description = BuildPayOsDescription(order);
            var signatureData = $"amount={amount}&cancelUrl={cancelUrl}&description={description}&orderCode={order.Id}&returnUrl={returnUrl}";
            var signature = ComputeHmacSha256(checksumKey, signatureData);
            var expiredAt = ResolvePayOsExpiredAt();

            var request = new PayOsCreatePaymentRequest
            {
                OrderCode = order.Id,
                Amount = amount,
                Description = description,
                CancelUrl = cancelUrl,
                ReturnUrl = returnUrl,
                ExpiredAt = expiredAt,
                Signature = signature
            };

            var httpClient = _httpClientFactory.CreateClient();
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, CombineUrl(baseUrl, "/v2/payment-requests"))
            {
                Content = JsonContent.Create(request, options: JsonOptions)
            };
            httpRequest.Headers.Add("x-client-id", clientId);
            httpRequest.Headers.Add("x-api-key", apiKey);

            using var response = await httpClient.SendAsync(httpRequest, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new ProviderBuildResult
                {
                    Message = $"payOS create payment link failed with HTTP {(int)response.StatusCode}",
                    RawPayload = responseContent
                };
            }

            var payOsResponse = JsonSerializer.Deserialize<PayOsCreatePaymentResponse>(responseContent, JsonOptions);
            if (payOsResponse == null || !string.Equals(payOsResponse.Code, "00", StringComparison.OrdinalIgnoreCase) || payOsResponse.Data == null)
            {
                return new ProviderBuildResult
                {
                    Message = payOsResponse?.Desc ?? "payOS create payment link failed",
                    RawPayload = responseContent
                };
            }

            return new ProviderBuildResult
            {
                IsSuccess = true,
                Message = "payOS URL created",
                PaymentUrl = payOsResponse.Data.CheckoutUrl,
                ProviderTransactionId = payOsResponse.Data.PaymentLinkId,
                RawPayload = responseContent
            };
        }

        private async Task MarkPayOsPaymentFailedAsync(long orderId, object rawResponse, CancellationToken cancellationToken)
        {
            var paymentTransaction = await _unitOfWork.PaymentTransactionRepository
                .Get(x => x.OrderId == orderId && x.PaymentProvider == PayOsProvider)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (paymentTransaction == null || paymentTransaction.Status == PaymentStatus.Success)
            {
                return;
            }

            paymentTransaction.RawResponse = JsonSerializer.Serialize(rawResponse, JsonOptions);
            paymentTransaction.Status = PaymentStatus.Failed;
            paymentTransaction.PaidAt = null;
            _unitOfWork.PaymentTransactionRepository.Update(paymentTransaction);
        }

        private async Task<PaymentCallbackResult> MarkPayOsPaymentSucceededAsync(long orderId, string? providerTransactionId, object rawResponse, CancellationToken cancellationToken)
        {
            var paymentTransaction = await _unitOfWork.PaymentTransactionRepository
                .Get(x => x.OrderId == orderId && x.PaymentProvider == PayOsProvider)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (paymentTransaction == null)
            {
                return new PaymentCallbackResult
                {
                    Message = "Payment transaction not found",
                    OrderId = orderId,
                    Provider = PayOsProvider
                };
            }

            if (!string.IsNullOrWhiteSpace(providerTransactionId))
            {
                paymentTransaction.ProviderTransactionId = providerTransactionId;
            }

            paymentTransaction.RawResponse = JsonSerializer.Serialize(rawResponse, JsonOptions);
            paymentTransaction.Status = PaymentStatus.Success;
            paymentTransaction.PaidAt = DateTime.UtcNow;
            _unitOfWork.PaymentTransactionRepository.Update(paymentTransaction);

            var confirmResult = await ConfirmOrderAfterPaymentAsync(orderId, cancellationToken);

            return new PaymentCallbackResult
            {
                IsSuccess = confirmResult.IsSuccess,
                Message = confirmResult.Message,
                OrderId = confirmResult.OrderId,
                Provider = PayOsProvider,
                PaymentStatus = paymentTransaction.Status.ToString(),
                OrderStatus = confirmResult.OrderStatus
            };
        }

        private async Task<PaymentCallbackResult> ConfirmOrderAfterPaymentAsync(long orderId, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var order = await _unitOfWork.BookingOrderRepository
                    .GetIncludeMultiLayer(
                        x => x.Id == orderId,
                        include: query => query.Include(x => x.Items))
                    .FirstOrDefaultAsync(cancellationToken);

                if (order == null)
                {
                    return new PaymentCallbackResult
                    {
                        Message = "Order not found"
                    };
                }

                if (order.Status == BookingOrderStatus.Confirmed)
                {
                    return new PaymentCallbackResult
                    {
                        IsSuccess = true,
                        Message = "Order already confirmed",
                        OrderId = order.Id,
                        PaymentStatus = PaymentStatus.Success.ToString(),
                        OrderStatus = order.Status.ToString()
                    };
                }

                if (order.Status != BookingOrderStatus.Pending && order.Status != BookingOrderStatus.PaymentProcessing)
                {
                    return new PaymentCallbackResult
                    {
                        Message = "Order is not payable",
                        OrderId = order.Id,
                        PaymentStatus = PaymentStatus.Failed.ToString(),
                        OrderStatus = order.Status.ToString()
                    };
                }

                if (!order.HoldToken.HasValue)
                {
                    return new PaymentCallbackResult
                    {
                        Message = "Order has no hold token",
                        OrderId = order.Id,
                        PaymentStatus = PaymentStatus.Failed.ToString(),
                        OrderStatus = order.Status.ToString()
                    };
                }

                var hold = await _unitOfWork.SeatHoldRepository
                    .FirstOrDefaultAsync(x => x.HoldToken == order.HoldToken.Value && x.UserId == order.UserId);

                if (hold == null)
                {
                    return new PaymentCallbackResult
                    {
                        Message = "Hold not found",
                        OrderId = order.Id,
                        PaymentStatus = PaymentStatus.Failed.ToString(),
                        OrderStatus = order.Status.ToString()
                    };
                }

                if (hold.Status != HoldStatus.Active || hold.ExpiredAt <= DateTime.UtcNow)
                {
                    var cancellationResult = await CancelExpiredOrderAsync(order, hold, cancellationToken);

                    return new PaymentCallbackResult
                    {
                        Message = cancellationResult.Message,
                        OrderId = order.Id,
                        PaymentStatus = PaymentStatus.Failed.ToString(),
                        OrderStatus = cancellationResult.Status ?? BookingOrderStatus.Cancelled.ToString()
                    };
                }

                if (order.Items.Count == 0)
                {
                    return new PaymentCallbackResult
                    {
                        Message = "Order items not found",
                        OrderId = order.Id,
                        PaymentStatus = PaymentStatus.Failed.ToString(),
                        OrderStatus = order.Status.ToString()
                    };
                }

                var now = DateTime.UtcNow;
                foreach (var item in order.Items)
                {
                    var inventory = await _unitOfWork.TripSeatInventoryRepository
                        .FirstOrDefaultAsync(x => x.Id == item.InventoryId
                            && x.Status == InventoryStatus.Hold
                            && x.HoldToken == order.HoldToken.Value);

                    if (inventory == null)
                    {
                        return new PaymentCallbackResult
                        {
                            Message = $"Seat inventory {item.InventoryId} is no longer available",
                            OrderId = order.Id,
                            PaymentStatus = PaymentStatus.Failed.ToString(),
                            OrderStatus = order.Status.ToString()
                        };
                    }

                    inventory.Status = InventoryStatus.Booked;
                    inventory.BookedOrderId = order.Id;
                    inventory.HoldToken = null;
                    inventory.HoldExpiredAt = null;
                    inventory.Version += 1;
                    inventory.LastReservedAt = now;
                    _unitOfWork.TripSeatInventoryRepository.Update(inventory);
                }

                order.Status = BookingOrderStatus.Confirmed;
                order.ConfirmedAt = now;
                _unitOfWork.BookingOrderRepository.Update(order);

                hold.Status = HoldStatus.Completed;
                _unitOfWork.SeatHoldRepository.Update(hold);

                return new PaymentCallbackResult
                {
                    IsSuccess = true,
                    Message = "Payment success. Order confirmed",
                    OrderId = order.Id,
                    PaymentStatus = PaymentStatus.Success.ToString(),
                    OrderStatus = order.Status.ToString()
                };
            });
        }

        private async Task<BookingOrderCancellationResult> CancelExpiredOrderAsync(BookingOrder order, SeatHold hold, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var releasedCount = 0;

            foreach (var item in order.Items)
            {
                var inventory = await _unitOfWork.TripSeatInventoryRepository
                    .FirstOrDefaultAsync(x => x.Id == item.InventoryId
                        && x.Status == InventoryStatus.Hold
                        && x.HoldToken == order.HoldToken);

                if (inventory == null)
                {
                    return new BookingOrderCancellationResult
                    {
                        Message = $"Seat inventory {item.InventoryId} is no longer on hold"
                    };
                }

                inventory.Status = InventoryStatus.Available;
                inventory.BookedOrderId = null;
                inventory.HoldToken = null;
                inventory.HoldExpiredAt = null;
                inventory.Version += 1;
                inventory.LastReservedAt = now;
                _unitOfWork.TripSeatInventoryRepository.Update(inventory);
                releasedCount++;
            }

            hold.Status = HoldStatus.Cancelled;
            _unitOfWork.SeatHoldRepository.Update(hold);

            order.Status = BookingOrderStatus.Cancelled;
            order.CancelledAt = now;
            _unitOfWork.BookingOrderRepository.Update(order);

            return new BookingOrderCancellationResult
            {
                IsSuccess = true,
                Message = "Hold expired. Order cancelled and seats released",
                OrderId = order.Id,
                OrderCode = order.OrderCode,
                CancelledAt = order.CancelledAt,
                ReleasedSeatCount = releasedCount,
                Status = order.Status.ToString()
            };
        }

        private async Task MoveOrderBackToPendingAsync(long orderId, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.BookingOrderRepository
                .FirstOrDefaultAsync(x => x.Id == orderId);

            if (order == null)
            {
                return;
            }

            if (order.Status == BookingOrderStatus.Pending || order.Status == BookingOrderStatus.PaymentProcessing)
            {
                order.Status = BookingOrderStatus.Pending;
                _unitOfWork.BookingOrderRepository.Update(order);
            }
        }

        private static string? NormalizeProvider(string provider)
        {
            if (string.IsNullOrWhiteSpace(provider))
            {
                return null;
            }

            return provider.Trim().Equals(PayOsProvider, StringComparison.OrdinalIgnoreCase)
                ? PayOsProvider
                : null;
        }

        private int? ResolvePayOsExpiredAt()
        {
            var expireMinutesValue = _configuration["PaymentProviders:PayOS:ExpireMinutes"];
            if (!int.TryParse(expireMinutesValue, out var expireMinutes) || expireMinutes <= 0)
            {
                return null;
            }

            return Convert.ToInt32(DateTimeOffset.UtcNow.AddMinutes(expireMinutes).ToUnixTimeSeconds());
        }

        private static string BuildPayOsDescription(BookingOrder order)
        {
            var suffix = order.Id.ToString(CultureInfo.InvariantCulture);
            var description = $"Railway {suffix}";

            return description.Length <= 25
                ? description
                : description[..25];
        }

        private static long? GetPayOsOrderCode(IDictionary<string, string> callbackParams)
        {
            if (callbackParams.TryGetValue("orderCode", out var orderCode) && long.TryParse(orderCode, out var parsedOrderCode))
            {
                return parsedOrderCode;
            }

            if (callbackParams.TryGetValue("order_id", out var orderId) && long.TryParse(orderId, out var parsedOrderId))
            {
                return parsedOrderId;
            }

            return null;
        }

        private static string BuildPayOsSignatureData(PayOsWebhookData data)
        {
            var values = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                ["accountNumber"] = data.AccountNumber ?? string.Empty,
                ["amount"] = data.Amount.ToString(CultureInfo.InvariantCulture),
                ["code"] = data.Code ?? string.Empty,
                ["counterAccountBankId"] = data.CounterAccountBankId ?? string.Empty,
                ["counterAccountBankName"] = data.CounterAccountBankName ?? string.Empty,
                ["counterAccountName"] = data.CounterAccountName ?? string.Empty,
                ["counterAccountNumber"] = data.CounterAccountNumber ?? string.Empty,
                ["currency"] = data.Currency ?? string.Empty,
                ["desc"] = data.Desc ?? string.Empty,
                ["description"] = data.Description ?? string.Empty,
                ["orderCode"] = data.OrderCode.ToString(CultureInfo.InvariantCulture),
                ["paymentLinkId"] = data.PaymentLinkId ?? string.Empty,
                ["reference"] = data.Reference ?? string.Empty,
                ["transactionDateTime"] = data.TransactionDateTime ?? string.Empty,
                ["virtualAccountName"] = data.VirtualAccountName ?? string.Empty,
                ["virtualAccountNumber"] = data.VirtualAccountNumber ?? string.Empty
            };

            return string.Join("&", values.Select(x => $"{x.Key}={x.Value}"));
        }

        private static string CombineUrl(string baseUrl, string path)
        {
            return $"{baseUrl.TrimEnd('/')}/{path.TrimStart('/')}";
        }

        private static string ComputeHmacSha256(string secret, string rawData)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var dataBytes = Encoding.UTF8.GetBytes(rawData);
            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(dataBytes);
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }

        private class ProviderBuildResult
        {
            public bool IsSuccess { get; init; }
            public string Message { get; init; } = string.Empty;
            public string PaymentUrl { get; init; } = string.Empty;
            public string ProviderTransactionId { get; init; } = string.Empty;
            public string RawPayload { get; init; } = "{}";
        }

        private class PayOsCreatePaymentRequest
        {
            public long OrderCode { get; init; }
            public long Amount { get; init; }
            public string Description { get; init; } = string.Empty;
            public string CancelUrl { get; init; } = string.Empty;
            public string ReturnUrl { get; init; } = string.Empty;
            public int? ExpiredAt { get; init; }
            public string Signature { get; init; } = string.Empty;
        }

        private class PayOsCreatePaymentResponse
        {
            public string Code { get; init; } = string.Empty;
            public string Desc { get; init; } = string.Empty;
            public PayOsCreatePaymentData? Data { get; init; }
        }

        private class PayOsCreatePaymentData
        {
            public string PaymentLinkId { get; init; } = string.Empty;
            public string CheckoutUrl { get; init; } = string.Empty;
        }
    }
}
