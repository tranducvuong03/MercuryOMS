namespace MercuryOMS.Domain.Entities
{
    public class ProductCategory
    {
        public Guid ProductId { get; private set; }
        public Guid CategoryId { get; private set; }

        private ProductCategory() { }

        internal ProductCategory(Guid productId, Guid categoryId)
        {
            ProductId = productId;
            CategoryId = categoryId;
        }
    }
}
