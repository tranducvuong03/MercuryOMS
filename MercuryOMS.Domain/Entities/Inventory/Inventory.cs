using MercuryOMS.Domain.Commons;

public class Inventory : AggregateRoot, IAuditableUser
{
    public Guid ProductId { get; private set; }

    public int Available { get; private set; } 
    public int Reserved { get; private set; }
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }

    private Inventory() { }

    public Inventory(Guid productId, int initialQuantity)
    {
        if (initialQuantity < 0)
            throw new ArgumentException("Initial quantity cannot be negative.");

        Id = Guid.NewGuid();
        ProductId = productId;
        Available = initialQuantity;
        Reserved = 0;
    }

    public void Reserve(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        if (Available < quantity)
            throw new ArgumentException("Not enough stock.");

        Available -= quantity;
        Reserved += quantity;
    }

    public void Commit(int quantity)
    {
        if (quantity <= 0 || Reserved < quantity)
            throw new ArgumentException("Invalid commit quantity.");

        Reserved -= quantity;
    }

    public void Release(int quantity)
    {
        if (quantity <= 0 || Reserved < quantity)
            throw new ArgumentException("Invalid release quantity.");

        Reserved -= quantity;
        Available += quantity;
    }

    public void Restock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");
        Available += quantity;
    }
}
