namespace Accounting.Domain.DTOs;

public record JournalLineDto(Guid AccountId, decimal Amount);