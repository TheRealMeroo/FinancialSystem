using Accounting.Domain.Enums;
using BuildingBlocks.Domain.Events;

namespace Accounting.Domain.Events.JournalEntry;

public record JournalEntryCreatedEvent(string description,
    DateTime createdAt,
    Guid correlationId,
    JournalEntryStatus status) : DomainEventBase;