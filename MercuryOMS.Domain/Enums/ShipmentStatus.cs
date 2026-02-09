namespace MercuryOMS.Domain.Enums
{
    public enum ShipmentStatus
    {
        Pending,     // mới tạo
        Ready,       // sẵn sàng giao
        Shipping,    // đang giao
        Delivered,   // giao thành công
        Failed,      // giao thất bại
        Cancelled    // hủy giao
    }
}
