using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record GetAllOrdersQuery
        : IRequest<Result<List<OrderResponse>>>;

    public class GetAllOrdersQueryHandler
        : IRequestHandler<GetAllOrdersQuery, Result<List<OrderResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public GetAllOrdersQueryHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<List<OrderResponse>>> Handle(
            GetAllOrdersQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;

            if (userId is null)
            {
                return Result<List<OrderResponse>>
                    .Failure("User chưa đăng nhập.");
            }

            var orders = await _unitOfWork.GetRepository<Order>()
                .QueryNoTracking
                .Where(x => x.UserId == userId.Value)
                .OrderByDescending(x => x.OrderDate)
                .Select(x => new OrderResponse
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    OrderDate = x.OrderDate,
                    Status = x.Status.ToString(),
                    TotalAmount = x.TotalAmount,

                    ReceiverName = x.ShippingAddress.ReceiverName,
                    Phone = x.ShippingAddress.Phone,
                    Street = x.ShippingAddress.Street,
                    District = x.ShippingAddress.District,
                    City = x.ShippingAddress.City,
                    Province = x.ShippingAddress.Province
                })
                .ToListAsync(cancellationToken);

            return Result<List<OrderResponse>>.Success(orders);
        }
    }
}