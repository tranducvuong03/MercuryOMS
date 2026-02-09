using MediatR;

namespace MercuryOMS.Domain.Commons
{
    public interface IDomainEvent : INotification
    {
        DateTime OccurredOn { get; }
    }
}
