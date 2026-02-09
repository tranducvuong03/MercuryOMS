namespace MercuryOMS.Domain.Entities
{
    public class OutboxMessage
    {
        public Guid Id { get; private set; }
        public string Type { get; private set; } = default!;
        public string Payload { get; private set; } = default!;
        public bool IsProcessed { get; private set; }
        public DateTime OccurredOn { get; private set; }
        public DateTime? ProcessedOn { get; private set; }

        private OutboxMessage() { }

        public OutboxMessage(string type, string payload)
        {
            Id = Guid.NewGuid();
            Type = type;
            Payload = payload;
            IsProcessed = false;
        }

        public void MarkAsProcessed()
        {
            IsProcessed = true;
        }
    }
}
