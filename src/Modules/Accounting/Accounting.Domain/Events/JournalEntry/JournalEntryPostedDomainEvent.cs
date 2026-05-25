using Accounting.Domain.DTOs;
using BuildingBlocks.Domain.Events;

namespace Accounting.Domain.Events.JournalEntry;

public record JournalEntryPostedDomainEvent(
    Guid JournalEntryId,
    Guid CorrelationId,
    List<JournalLineDto> JournalLineDtos) : DomainEventBase;
