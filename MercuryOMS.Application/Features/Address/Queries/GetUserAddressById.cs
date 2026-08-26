using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features 
{
    public record GetUserAddressByIdQuery(Guid Id)
        : IRequest<Result<UserAddressResponse>>;

    public class GetUserAddressByIdHandler
        : IRequestHandler<
            GetUserAddressByIdQuery,
            Result<UserAddressResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public GetUserAddressByIdHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<UserAddressResponse>> Handle(
            GetUserAddressByIdQuery request,
            CancellationToken ct)
        {
            var userId = _currentUser.UserId;

            if (userId == null)
                throw new UnauthorizedAccessException(
                    "Người dùng chưa đăng nhập.");

            var repo = _unitOfWork.GetRepository<UserAddress>();

            var address = await repo.Query
                .Where(x =>
                    x.Id == request.Id &&
                    x.UserId == userId.Value)
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
                .FirstOrDefaultAsync(ct);

            if (address == null)
                return Result<UserAddressResponse>
                    .Failure("Không tìm thấy địa chỉ");

            return Result<UserAddressResponse>
                .Success(address);
        }
    }
}