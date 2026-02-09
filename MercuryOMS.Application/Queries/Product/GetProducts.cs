using AutoMapper;
using MediatR;
using MercuryOMS.Application.Common;
using MercuryOMS.Application.Commons.Models.Responses.Product;
using MercuryOMS.Application.Interfaces;

namespace MercuryOMS.Application.Queries.Product
{
    public record GetProducts(int pageIndex, int pageSize) : IRequest<BasePaginated<ProductResponse>>;

    public class GetProductsHandler : IRequestHandler<GetProducts, BasePaginated<ProductResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<BasePaginated<ProductResponse>> Handle(
            GetProducts request,
            CancellationToken cancellationToken)
        {
            var products = _unitOfWork
                .GetRepository<Domain.Entities.Product>()
                .GetByFilterWithPaginated(request.pageIndex, request.pageSize)
                .ToList(); 

            var productResponse = _mapper.Map<List<ProductResponse>>(products);

            var result = new BasePaginated<ProductResponse>(
                productResponse,
                request.pageIndex,
                request.pageSize,
                products.Count
            );

            return Task.FromResult(result);
        }

    }
}
