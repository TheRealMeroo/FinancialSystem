using Accounting.Application.Commands.Deposit;
using Accounting.Application.Errors;
using Accounting.Domain.Aggregates;
using Accounting.Domain.Enums;
using Accounting.Domain.Errors;
using Accounting.Domain.Repositories;
using BuildingBlocks.Application.Abstractions.Persistance;
using BuildingBlocks.Application.DTOs;
using BuildingBlocks.Domain.Exceptions;
using Accounting.Domain.Aggregates;
using Accounting.Domain.Enums;
using Accounting.Domain.Repositories;
using BuildingBlocks.Application.Abstractions.Persistance;
using BuildingBlocks.Application.DTOs;
using Moq;

namespace UnitTests.Accounting.Application.Commands;

public class CreateDepositRequestCommandHandlerTests
{
    private readonly Mock<IIdempotencyRepository> _idempotencyRepositoryMock;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IDepositRequestRepository> _depositRequestRepositoryMock;
    private readonly Mock<IJournalEntryRepository> _journalEntryRepositoryMock;

    private readonly CreateDepositRequestCommandHandler _handler;

    public CreateDepositRequestCommandHandlerTests()
    {
        _idempotencyRepositoryMock = new Mock<IIdempotencyRepository>(MockBehavior.Strict);
        _accountRepositoryMock = new Mock<IAccountRepository>(MockBehavior.Strict);
        _unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
        _depositRequestRepositoryMock = new Mock<IDepositRequestRepository>(MockBehavior.Strict);
        _journalEntryRepositoryMock = new Mock<IJournalEntryRepository>(MockBehavior.Strict);

        _handler = new CreateDepositRequestCommandHandler(
            _idempotencyRepositoryMock.Object,
            _accountRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _depositRequestRepositoryMock.Object,
            _journalEntryRepositoryMock.Object);

    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenAllInputsAreValid()
    {
        // Arrange
        var command = new CreateDepositRequestCommand(
            identityId: Guid.NewGuid(),
            amount: 1000,
            referenceNumber: "Ref0001",
            desciption: "New Deposit");

        _idempotencyRepositoryMock.Setup(x =>
            x.GetAsync(command.ReferenceNumber, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IdempotentResponse)null);

        var userAccount = Account.Create(identityId: command.IdentityId, type: AccountType.Customer);
        userAccount.Activate();
        _accountRepositoryMock.Setup(x => x.GetByIdentityIdAsync(command.IdentityId))
            .ReturnsAsync(userAccount);

        var bankAccount = Account.Create(identityId: Guid.NewGuid(), type: AccountType.Bank);
        _accountRepositoryMock.Setup(x => x.GetByAccountTypeAsync(bankAccount.Type))
            .ReturnsAsync(bankAccount);

        _depositRequestRepositoryMock.Setup(x => x.AddAsync(It.IsAny<DepositRequest>()))
            .ReturnsAsync((DepositRequest depositRequest) => depositRequest);

        _journalEntryRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<JournalEntry>()))
            .Returns(Task.CompletedTask);

