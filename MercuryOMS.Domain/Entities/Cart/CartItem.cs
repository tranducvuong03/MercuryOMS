using MercuryOMS.Domain.Commons;

namespace MercuryOMS.Domain.Entities
{
    public class CartItem : BaseEntity
    {
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }

        private CartItem() { }

        internal CartItem(Guid productId, int quantity)
        {
            if (quantity <= 0) 
            {
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
            };

            Id = Guid.NewGuid();
            ProductId = productId;
            Quantity = quantity;
        }

        internal void SetQuantity(int quantity)
        {
            if (quantity <= 0) 
            {
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
            };
            Quantity = quantity;
        }

        internal void Increase(int amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Increase amount must be greater than zero.");

            Quantity += amount;
        }

        internal void Decrease(int amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Decrease amount must be greater than zero.");

            if (Quantity - amount <= 0)
                throw new ArgumentException("Quantity cannot be zero or negative.");

            Quantity -= amount;
        }

    }
}
