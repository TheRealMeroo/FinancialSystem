using BuildingBlocks.Domain.Events;

namespace Accounting.Domain.Events.DepositRequest;

public record DepositRequestedDomainEvent(
    Guid DepositRequestId,
    Guid AccountId,
    decimal Amount,
    string ReferenceNumber) : DomainEventBase;
