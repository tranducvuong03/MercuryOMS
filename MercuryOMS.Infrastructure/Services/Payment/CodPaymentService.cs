using MercuryOMS.Application.IServices;
using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Domain.Entities;

namespace MercuryOMS.Infrastructure.Services
{
    public class CodPaymentService : IPaymentStrategyService
    {
        public string Method => "COD";

        public async Task<PaymentResult> CreatePaymentAsync(
            Guid orderId,
            decimal amount,
            CancellationToken ct)
        {
            var payment = new Payment(orderId, amount, Method);

            return new PaymentResult { Payment = payment};
        }
    }
}
