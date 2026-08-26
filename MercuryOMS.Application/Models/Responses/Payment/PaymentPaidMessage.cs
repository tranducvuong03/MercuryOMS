namespace MercuryOMS.Application.Models.Responses
{
    /// <summary>
    /// Class trả về thông tin thanh toán thành công ở worker
    /// </summary>
    public class PaymentPaidMessage
    {
        public Guid PaymentId { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
    }
}
