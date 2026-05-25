using BuildingBlocks.Domain.Events;

namespace Accounting.Domain.Events.DepositRequest;

public record DepositRequestedCompletedDomainEvent(
    Guid DepositRequestId,
    Guid AccountId,
    Guid JournalEntryId) : DomainEventBase;


