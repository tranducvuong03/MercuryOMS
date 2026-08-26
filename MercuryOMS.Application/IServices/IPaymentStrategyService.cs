using MercuryOMS.Application.Features;
using MercuryOMS.Application.Models.Responses;

namespace MercuryOMS.Application.IServices
{
    public interface IPaymentStrategyService
    {
        string Method { get; }

        Task<PaymentResult> CreatePaymentAsync(
            Guid orderId,
            decimal amount,
            CancellationToken ct);
    }
}
