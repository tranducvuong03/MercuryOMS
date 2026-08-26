using MercuryOMS.Domain.Enums;

namespace MercuryOMS.Application.Models.Responses
{
    /// <summary>
    /// Class trả về thông tin thanh toán thành công ở client
    /// </summary>
    public class PaymentResponse
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public string PaymentMethod { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}
