using AutoMapper;
using MediatR;
using MercuryOMS.Application.Interfaces;
using MercuryOMS.Application.Models.Responses;

namespace MercuryOMS.Application.Queries.Category
{
    public record GetAllCategoryQuery()
    : IRequest<List<CategoryResponse>>;

    public class GetAllCategoryQueryHandler
    : IRequestHandler<GetAllCategoryQuery, List<CategoryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllCategoryQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<CategoryResponse>> Handle(
            GetAllCategoryQuery request,
            CancellationToken cancellationToken)
        {
            var categories = _unitOfWork
                .GetRepository<Domain.Entities.Category>()
                .Query
                .ToList();

            return _mapper.Map<List<CategoryResponse>>(categories);
        }
    }
}
