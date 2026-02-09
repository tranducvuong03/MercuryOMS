using MediatR;
using MercuryOMS.Application.Commands;
using MercuryOMS.Application.Models.Requests.Category;
using MercuryOMS.Application.Queries.Category;
using Microsoft.AspNetCore.Mvc;
using static MercuryOMS.Application.Queries.Category.GetById;

namespace MercuryOMS.API.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(
            [FromBody] CreateCategoryRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateCategoryCommand(
                request.Name,
                request.Description
            );
            var categoryId = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(
                nameof(Get),
                new { id = categoryId },
                null);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _mediator.Send(new GetAllCategoryQuery()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _mediator.Send(new GetByIdQuery(id)));
        }
    }
}
