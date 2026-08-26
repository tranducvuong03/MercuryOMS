using MediatR;
using MercuryOMS.Application.Features;
using MercuryOMS.Application.Features.Address.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MercuryOMS.API.Controllers
{
    [ApiController]
    [Route("api/user-addresses")]
    [Authorize]
    public class UserAddressesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserAddressesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyAddresses()
        {
            var result = await _mediator.Send(
                new GetMyUserAddressesQuery());

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("default")]
        public async Task<IActionResult> GetMyDefaultAddress()
        {
            var result = await _mediator.Send(
                new GetMyDefaultUserAddressQuery());

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(
                new GetUserAddressByIdQuery(id));

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateUserAddressCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(
            UpdateUserAddressCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteUserAddressCommand(id);

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPatch("{id}/default")]
        public async Task<IActionResult> SetDefault(Guid id)
        {
            var command = new SetDefaultUserAddressCommand(id);

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}