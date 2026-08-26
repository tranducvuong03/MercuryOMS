using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record SetDefaultUserAddressCommand(Guid Id)
        : IRequest<Result>;

    public class SetDefaultUserAddressHandler
       : IRequestHandler<SetDefaultUserAddressCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public SetDefaultUserAddressHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(
            SetDefaultUserAddressCommand request,
            CancellationToken ct)
        {
            var userId = _currentUser.UserId;

            if (userId == null)
                throw new UnauthorizedAccessException(
                    "Người dùng chưa đăng nhập.");

            var userAddressRepo =
                _unitOfWork.GetRepository<UserAddress>();

            var addresses = await userAddressRepo.Query
                .Where(x => x.UserId == userId.Value)
                .ToListAsync(ct);

            var selectedAddress = addresses
                .FirstOrDefault(x => x.Id == request.Id);

            if (selectedAddress == null)
                return Result.Failure("Không tìm thấy địa chỉ");

            foreach (var address in addresses)
            {
                address.RemoveDefault();
            }

            selectedAddress.SetDefault();

            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success(
                Message.UpdateSuccessfully);
        }
    }
}
