using MercuryOMS.Domain.Commons;
using MercuryOMS.Domain.Enums;

namespace MercuryOMS.Domain.Entities
{
    public class SupportTicket : AggregateRoot
    {
        private readonly List<TicketMessage> _messages = new();

        public string Title { get; private set; } = null!;
        public string Description { get; private set; } = null!;

        public Guid CreatedByUserId { get; private set; }
        public Guid? AssignedStaffId { get; private set; }

        public TicketStatus Status { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? ClosedAt { get; private set; }

        public IReadOnlyCollection<TicketMessage> Messages => _messages.AsReadOnly();

        private SupportTicket() { }

        public SupportTicket(Guid createdByUserId, string title, string description)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required.");

            Id = Guid.NewGuid();
            CreatedByUserId = createdByUserId;
            Title = title.Trim();
            Description = description.Trim();

            Status = TicketStatus.Open;
            CreatedAt = DateTime.UtcNow;
        }

        public void AssignStaff(Guid staffId)
        {
            if (Status == TicketStatus.Closed)
                throw new InvalidOperationException("Ticket is closed.");

            AssignedStaffId = staffId;
            Status = TicketStatus.InProgress;
        }

        public void AddMessage(Guid senderId, string message)
        {
            if (Status == TicketStatus.Closed)
                throw new InvalidOperationException("Ticket is closed.");

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message is required.");

            _messages.Add(new TicketMessage(Id, senderId, message));
        }

        public void Close(Guid staffId)
        {
            if (Status == TicketStatus.Closed)
                throw new InvalidOperationException("Ticket already closed.");

            AssignedStaffId ??= staffId;
            Status = TicketStatus.Closed;
            ClosedAt = DateTime.UtcNow;
        }
    }
}
