using Accounting.Domain.Aggregates;
using Accounting.Domain.Repositories;
using Accounting.Infrastructure.Persistance.Data;

namespace Accounting.Infrastructure.Persistance.Repositories;

internal class DepositRequestRepository : IDepositRequestRepository
{
    private readonly AccountingDbContext _context;

    public DepositRequestRepository(AccountingDbContext context)
    {
        _context = context;
    }

    public async Task<DepositRequest> AddAsync(DepositRequest request)
    {
        var entity = await _context
            .DepositRequests
            .AddAsync(request);

        return entity.Entity;
    }
}
