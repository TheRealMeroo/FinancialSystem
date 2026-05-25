using Accounting.Domain.Enums;
using BuildingBlocks.Domain.Events;

namespace Accounting.Domain.Events.Accounts;

public record AccountCreatedDomainEvent(Guid accountId, Guid identityId, AccountType type) : DomainEventBase;
