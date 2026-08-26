using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record GetMyUserAddressesQuery()
        : IRequest<Result<List<UserAddressResponse>>>;

    public class GetMyUserAddressesHandler
        : IRequestHandler<
            GetMyUserAddressesQuery,
            Result<List<UserAddressResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public GetMyUserAddressesHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<List<UserAddressResponse>>> Handle(
            GetMyUserAddressesQuery request,
            CancellationToken ct)
        {
            var userId = _currentUser.UserId;

            if (userId == null)
                throw new UnauthorizedAccessException(
                    "Người dùng chưa đăng nhập.");

            var repo = _unitOfWork.GetRepository<UserAddress>();

            var addresses = await repo.Query
                .Where(x => x.UserId == userId.Value)
                .OrderByDescending(x => x.IsDefault)
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
                .ToListAsync(ct);

            return Result<List<UserAddressResponse>>
                .Success(addresses);
        }
    }
}