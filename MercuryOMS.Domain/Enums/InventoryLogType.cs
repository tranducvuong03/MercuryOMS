namespace MercuryOMS.Domain.Enums
{
    public enum InventoryLogType
    {
        StockIn,     // nhập kho
        Reserve,     // giữ chỗ
        Commit,      // bán (chốt)
        Release,     // trả kho
        Adjust       // chỉnh tay (admin)
    }
}
