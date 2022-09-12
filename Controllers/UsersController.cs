using Ardalis.GuardClauses;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Assignment.Service.Controllers.v1.Commands;
using Assignment.Service.Controllers.v1.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static Assignment.Service.Controllers.v1.Commands.CreateNewUserCommand.Handler;
using static Assignment.Service.Controllers.v1.Commands.UpdateUserCommand;
using static Assignment.Service.Controllers.v1.Queries.GetUserDetailsQuery.Handler;

namespace Assignment.Service.Controllers
{
    [Route("api/v1")]
    [ApiController]
    [TranslateResultToActionResult]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        [Route("createuser")]
        [SwaggerOperation(Summary = "Create new user")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateNewUserOutputDto))]
        public async Task<Result<CreateNewUserOutputDto>> CreateNewUser([FromBody] CreateNewUserCommand request)
        {
           Guard.Against.Null(request, nameof(request));

            return await _mediator.Send(request);
        }

        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Get user details")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDetailsOutputDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Result<UserDetailsOutputDto>> GetUserDetails([FromRoute] int id)
        {
            var cancellationToken = HttpContext.RequestAborted;
            return await _mediator.Send(new GetUserDetailsQuery(id), cancellationToken);
        }

        [HttpPut]
        [Route("{id}")]
        [SwaggerOperation(Summary = "update user details")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Unit))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Result<Unit>> UpdateUserDetails([FromRoute] int id, [FromBody] UserInputDto userData)
        {
            return await _mediator.Send(new UpdateUserCommand(id, userData));
        }
    }
}
