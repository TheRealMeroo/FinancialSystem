using Accounting.Domain;
using Accounting.Domain.Aggregates;
using Accounting.Domain.Enums;
using Accounting.Domain.Errors;
using Accounting.Domain.Events.Accounts;
using BuildingBlocks.Domain.Exceptions;
namespace UnitTests.Accounting.Domain;

public class AccountTests
{
    [Fact]
    public void Create_WithValidData_ShouldInitializeCorrectlyAndRaiseEvent()
    {
        // Arrange
        var identityId = Guid.NewGuid();
        var type = AccountType.Customer;

        // Act
        var account = Account.Create(identityId, type);

        // Assert
        Assert.Equal(identityId, account.IdentityId);
        Assert.Equal<AccountType>(type, account.Type);
        Assert.Equal(AccountStatus.Deactive, account.Status);

        Assert.Single(account.DomainEvents);
        Assert.IsType<AccountCreatedDomainEvent>(account.DomainEvents.First());
    }

    [Fact]
    public void Activeate_ShouldSetAccountStatusToActive()
    {
        // Arrange
        var identityId = Guid.NewGuid();
        var type = AccountType.Customer;
        var account = Account.Create(identityId, type);

        // Act
        account.Activate();

        // Assert
        Assert.Equal(AccountStatus.Active, account.Status);
        Assert.Equal(2, account.DomainEvents.Count);
        Assert.Contains(account.DomainEvents, e => e is AccountActivatedDomainEvent);
    }

    [Fact]
    public void Deactiveate_ShouldSetAccountStatusToDeactive()
    {
        // Arrange
        var identityId = Guid.NewGuid();
        var type = AccountType.Customer;
        var account = Account.Create(identityId, type);
        account.Activate();

        // Act
        account.Deactivate();

        // Assert
        Assert.Equal(AccountStatus.Deactive, account.Status);
        Assert.Equal(3, account.DomainEvents.Count);
        Assert.Contains(account.DomainEvents, e => e is AccountDeactivatedDomainEvent);
    }

    [Fact]
    public void EnsureActive_WhenAccountIsDeactive_ShouldThrowDomainException()
    {
        // Arrange
        var identityId = Guid.NewGuid();
        var type = AccountType.Customer;
        var account = Account.Create(identityId, type);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => account.EnsureActive());

        Assert.Equal(AccountingDomainErrorCodes.InactiveAccount, exception.Errors.First().Code);
    }

    [Fact]
    public void EnsureActive_WhenAccountIsActive_ShouldNotThrowDomainException()
    {
        // Arrange
        var identityId = Guid.NewGuid();
        var type = AccountType.Customer;
        var account = Account.Create(identityId, type);
        account.Activate();

        // Act
        var exception = Record.Exception(() => account.EnsureActive());

        // Assert
        Assert.Null(exception);
    }

}
