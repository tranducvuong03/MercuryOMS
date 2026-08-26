using MercuryOMS.Domain.Entities;

namespace MercuryOMS.Application.Models.Responses
{
    /// <summary>
    /// Class trả về thông tin để thanh toán và url thanh toán
    /// </summary>
    public class PaymentResult
    {
        public Payment Payment;
        public string? PaymentUrl = null;
    }
}
