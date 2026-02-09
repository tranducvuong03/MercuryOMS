namespace MercuryOMS.Domain.Commons
{
    public interface IAuditableUser
    {
        string? CreatedBy { get; set; }
        string? LastModifiedBy { get; set; }
    }
}
