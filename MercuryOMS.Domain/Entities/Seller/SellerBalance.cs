namespace MercuryOMS.Domain.Entities
{
    public class SellerBalance
    {
        public Guid SellerId { get; private set; }
        public decimal Amount { get; private set; }

        private SellerBalance() { }

        internal SellerBalance(Guid sellerId)
        {
            SellerId = sellerId;
            Amount = 0;
        }

        internal void Credit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");

            Amount += amount;
        }

        internal void Debit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");

            if (Amount < amount)
                throw new InvalidOperationException("Insufficient seller balance.");

            Amount -= amount;
        }
    }
}
