using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Entities;

namespace MercuryOMS.Application.Features
{
    public record GetTotalPriceOrderQuery(Guid OrderId)
        : IRequest<Result<decimal>>;

    public class GetTotalPriceOrderQueryHandler
        : IRequestHandler<GetTotalPriceOrderQuery, Result<decimal>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTotalPriceOrderQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<decimal>> Handle(
            GetTotalPriceOrderQuery request,
            CancellationToken cancellationToken)
        {
            var orderRepository = _unitOfWork.GetRepository<Order>();

            var order = await orderRepository.GetByIdAsync(
                request.OrderId,
                cancellationToken);

            if (order is null)
            {
                return Result<decimal>.Failure("Order not found.");
            }

            return Result<decimal>.Success(order.TotalAmount);
        }
    }
}