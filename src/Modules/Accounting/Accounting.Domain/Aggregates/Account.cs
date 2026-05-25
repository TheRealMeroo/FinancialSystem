using Accounting.Domain.Enums;
using Accounting.Domain.Errors;
using Accounting.Domain.Events.Accounts;
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Exceptions;

namespace Accounting.Domain.Aggregates;

public class Account : AggregateRoot
{
    public Guid IdentityId { get; private set; }
    public AccountType Type { get; private set; }
    public AccountStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Account() { }

    private Account(Guid identityId, AccountType type)
    {
        IdentityId = identityId;
        Type = type;
        Status = AccountStatus.Deactive;
        CreatedAt = DateTime.UtcNow;
    }

    public static Account Create(Guid identityId, AccountType type)
    {
        var account = new Account(identityId, type);
        account.AddDomainEvent(new AccountCreatedDomainEvent(account.Id, account.IdentityId, account.Type));
        return account;
    }

    public void Activate()
    {
        Status = AccountStatus.Active;
        AddDomainEvent(new AccountActivatedDomainEvent(this.Id));
    }

    public void Deactivate()
    {
        Status = AccountStatus.Deactive;
        AddDomainEvent(new AccountDeactivatedDomainEvent(this.Id));
    }

    public void EnsureActive()
    {
        if (Status != AccountStatus.Active)
            throw new DomainException(
                new DomainError(
                    AccountingDomainErrorCodes.InactiveAccount, AccountingDomainErrorCodes.InactiveAccount.ToString()));
    }
}
