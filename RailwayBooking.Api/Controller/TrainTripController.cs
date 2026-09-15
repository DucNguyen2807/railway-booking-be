using MediatR;
using Microsoft.AspNetCore.Mvc;
using RailwayBooking.Application.Features.Trips.TrainTrips;

namespace RailwayBooking.Api.Controller
{
    [ApiController]
    [Route("api/train-trips")]
    public class TrainTripController : BaseController
    {
        public TrainTripController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] long? trainId, [FromQuery] string? fromStation = null, [FromQuery] string? toStation = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var response = await Send(new GetTrainTripsQuery { TrainId = trainId, FromStation = fromStation, ToStation = toStation, Page = page, PageSize = pageSize });
            return response;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var response = await Send(new GetTrainTripByIdQuery { Id = id });
            return response;
        }

        [HttpGet("{tripId}/seats/availability")]
        public async Task<IActionResult> GetSeatAvailability(long tripId, [FromQuery] string? fromStation = null, [FromQuery] string? toStation = null)
        {
            var response = await Send(new GetSeatAvailabilityQuery { TripId = tripId, FromStation = fromStation, ToStation = toStation });
            return response;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTrainTripCommand command)
        {
            var response = await Send(command);
            return response;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateTrainTripCommand command)
        {
            command.Id = id;
            var response = await Send(command);
            return response;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await Send(new DeleteTrainTripCommand { Id = id });
            return response;
        }
    }
}
