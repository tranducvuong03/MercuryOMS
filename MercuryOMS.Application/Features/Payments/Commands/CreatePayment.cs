using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.UOW;
using MercuryOMS.Application.IServices;
using MercuryOMS.Domain.Entities;
using MercuryOMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record CreatePaymentCommand(
        Guid OrderId,
        PaymentMethod Method
    ) : IRequest<Result<CreatePaymentResponse>>;

    public record CreatePaymentResponse(
        Guid PaymentId,
        string? PaymentUrl
    );

    public class CreatePaymentHandler
        : IRequestHandler<CreatePaymentCommand, Result<CreatePaymentResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentFactory _factory;

        public CreatePaymentHandler(
            IUnitOfWork unitOfWork,
            IPaymentFactory factory)
        {
            _unitOfWork = unitOfWork;
            _factory = factory;
        }

        public async Task<Result<CreatePaymentResponse>> Handle(
            CreatePaymentCommand request,
            CancellationToken ct)
        {
            var strategy = _factory.Get(request.Method.ToString());

            var order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(request.OrderId, o => o.Include(oi => oi.Items));

            var paymentResult = await strategy.CreatePaymentAsync(
                request.OrderId,
                order!.TotalAmount,
                ct
            );

            await _unitOfWork.GetRepository<Payment>()
                .AddAsync(paymentResult.Payment, ct);

            await _unitOfWork.SaveChangesAsync(ct);

            return Result<CreatePaymentResponse>.Success(
                        new CreatePaymentResponse(paymentResult.Payment.Id, paymentResult.PaymentUrl),
                        "Tạo thanh toán thành công");
        }
    }
}