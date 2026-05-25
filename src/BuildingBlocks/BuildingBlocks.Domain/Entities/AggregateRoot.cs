using BuildingBlocks.Domain.Events;

namespace BuildingBlocks.Domain.Entities
{
    public abstract class AggregateRoot : Entity
    {
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public long Version { get; private set; } = 0;

        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            Version++;
            if (domainEvent is DomainEventBase baseEvent)
            {
                typeof(DomainEventBase)
                    .GetProperty(nameof(IDomainEvent.AggregateVersion))?
                    .SetValue(baseEvent, Version);
            }

            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
