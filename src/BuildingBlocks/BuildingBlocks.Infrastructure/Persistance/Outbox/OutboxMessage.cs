namespace BuildingBlocks.Infrastructure.Persistance.Outbox
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public DateTime OccuredOn { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public DateTime? ProcessedOn { get; set; }
        public string? Error { get; set; }
    }
}
