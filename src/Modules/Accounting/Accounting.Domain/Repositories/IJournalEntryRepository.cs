using Accounting.Domain.Aggregates;

namespace Accounting.Domain.Repositories;

public interface IJournalEntryRepository
{
    Task AddAsync(JournalEntry journalEntry);
}
