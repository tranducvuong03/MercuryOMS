using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.UOW;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record GetCartItemCountQuery() : IRequest<Result<int>>;

    public class GetCartItemCountHandler
    : IRequestHandler<GetCartItemCountQuery, Result<int>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetCartItemCountHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<int>> Handle(
            GetCartItemCountQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var cartRepository = _unitOfWork.GetRepository<Cart>();

            var cart = await cartRepository.QueryNoTracking
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

            if (cart == null)
                return Result<int>.Success(0);

            var count = cart.Items.Sum(x => x.Quantity);

            return Result<int>.Success(count);
        }
    }
}
