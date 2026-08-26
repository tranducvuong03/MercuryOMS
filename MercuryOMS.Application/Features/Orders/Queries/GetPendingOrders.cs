using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Entities;
using MercuryOMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features;

public record GetPendingOrdersQuery
    : IRequest<Result<List<PendingOrderResponse>>>;

public class GetPendingOrdersQueryHandler
    : IRequestHandler<GetPendingOrdersQuery, Result<List<PendingOrderResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public GetPendingOrdersQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<List<PendingOrderResponse>>> Handle(
        GetPendingOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        if (userId is null)
            return Result<List<PendingOrderResponse>>
                .Failure("User chưa đăng nhập.");

        var orders = await _unitOfWork.GetRepository<Order>()
            .QueryNoTracking
            .Where(x => x.UserId == userId &&
                        x.Status == OrderStatus.Pending)
            .Select(x => new PendingOrderResponse
            {
                Id = x.Id,
                OrderDate = x.OrderDate,
                TotalAmount = x.TotalAmount,
                Status = x.Status.ToString(),

                Items = x.Items.Select(i => new PendingOrderItemResponse
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductVariantId = i.ProductVariantId,

                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        return Result<List<PendingOrderResponse>>
            .Success(orders);
    }
}