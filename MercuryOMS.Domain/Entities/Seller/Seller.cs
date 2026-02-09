using MercuryOMS.Domain.Commons;

namespace MercuryOMS.Domain.Entities
{
    public class Seller : AggregateRoot, IAuditableUser
    {
        private readonly List<SellerProduct> _products = new();

        public string Name { get; private set; } = null!;
        public bool IsActive { get; private set; }

        public SellerBalance Balance { get; private set; } = null!;

        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }

        public IReadOnlyCollection<SellerProduct> Products => _products.AsReadOnly();

        private Seller() { } // EF Core

        public Seller(string name)
        {
            Id = Guid.NewGuid();
            SetName(name);

            IsActive = true;
            Balance = new SellerBalance(Id);
        }

        // -------- Core behavior --------
        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Seller name is required.");

            Name = name.Trim();
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;

        // -------- Product management --------
        public void AddProduct(Guid productId, decimal commissionRate)
        {
            if (commissionRate < 0 || commissionRate > 1)
                throw new ArgumentException("Commission rate must be between 0 and 1.");

            if (_products.Any(x => x.ProductId == productId))
                return;

            _products.Add(new SellerProduct(Id, productId, commissionRate));
        }

        public void RemoveProduct(Guid productId)
        {
            var sp = _products.FirstOrDefault(x => x.ProductId == productId);
            if (sp != null)
                _products.Remove(sp);
        }

        // -------- Balance behavior --------
        public void Credit(decimal amount) => Balance.Credit(amount);
        public void Debit(decimal amount) => Balance.Debit(amount);
    }
}
