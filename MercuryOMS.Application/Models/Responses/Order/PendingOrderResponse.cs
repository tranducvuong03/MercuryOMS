namespace MercuryOMS.Application.Models.Responses
{
    public class PendingOrderResponse
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = default!;
        public List<PendingOrderItemResponse> Items { get; set; } = [];
    }
}
