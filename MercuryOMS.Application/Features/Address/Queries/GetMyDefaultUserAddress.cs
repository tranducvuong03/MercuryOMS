using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features.Address.Queries
{
    public record GetMyDefaultUserAddressQuery
    : IRequest<Result<UserAddressResponse>>;

    public class GetMyDefaultUserAddressHandler
    : IRequestHandler<GetMyDefaultUserAddressQuery, Result<UserAddressResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public GetMyDefaultUserAddressHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<UserAddressResponse>> Handle(
            GetMyDefaultUserAddressQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;

            if (userId == null)
                throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");

            var repository = _unitOfWork.GetRepository<UserAddress>();

            var address = await repository.Query
                .AsNoTracking()
                .Where(x => x.UserId == userId.Value && x.IsDefault)
                .Select(x => new UserAddressResponse
                {
                    Id = x.Id,
                    Label = x.Label,
                    ReceiverName = x.Address.ReceiverName,
                    Phone = x.Address.Phone,
                    Street = x.Address.Street,
                    District = x.Address.District,
                    City = x.Address.City,
                    Province = x.Address.Province,
                    IsDefault = x.IsDefault
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (address == null)
                return Result<UserAddressResponse>.Failure("Không tìm thấy địa chỉ mặc định.");

            return Result<UserAddressResponse>.Success(address);
        }
    }
}
