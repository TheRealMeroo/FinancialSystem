using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Infrastructure.Persistance.Idempotency;
using BuildingBlocks.Infrastructure.Persistance.Outbox;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BuildingBlocks.Infrastructure.Persistance
{
    public class BaseDbContext : DbContext
    {
        public BaseDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<IdempotentRequest> IdempotentRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OutboxMessage>(e =>
            {
                e.HasKey(e => e.Id);
            });

            modelBuilder.Entity<IdempotentRequest>(e =>
            {
                e.HasKey(e => e.Id);
                e.HasIndex(e => new { e.Key, e.RequestType }).IsUnique();
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var domainEntities = ChangeTracker.Entries<AggregateRoot>()
                .Where(e => e.Entity.DomainEvents.Any())
                .ToList();

            var outboxMessage = domainEntities.SelectMany(x => x.Entity.DomainEvents
            .Select(domainEvent => new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccuredOn = DateTime.UtcNow,
                Type = domainEvent.GetType().Name,
                Content = JsonSerializer.Serialize(domainEvent, domainEvent.GetType())
            })).ToList();

            if (outboxMessage.Any())
            {
                await OutboxMessages.AddRangeAsync(outboxMessage, cancellationToken);
            }

            domainEntities.ForEach(e => e.Entity.ClearDomainEvents());

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
