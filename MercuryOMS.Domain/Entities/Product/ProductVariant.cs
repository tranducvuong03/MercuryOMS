using MercuryOMS.Domain.Commons;

namespace MercuryOMS.Domain.Entities
{
    public class ProductVariant : BaseEntity
    {
        public Guid ProductId { get; private set; }
        public string Sku { get; private set; } = null!;
        public decimal Price { get; private set; }
        public int Stock { get; private set; }

        private ProductVariant() { }

        internal ProductVariant(Guid productId, string sku, decimal price, int stock)
        {
            if (price <= 0)
                throw new ArgumentException("Price must be greater than zero.");

            if (stock < 0)
                throw new ArgumentException("Stock cannot be negative.");

            Id = Guid.NewGuid();
            ProductId = productId;
            Sku = sku;
            Price = price;
            Stock = stock;
        }

        internal void SetStock(int stock)
        {
            if (stock < 0)
                throw new ArgumentException("Stock cannot be negative.");

            Stock = stock;
        }
    }
}
