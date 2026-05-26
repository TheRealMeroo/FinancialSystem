using Accounting.Domain.Aggregates;

namespace Accounting.Domain.Repositories
{
    public interface IDepositRequestRepository
    {
        Task<DepositRequest> AddAsync(DepositRequest request);
    }
}
