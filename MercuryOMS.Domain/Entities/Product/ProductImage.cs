using MercuryOMS.Domain.Commons;

namespace MercuryOMS.Domain.Entities
{
    public class ProductImage : BaseEntity
    {
        public Guid ProductId { get; private set; }
        public string Url { get; private set; } = null!;
        public bool IsPrimary { get; private set; }

        private ProductImage() { }

        internal ProductImage(Guid productId, string url, bool isPrimary)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            Url = url;
            IsPrimary = isPrimary;
        }

        internal void UnsetPrimary()
        {
            IsPrimary = false;
        }
    }
}
