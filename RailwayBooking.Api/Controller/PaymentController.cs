using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Booking;
using RailwayBooking.Application.Services;

namespace RailwayBooking.Api.Controller
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentGatewayService _paymentGatewayService;

        public PaymentController(IPaymentGatewayService paymentGatewayService)
        {
            _paymentGatewayService = paymentGatewayService;
        }

        [Authorize]
        [HttpPost("payos/orders/{orderId:long}")]
        public async Task<IActionResult> CreatePayOsPayment(long orderId, CancellationToken cancellationToken)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized(BaseResponse<InitiatePaymentResponseDto>.FailureResponse("Unauthorized user"));
            }

            var result = await _paymentGatewayService.CreatePaymentAsync(orderId, userId, "payos", GetClientIp(), cancellationToken);
            var response = new InitiatePaymentResponseDto
            {
                OrderId = result.OrderId,
                OrderCode = result.OrderCode,
                Provider = result.Provider,
                PaymentUrl = result.PaymentUrl,
                ProviderTransactionId = result.ProviderTransactionId,
                OrderStatus = result.OrderStatus
            };

            if (!result.IsSuccess)
            {
                return BadRequest(BaseResponse<InitiatePaymentResponseDto>.FailureResponse(response, result.Message));
            }

            return Ok(BaseResponse<InitiatePaymentResponseDto>.SuccessResponse(response, result.Message));
        }

        [AllowAnonymous]
        [HttpGet("payos/return")]
        public async Task<IActionResult> HandlePayOsReturn(CancellationToken cancellationToken)
        {
            var callbackParams = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
            var result = await _paymentGatewayService.HandlePayOsReturnAsync(callbackParams, cancellationToken);
            return BuildPaymentCallbackResponse(result);
        }

        [AllowAnonymous]
        [HttpGet("payos/cancel")]
        public async Task<IActionResult> HandlePayOsCancel(CancellationToken cancellationToken)
        {
            var callbackParams = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
            var result = await _paymentGatewayService.HandlePayOsReturnAsync(callbackParams, cancellationToken);

            return BuildPaymentCallbackResponse(result);
        }

        [AllowAnonymous]
        [HttpPost("payos/webhook")]
        public async Task<IActionResult> HandlePayOsWebhook([FromBody] PayOsWebhookPayload payload, CancellationToken cancellationToken)
        {
            var result = await _paymentGatewayService.HandlePayOsWebhookAsync(payload, cancellationToken);

            return BuildPaymentCallbackResponse(result);
        }

        private static IActionResult BuildPaymentCallbackResponse(PaymentCallbackResult result)
        {
            var response = new PaymentCallbackResponseDto
            {
                OrderId = result.OrderId,
                Provider = result.Provider,
                PaymentStatus = result.PaymentStatus,
                OrderStatus = result.OrderStatus,
                Message = result.Message
            };

            if (!result.IsSuccess)
            {
                return new BadRequestObjectResult(BaseResponse<PaymentCallbackResponseDto>.FailureResponse(response, result.Message));
            }

            return new OkObjectResult(BaseResponse<PaymentCallbackResponseDto>.SuccessResponse(response, result.Message));
        }

        private string GetClientIp()
        {
            var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        }
    }
}
