namespace BuildingBlocks.Application.Abstractions.Persistance
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
