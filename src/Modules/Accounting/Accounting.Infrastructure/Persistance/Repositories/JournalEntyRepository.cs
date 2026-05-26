using Accounting.Domain.Aggregates;
using Accounting.Domain.Repositories;
using Accounting.Infrastructure.Persistance.Data;

namespace Accounting.Infrastructure.Persistance.Repositories;

internal class JournalEntyRepository : IJournalEntryRepository
{
    private readonly AccountingDbContext _context;

    public JournalEntyRepository(AccountingDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(JournalEntry journalEntry)
    {
        await _context
            .JournalEntries
            .AddAsync(journalEntry);
    }
}
