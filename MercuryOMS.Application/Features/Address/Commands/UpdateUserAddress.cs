using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;
using MercuryOMS.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record UpdateUserAddressCommand(
        Guid Id,
        string Label,
        string ReceiverName,
        string Phone,
        string Street,
        string District,
        string City,
        string Province,
        bool IsDefault
    ) : IRequest<Result>;

    public class UpdateUserAddressHandler
        : IRequestHandler<UpdateUserAddressCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public UpdateUserAddressHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(
            UpdateUserAddressCommand request,
            CancellationToken ct)
        {
            var userId = _currentUser.UserId;

            if (userId == null)
                throw new UnauthorizedAccessException(
                    "Người dùng chưa đăng nhập.");

            var userAddressRepo =
                _unitOfWork.GetRepository<UserAddress>();

            var userAddress = await userAddressRepo.Query
                .FirstOrDefaultAsync(
                    x => x.Id == request.Id &&
                         x.UserId == userId.Value,
                    ct);

            if (userAddress == null)
                return Result.Failure("Không tìm thấy địa chỉ");

            var address = new Domain.ValueObjects.Address(
                request.ReceiverName,
                request.Phone,
                request.Street,
                request.District,
                request.City,
                request.Province
            );

            userAddress.Update(
                request.Label,
                request.IsDefault,
                address
            );

            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success(
                Message.UpdateSuccessfully);
        }
    }
}