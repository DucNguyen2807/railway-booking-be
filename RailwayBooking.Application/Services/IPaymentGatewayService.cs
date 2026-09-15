namespace RailwayBooking.Application.Services
{
    public interface IPaymentGatewayService
    {
        Task<PaymentStartResult> CreatePaymentAsync(long orderId, Guid userId, string provider, string clientIp, CancellationToken cancellationToken);

        Task<PaymentCallbackResult> HandlePayOsReturnAsync(IDictionary<string, string> callbackParams, CancellationToken cancellationToken);

        Task<PaymentCallbackResult> HandlePayOsWebhookAsync(PayOsWebhookPayload payload, CancellationToken cancellationToken);
    }

    public class PaymentStartResult
    {
        public bool IsSuccess { get; init; }
        public string Message { get; init; } = string.Empty;
        public long OrderId { get; init; }
        public string? OrderCode { get; init; }
        public string? Provider { get; init; }
        public string? PaymentUrl { get; init; }
        public string? ProviderTransactionId { get; init; }
        public string? OrderStatus { get; init; }
    }

    public class PaymentCallbackResult
    {
        public bool IsSuccess { get; init; }
        public string Message { get; init; } = string.Empty;
        public long OrderId { get; init; }
        public string? Provider { get; init; }
        public string? PaymentStatus { get; init; }
        public string? OrderStatus { get; init; }
    }

    public class PayOsWebhookPayload
    {
        public string Code { get; init; } = string.Empty;
        public string Desc { get; init; } = string.Empty;
        public bool Success { get; init; }
        public PayOsWebhookData? Data { get; init; }
        public string Signature { get; init; } = string.Empty;
    }

    public class PayOsWebhookData
    {
        public long OrderCode { get; init; }
        public long Amount { get; init; }
        public string? Description { get; init; }
        public string? AccountNumber { get; init; }
        public string? Reference { get; init; }
        public string? TransactionDateTime { get; init; }
        public string? Currency { get; init; }
        public string? PaymentLinkId { get; init; }
        public string? Code { get; init; }
        public string? Desc { get; init; }
        public string? CounterAccountBankId { get; init; }
        public string? CounterAccountBankName { get; init; }
        public string? CounterAccountName { get; init; }
        public string? CounterAccountNumber { get; init; }
        public string? VirtualAccountName { get; init; }
        public string? VirtualAccountNumber { get; init; }
    }
}
