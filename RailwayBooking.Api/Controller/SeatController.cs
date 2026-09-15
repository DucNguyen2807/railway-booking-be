using MediatR;
using Microsoft.AspNetCore.Mvc;
using RailwayBooking.Application.Features.Railway.Seats;

namespace RailwayBooking.Api.Controller
{
    [ApiController]
    [Route("api/seats")]
    public class SeatController : BaseController
    {
        public SeatController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] long? coachId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var response = await Send(new GetSeatsQuery { CoachId = coachId, Page = page, PageSize = pageSize });
            return response;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var response = await Send(new GetSeatByIdQuery { Id = id });
            return response;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSeatCommand command)
        {
            var response = await Send(command);
            return response;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateSeatCommand command)
        {
            command.Id = id;
            var response = await Send(command);
            return response;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await Send(new DeleteSeatCommand { Id = id });
            return response;
        }
    }
}
