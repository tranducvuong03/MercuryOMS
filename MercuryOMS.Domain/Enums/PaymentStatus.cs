namespace MercuryOMS.Domain.Enums
{
    public enum PaymentStatus
    {
        Pending,     // mới tạo, chờ thanh toán
        Paid,        // đã thanh toán thành công
        Failed,      // thanh toán thất bại
        Refunded,    // đã hoàn tiền
        Cancelled    // bị hủy (timeout / user hủy)
    }
}
