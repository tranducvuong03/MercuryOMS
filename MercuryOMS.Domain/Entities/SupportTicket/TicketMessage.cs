using MercuryOMS.Domain.Commons;

namespace MercuryOMS.Domain.Entities
{
    public class TicketMessage : BaseEntity
    {
        public Guid TicketId { get; private set; }
        public Guid SenderId { get; private set; }   // user hoặc staff
        public string Content { get; private set; } = null!;
        public DateTime SentAt { get; private set; }

        private TicketMessage() { }

        internal TicketMessage(
            Guid ticketId,
            Guid senderId,
            string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Message content is required.");

            Id = Guid.NewGuid();
            TicketId = ticketId;
            SenderId = senderId;
            Content = content.Trim();
            SentAt = DateTime.UtcNow;
        }
    }
}
