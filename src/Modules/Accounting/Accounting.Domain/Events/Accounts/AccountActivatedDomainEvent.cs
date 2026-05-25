using BuildingBlocks.Domain.Events;

namespace Accounting.Domain.Events.Accounts;

public record AccountActivatedDomainEvent(Guid accountId) : DomainEventBase;
