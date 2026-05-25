using Accounting.Domain.DTOs;
using Accounting.Domain.Entities;
using Accounting.Domain.Enums;
using Accounting.Domain.Errors;
using Accounting.Domain.Events.JournalEntry;
using Accounting.Domain.ValueObjects;
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Exceptions;

namespace Accounting.Domain.Aggregates;

public class JournalEntry : AggregateRoot
{
    public string Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid CorrelationId { get; private set; }
    public JournalEntryStatus Status { get; private set; }

    private readonly List<JournalEntryLine> _lines = new();
    public IReadOnlyCollection<JournalEntryLine> Lines => _lines.AsReadOnly();

    private JournalEntry() { }

    private JournalEntry(string description, Guid correlationId)
    {
        Description = description;
        CorrelationId = correlationId;
        CreatedAt = DateTime.UtcNow;
        Status = JournalEntryStatus.Draft;
    }

    public static JournalEntry Create(Guid correlationId, string description)
        => new JournalEntry(description, correlationId);

    public void AddLine(Guid accountId, TransactionAmount amount, string description)
    {
        if (Status != JournalEntryStatus.Draft)
            throw new DomainException(
                new DomainError(
                    AccountingDomainErrorCodes.InvalidStatus,
                    "Cannot add lines to posted transactions."));

        _lines.Add(new JournalEntryLine(accountId, amount.Value, description));
    }

    public void Post()
    {
        if (Status != JournalEntryStatus.Draft) return;

        if (_lines.Count < 2)
            throw new DomainException(
                new DomainError(
                    AccountingDomainErrorCodes.AtLeastTwoLinesRequired,
                    AccountingDomainErrorCodes.AtLeastTwoLinesRequired.ToString()));

        if (_lines.Sum(x => x.Amount) != 0)
            throw new DomainException(
                new DomainError(
                    AccountingDomainErrorCodes.InvalidSumOfCreditAndDebit,
                    AccountingDomainErrorCodes.InvalidSumOfCreditAndDebit.ToString()));

        Status = JournalEntryStatus.Posted;

        AddDomainEvent(new JournalEntryPostedDomainEvent(
            this.Id,
            this.CorrelationId,
            _lines.Select(l => new JournalLineDto(l.AccountId, l.Amount)).ToList()));
    }
}