        _idempotencyRepositoryMock
            .Setup(x => x.SaveAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(It.IsAny<int>);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnExistingResult_WhenRequestIsDuplicate()
    {
        // Arrange
        var command = new CreateDepositRequestCommand(
            identityId: Guid.NewGuid(),
            amount: 1000,
            referenceNumber: "Ref0001",
            desciption: "New Deposit");

        _idempotencyRepositoryMock.Setup(x =>
            x.GetAsync(command.ReferenceNumber, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IdempotentResponse(command.IdentityId.ToString()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Arrange
        Assert.True(result.IsSuccess);

        _accountRepositoryMock
            .Verify(x => x.GetByIdentityIdAsync(command.IdentityId), Times.Never);

        _accountRepositoryMock
            .Verify(x => x.GetByAccountTypeAsync(AccountType.Bank), Times.Never);

        _depositRequestRepositoryMock
            .Verify(x => x.AddAsync(It.IsAny<DepositRequest>()), Times.Never);

        _journalEntryRepositoryMock
            .Verify(x => x.AddAsync(It.IsAny<JournalEntry>()), Times.Never);

        _idempotencyRepositoryMock
            .Verify(x => x.SaveAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>())
            , Times.Never);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenCustomerAccountNotFound()
    {
        // Arrange
        var command = new CreateDepositRequestCommand(
            identityId: Guid.NewGuid(),
            amount: 1000,
            referenceNumber: "Ref0001",
            desciption: "New Deposit");

        _idempotencyRepositoryMock.Setup(x =>
            x.GetAsync(command.ReferenceNumber, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IdempotentResponse?)null);

        _accountRepositoryMock
            .Setup(x => x.GetByIdentityIdAsync(command.IdentityId))
            .ReturnsAsync((Account?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Arrange
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        Assert.Equal(AccountingApplicationErrorCodes.AccountNotFound, result.Errors.First().Code);

        _accountRepositoryMock
            .Verify(x => x.GetByAccountTypeAsync(AccountType.Bank), Times.Never);

        _depositRequestRepositoryMock
            .Verify(x => x.AddAsync(It.IsAny<DepositRequest>()), Times.Never);

        _journalEntryRepositoryMock
            .Verify(x => x.AddAsync(It.IsAny<JournalEntry>()), Times.Never);

        _idempotencyRepositoryMock
            .Verify(x => x.SaveAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>())
            , Times.Never);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenCustomerAccountIsNotActive()
    {
        // Arrange
        var command = new CreateDepositRequestCommand(
            identityId: Guid.NewGuid(),
            amount: 1000,
            referenceNumber: "Ref0001",
            desciption: "New Deposit");

        _idempotencyRepositoryMock
            .Setup(x => x.GetAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((IdempotentResponse?)null);

        var userAccount = Account.Create(identityId: command.IdentityId, type: AccountType.Customer);

        _accountRepositoryMock
            .Setup(x => x.GetByIdentityIdAsync(command.IdentityId))
            .ReturnsAsync(userAccount);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(async ()
            => await _handler.Handle(command, CancellationToken.None));

        Assert.NotNull(exception.Errors);
        Assert.Equal(AccountingDomainErrorCodes.InactiveAccount,
            exception.Errors.First().Code);
        Assert.Equal(AccountingDomainErrorCodes.InactiveAccount.ToString(),
            exception.Errors.First().Message);

        _accountRepositoryMock
            .Verify(x => x.GetByAccountTypeAsync(AccountType.Bank), Times.Never);

        _depositRequestRepositoryMock
            .Verify(x => x.AddAsync(It.IsAny<DepositRequest>()), Times.Never);

        _journalEntryRepositoryMock
            .Verify(x => x.AddAsync(It.IsAny<JournalEntry>()), Times.Never);

        _idempotencyRepositoryMock
            .Verify(x => x.SaveAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>())
            , Times.Never);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenBankAccountNotFound()
    {
        // Arrange
        var command = new CreateDepositRequestCommand(
            identityId: Guid.NewGuid(),
            amount: 1000,
            referenceNumber: "Ref0001",
            desciption: "New Deposit");

        _idempotencyRepositoryMock
            .Setup(x => x.GetAsync(
                command.ReferenceNumber,
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((IdempotentResponse?)null);

        var userAccount = Account.Create(identityId: command.IdentityId, type: AccountType.Customer);
        userAccount.Activate();

        _accountRepositoryMock
            .Setup(x => x.GetByIdentityIdAsync(command.IdentityId))
            .ReturnsAsync(userAccount);

        _accountRepositoryMock
            .Setup(x => x.GetByAccountTypeAsync(AccountType.Bank))
            .ReturnsAsync((Account?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        Assert.Equal(AccountingApplicationErrorCodes.AccountNotFound,
            result.Errors.First().Code);
        Assert.Equal(AccountingApplicationErrorCodes.AccountNotFound.ToString(),
            result.Errors.First().Message);

        _accountRepositoryMock
            .Verify(x => x.GetByIdentityIdAsync(command.IdentityId), Times.Once);
        _accountRepositoryMock
            .Verify(x => x.GetByAccountTypeAsync(AccountType.Bank), Times.Once);
        _depositRequestRepositoryMock
            .Verify(x => x.AddAsync(It.IsAny<DepositRequest>()), Times.Never);
        _journalEntryRepositoryMock
            .Verify(x => x.AddAsync(It.IsAny<JournalEntry>()), Times.Never);
        _idempotencyRepositoryMock
            .Verify(x => x.SaveAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock
            .Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

}
