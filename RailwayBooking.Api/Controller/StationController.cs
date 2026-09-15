using MediatR;
using Microsoft.AspNetCore.Mvc;
using RailwayBooking.Application.Features.Stations;

namespace RailwayBooking.Api.Controller
{
    [ApiController]
    [Route("api/stations")]
    public class StationController : BaseController
    {
        public StationController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var response = await Send(new GetStationsQuery { Page = page, PageSize = pageSize });
            return response;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var response = await Send(new GetStationByIdQuery { Id = id });
            return response;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStationCommand command)
        {
            var response = await Send(command);
            return response;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateStationCommand command)
        {
            command.Id = id;
            var response = await Send(command);
            return response;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await Send(new DeleteStationCommand { Id = id });
            return response;
        }
    }
}
