using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RailwayBooking.Application.Features.BookingFeat.CancelBookingOrder;
using RailwayBooking.Application.Features.BookingFeat.CreateBookingOrder;
using RailwayBooking.Application.Features.BookingFeat.ConfirmBookingOrder;
using RailwayBooking.Application.Features.BookingFeat.HoldSeat;

namespace RailwayBooking.Api.Controller
{
    [ApiController]
    [Route("api/booking")]
    [Authorize]
    public class BookingController : BaseController
    {
        public BookingController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost("hold-seat")]
        public async Task<IActionResult> HoldSeat([FromBody] HoldSeatCommand command)
        {
            var response = await Send(command);
            return response;
        }

        [HttpPost("orders")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateBookingOrderCommand command)
        {
            var response = await Send(command);
            return response;
        }

        [HttpPost("orders/{orderId:long}/confirm")]
        public async Task<IActionResult> ConfirmOrder(long orderId)
        {
            var response = await Send(new ConfirmBookingOrderCommand { OrderId = orderId });
            return response;
        }

        [HttpPost("orders/{orderId:long}/cancel")]
        public async Task<IActionResult> CancelOrder(long orderId)
        {
            var response = await Send(new CancelBookingOrderCommand { OrderId = orderId });
            return response;
        }
    }
}
