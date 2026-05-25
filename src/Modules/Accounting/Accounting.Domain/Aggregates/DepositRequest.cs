using Accounting.Domain.Enums;
using Accounting.Domain.Errors;
using Accounting.Domain.Events.DepositRequest;
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Gaurds;

namespace Accounting.Domain.Aggregates;

public class DepositRequest : AggregateRoot
{
    public Guid AccountId { get; private set; }
    public decimal Amount { get; private set; }
    public string ReferenceNumber { get; private set; }
    public DepositRequestStatus Status { get; private set; }
    public Guid? JournalEntryId { get; private set; }
    public string? RejectReason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }

    private DepositRequest() { }

    private DepositRequest(
        Guid accountId,
        decimal amount,
        string referenceNumber)
    {
        Check.NotEmpty(accountId, nameof(accountId));
        AccountId = accountId;

        if (amount <= 0)
            throw new DomainException(
                new DomainError(AccountingDomainErrorCodes.InvalidAmount,
                AccountingDomainErrorCodes.InvalidAmount.ToString()));
        Amount = amount;

        Check.NotEmpty(referenceNumber, nameof(referenceNumber));
        ReferenceNumber = referenceNumber;

        Status = DepositRequestStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public static DepositRequest Create(Guid accountId, decimal amount, string referenceNumber)
    {
        var depositRequest = new DepositRequest(accountId, amount, referenceNumber);
        depositRequest.AddDomainEvent(
            new DepositRequestedDomainEvent(
                depositRequest.Id,
                depositRequest.AccountId,
                depositRequest.Amount,
                depositRequest.ReferenceNumber));
        return depositRequest;
    }

    public void Complete(Guid journalEntryId)
    {
        EnsureNotFinalized();

        Status = DepositRequestStatus.Completed;
        JournalEntryId = journalEntryId;
        LastUpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new DepositRequestedCompletedDomainEvent(this.Id, this.AccountId, journalEntryId));
    }

    public void Reject(string reason)
    {
        EnsureNotFinalized();

        Status = DepositRequestStatus.Rejected;
        RejectReason = reason;
        LastUpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new DepositRequestedRejectedDomainEvent(this.Id, reason));
    }

    private void EnsureNotFinalized()
    {
        if (Status == DepositRequestStatus.Completed || Status == DepositRequestStatus.Rejected)
        {
            throw new DomainException(
                new DomainError(AccountingDomainErrorCodes.DepositRequestAlreadyFinalized,
                AccountingDomainErrorCodes.DepositRequestAlreadyFinalized.ToString()));
        }
    }



}
