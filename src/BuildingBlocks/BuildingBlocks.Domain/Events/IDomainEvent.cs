namespace BuildingBlocks.Domain.Events
{
    public interface IDomainEvent
    {
        Guid EventId { get; }
        DateTime OccurredOn { get; }
        long AggregateVersion { get; }
    }

    public abstract record DomainEventBase : IDomainEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
        public long AggregateVersion { get; internal set; }
    }
}
