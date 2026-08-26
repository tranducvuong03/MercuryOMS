using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features 
{
    public record DeleteUserAddressCommand(Guid Id)
        : IRequest<Result>;

    public class DeleteUserAddressHandler
        : IRequestHandler<DeleteUserAddressCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public DeleteUserAddressHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(
            DeleteUserAddressCommand request,
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

            userAddressRepo.Remove(userAddress);

            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success(
                Message.DeleteSuccessfully);
        }
    }
}