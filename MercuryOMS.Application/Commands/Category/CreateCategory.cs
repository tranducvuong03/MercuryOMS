using MediatR;
using MercuryOMS.Application.Interfaces;
using MercuryOMS.Domain.Entities;

namespace MercuryOMS.Application.Commands
{
    public record CreateCategoryCommand(
        string Name,
        string Description
    ) : IRequest<Guid>;

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCategoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(
            CreateCategoryCommand request,
            CancellationToken cancellationToken)
        {
            var category = new Category(
                request.Name,
                request.Description
            );

            await _unitOfWork.GetRepository<Category>().AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return category.Id;
        }
    }
}