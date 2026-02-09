using MercuryOMS.Domain.Commons;
using MercuryOMS.Domain.Enums;

namespace MercuryOMS.Domain.Entities
{
    public class Payment : AggregateRoot
    {
        public Guid OrderId { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentStatus Status { get; private set; }
        public string PaymentMethod { get; private set; } = default!;
        public DateTime CreatedAt { get; private set; }

        private Payment() { }

        public Payment(Guid orderId, decimal amount, string paymentMethod)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");

            Id = Guid.NewGuid();
            OrderId = orderId;
            Amount = amount;
            PaymentMethod = paymentMethod;
            Status = PaymentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkPaid()
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException("Payment is not pending.");

            Status = PaymentStatus.Paid;

            //AddDomainEvent(new PaymentPaidEvent(Id, OrderId, Amount));
        }

        public void MarkFailed(string reason)
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException("Payment is not pending.");

            Status = PaymentStatus.Failed;

            //AddDomainEvent(new PaymentFailedEvent(Id, OrderId, reason));
        }
    }
}
