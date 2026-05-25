using BuildingBlocks.Application.Abstractions.Persistance;
using BuildingBlocks.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BuildingBlocks.Infrastructure.Persistance.Idempotency
{
    public class IdempotenceyRepository : IIdempotencyRepository
    {
        private readonly BaseDbContext _context;

        public IdempotenceyRepository(BaseDbContext context)
        {
            _context = context;
        }

        public async Task<IdempotentResponse?> GetAsync(string key, string requestType, CancellationToken cancellationToken)
        {
            var entity = await _context
                .IdempotentRequests
                .FirstOrDefaultAsync(i => i.Key == key && i.RequestType == requestType);

            return entity == null ? null : new IdempotentResponse(entity.ResponseData);
        }

        public async Task SaveAsync(string key, string requestType, object response, CancellationToken cancellationToken)
        {
            var entity = new IdempotentRequest
            {
                Id = Guid.NewGuid(),
                Key = key,
                RequestType = requestType,
                ResponseData = JsonSerializer.Serialize(response),
                CreatedAt = DateTime.UtcNow
            };

            await _context.IdempotentRequests.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync();
        }
    }
}
