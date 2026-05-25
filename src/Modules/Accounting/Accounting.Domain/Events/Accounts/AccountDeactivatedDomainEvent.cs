using BuildingBlocks.Domain.Events;

namespace Accounting.Domain.Events.Accounts;

public record AccountDeactivatedDomainEvent(Guid accountId) : DomainEventBase;
