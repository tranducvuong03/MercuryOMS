using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Application.UOW;

namespace MercuryOMS.Application.Features
{
    public record GetCurrentUserQuery : IRequest<Result<UserResponse>>;

    public class GetCurrentUserQueryHandler
        : IRequestHandler<GetCurrentUserQuery, Result<UserResponse>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        public GetCurrentUserQueryHandler(
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UserResponse>> Handle(
            GetCurrentUserQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (userId is null)
                return Result<UserResponse>.Failure("Unauthorized");

            return Result<UserResponse>.Success(new UserResponse
            {
                Id = _currentUserService.UserId,
                Email = _currentUserService.Email,
                FullName = _currentUserService.FullName
            });
        }
    }
}
