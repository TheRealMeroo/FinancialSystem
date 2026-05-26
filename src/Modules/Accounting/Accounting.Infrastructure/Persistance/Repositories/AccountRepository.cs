using Accounting.Domain.Aggregates;
using Accounting.Domain.Enums;
using Accounting.Domain.Repositories;
using Accounting.Infrastructure.Persistance.Data;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Infrastructure.Persistance.Repositories;

internal class AccountRepository : IAccountRepository
{
    private readonly AccountingDbContext _context;

    public AccountRepository(AccountingDbContext context)
    {
        _context = context;
    }

    public async Task<Account?> GetByAccountTypeAsync(AccountType accountType)
    {
        return await _context
            .Accounts
            .FirstOrDefaultAsync(a => a.Type == accountType);
    }

    public async Task<Account?> GetByIdentityIdAsync(Guid identityId)
    {
        return await _context
            .Accounts
            .FirstOrDefaultAsync(a => a.IdentityId == identityId);
    }
}
