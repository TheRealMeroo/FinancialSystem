using BuildingBlocks.Domain.Entities;

namespace Accounting.Domain.Entities;

public class JournalEntryLine : Entity
{
    public Guid AccountId { get; private set; }
    public decimal Amount { get; private set; }
    public string Description { get; private set; }

    private JournalEntryLine() { }

    internal JournalEntryLine(Guid accountId, decimal amount, string description)
    {
        AccountId = accountId;
        Amount = amount;
        Description = description;
    }
}
