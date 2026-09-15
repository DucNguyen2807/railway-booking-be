using MediatR;
using Microsoft.AspNetCore.Mvc;
using RailwayBooking.Application.Common;

namespace RailwayBooking.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected readonly IMediator _mediator;

        protected BaseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        protected async Task<IActionResult> Send<TResponse>(IRequest<BaseResponse<TResponse>> request)
        {
            var response = await _mediator.Send(request);

            if (response == null)
            {
                return BadRequest(BaseResponse<TResponse>.FailureResponse("Executed failed"));
            }

            if (IsSuccessful(response))
            {
                return Ok(response);
            }

            return BadRequest(response);
        }


        public bool IsSuccessful<T>(BaseResponse<T> response)
        {
            return response != null && response.Success;
        }
    }
}
