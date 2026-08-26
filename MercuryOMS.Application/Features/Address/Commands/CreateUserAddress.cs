using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record CreateUserAddressCommand(
        string Label,
        string ReceiverName,
        string Phone,
        string Street,
        string District,
        string City,
        string Province,
        bool IsDefault
    ) : IRequest<Result<Guid>>;

    public class CreateUserAddressHandler
        : IRequestHandler<CreateUserAddressCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public CreateUserAddressHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<Guid>> Handle(
            CreateUserAddressCommand request,
            CancellationToken ct)
        {
            var userId = _currentUser.UserId;

            if (userId == null)
                throw new UnauthorizedAccessException(
                    "Người dùng chưa đăng nhập.");

            var userAddressRepo =
                _unitOfWork.GetRepository<UserAddress>();

            if (request.IsDefault)
            {
                var oldDefaults = await userAddressRepo.Query
                    .Where(x =>
                        x.UserId == userId.Value &&
                        x.IsDefault)
                    .ToListAsync(ct);

                foreach (var item in oldDefaults)
                {
                    item.RemoveDefault();
                }
            }

            var address = new Domain.ValueObjects.Address(
                request.ReceiverName,
                request.Phone,
                request.Street,
                request.District,
                request.City,
                request.Province
            );

            var userAddress = new UserAddress(
                userId.Value,
                request.Label,
                address,
                request.IsDefault
            );

            await userAddressRepo.AddAsync(userAddress, ct);

            await _unitOfWork.SaveChangesAsync(ct);

            return Result<Guid>.Success(
                userAddress.Id,
                Message.CreateSuccessfully);
        }
    }
}