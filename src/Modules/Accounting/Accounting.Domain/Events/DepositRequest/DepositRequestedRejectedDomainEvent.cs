using BuildingBlocks.Domain.Events;

namespace Accounting.Domain.Events.DepositRequest;

public record DepositRequestedRejectedDomainEvent(
    Guid DepositRequestId,
    string Reason) : DomainEventBase;


