using BuildingBlocks.Application.DTOs;

namespace BuildingBlocks.Application.Abstractions.Persistance
{
    public interface IIdempotencyRepository
    {
        Task<IdempotentResponse?> GetAsync(string key, string requestType, CancellationToken cancellationToken);
        Task SaveAsync(string key, string requestType, object response, CancellationToken cancellationToken);
    }
}
