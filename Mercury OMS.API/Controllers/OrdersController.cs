using MediatR;
using MercuryOMS.Application.Features;
using Microsoft.AspNetCore.Mvc;

namespace MercuryOMS.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllOrdersQuery());
            return Ok(result);
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingOrders()
        {
            var result = await _mediator.Send(new GetPendingOrdersQuery());
            return Ok(result);
        }

        [HttpGet("{orderId:guid}/total-price")]
        public async Task<IActionResult> GetTotalPrice(Guid orderId)
        {
            var totalPrice = await _mediator.Send(new GetTotalPriceOrderQuery(orderId));

            return Ok(totalPrice);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(
            [FromBody] CreateOrderFromCartCommand command,
            CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}