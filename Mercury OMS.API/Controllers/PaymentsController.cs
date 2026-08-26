using MediatR;
using MercuryOMS.Application.Features;
using Microsoft.AspNetCore.Mvc;

namespace MercuryOMS.API.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("order/{orderId:guid}")]
        public async Task<IActionResult> GetByOrderId(
            Guid orderId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetPaymentByOrderIdQuery(orderId),
                cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(
            [FromQuery] CreatePaymentCommand command,
            CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("ipn")]
        public async Task<IActionResult> Ipn(CancellationToken ct)
        {
            var command = new VnPayIpnCommand
            {
                Parameters = Request.Query.ToDictionary(
                    x => x.Key,
                    x => x.Value.ToString())
            };

            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
                return BadRequest(new
                {
                    RspCode = result.Message,
                    Message = result.Message
                });

            return Ok(new
            {
                RspCode = "00",
                Message = "Confirm Success"
            });
        }
    }
}