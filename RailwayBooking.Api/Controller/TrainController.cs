using MediatR;
using Microsoft.AspNetCore.Mvc;
using RailwayBooking.Application.Features.Railway.Trains;

namespace RailwayBooking.Api.Controller
{
    [ApiController]
    [Route("api/trains")]
    public class TrainController : BaseController
    {
        public TrainController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var response = await Send(new GetTrainsQuery { Page = page, PageSize = pageSize });
            return response;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var response = await Send(new GetTrainByIdQuery { Id = id });
            return response;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTrainCommand command)
        {
            var response = await Send(command);
            return response;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateTrainCommand command)
        {
            command.Id = id;
            var response = await Send(command);
            return response;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await Send(new DeleteTrainCommand { Id = id });
            return response;
        }
    }
}
