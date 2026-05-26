using Accounting.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Infrastructure.Persistance.Data;

public class AccountingDbContext : DbContext
{
    public AccountingDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<DepositRequest> DepositRequests { get; set; }
    public DbSet<JournalEntry> JournalEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(e =>
        {
            e.HasKey(a => a.Id);
        });

        modelBuilder.Entity<DepositRequest>(e =>
        {
            e.HasKey(d => d.Id);
            e.HasIndex(d => d.ReferenceNumber).IsUnique();
        });

        modelBuilder.Entity<JournalEntry>(e =>
        {
            e.HasKey(d => d.Id);
        });
    }
}
