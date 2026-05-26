using Accounting.Domain.Aggregates;
using Accounting.Domain.Enums;

namespace Accounting.Domain.Repositories;

public interface IAccountRepository
{
    Task<Account?> GetByIdentityIdAsync(Guid identityId);
    Task<Account?> GetByAccountTypeAsync(AccountType accountType);
}
