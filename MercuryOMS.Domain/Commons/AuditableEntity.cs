namespace MercuryOMS.Domain.Commons
{
    public abstract class AuditableEntity : BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
    }
}
