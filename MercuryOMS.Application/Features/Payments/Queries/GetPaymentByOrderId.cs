using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record GetPaymentByOrderIdQuery(Guid OrderId)
        : IRequest<Result<PaymentResponse>>;

    public class GetPaymentByOrderIdQueryHandler
        : IRequestHandler<GetPaymentByOrderIdQuery, Result<PaymentResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetPaymentByOrderIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Result<PaymentResponse>> Handle(
            GetPaymentByOrderIdQuery request,
            CancellationToken cancellationToken)
        {
            var payment = await _uow.GetRepository<Payment>()
                .QueryNoTracking
                .Where(x => x.OrderId == request.OrderId)
                .Select(x => new PaymentResponse
                {
                    Id = x.Id,
                    OrderId = x.OrderId,
                    Amount = x.Amount,
                    Status = x.Status,
                    PaymentMethod = x.PaymentMethod,
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (payment is null)
                return Result<PaymentResponse>.Failure(Message.PaymentNotFound);

            return Result<PaymentResponse>.Success(payment);
        }
    }
}