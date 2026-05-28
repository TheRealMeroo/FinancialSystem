using Accounting.Application.Commands.Deposit;
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
}
