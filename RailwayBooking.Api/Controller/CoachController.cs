using MediatR;
using Microsoft.AspNetCore.Mvc;
using RailwayBooking.Application.Features.Railway.Coaches;

namespace RailwayBooking.Api.Controller
{
    [ApiController]
    [Route("api/coaches")]
    public class CoachController : BaseController
    {
        public CoachController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] long? trainId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var response = await Send(new GetCoachesQuery { TrainId = trainId, Page = page, PageSize = pageSize });
            return response;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var response = await Send(new GetCoachByIdQuery { Id = id });
            return response;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCoachCommand command)
        {
            var response = await Send(command);
            return response;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateCoachCommand command)
        {
            command.Id = id;
            var response = await Send(command);
            return response;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await Send(new DeleteCoachCommand { Id = id });
            return response;
        }
    }
}
