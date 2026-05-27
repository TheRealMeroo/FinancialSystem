using Accounting.Domain.Aggregates;
using Accounting.Domain.Enums;
using Accounting.Domain.Errors;
using Accounting.Domain.Events.JournalEntry;
using Accounting.Domain.ValueObjects;
using BuildingBlocks.Domain.Exceptions;

namespace UnitTests.Accounting.Domain;

public class JournalEntryTests
{
    [Fact]
    public void Create_WithValidInputs_ShouldCreateJournalEntryAndRaiseDomainEvent()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var description = "New Entry";

        // Act
        var journalEntry = JournalEntry.Create(correlationId, description);

        // Assert
        Assert.Equal(description, journalEntry.Description);
        Assert.Equal(correlationId, journalEntry.CorrelationId);
        Assert.Equal(JournalEntryStatus.Draft, journalEntry.Status);
        Assert.Single(journalEntry.DomainEvents);
        Assert.IsType<JournalEntryCreatedEvent>(journalEntry.DomainEvents.First());
    }

    [Fact]
    public void Create_WithEmptyCorrelationId_ShouldThrowException()
    {
        // Arrange
        var correlationId = Guid.Empty;
        var description = "New Entry";

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
        JournalEntry.Create(correlationId, description));

        Assert.Equal(BuildingBlocksErrorCodes.EmptyGuid, exception.Errors.First().Code);
    }

    [Fact]
    public void Create_WithEmptyDescription_ShouldThrowException()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var description = string.Empty;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
        JournalEntry.Create(correlationId, description));

        Assert.Equal(BuildingBlocksErrorCodes.EmptyString, exception.Errors.First().Code);
    }

    [Fact]
    public void AddLine_WithValidInputs_ShouldAddLineToJournalEntry()
    {
        // Arrange
        var journalEntry = CreateValidJournalEntry();
        var accountId = Guid.NewGuid();
        var transactionAmount = TransactionAmount.FromDebit(10);
        var lineDescription = "Line description";

        // Act
        journalEntry.AddLine(accountId, transactionAmount, lineDescription);

        // Assert
        Assert.Single(journalEntry.Lines);
    }

    [Fact]
    public void AddLine_WhenJournalEntryStatusIsNotDrafted_ShouldThrowException()
    {
        // Arrange
        var journalEntry = CreateValidJournalEntry();
        var accountId = Guid.NewGuid();
        var debitTransactionAmount = TransactionAmount.FromDebit(10);
        var creditTransactionAmount = TransactionAmount.FromCredit(10);
        var lineDescription = "Line description";
        journalEntry.AddLine(accountId, debitTransactionAmount, lineDescription);
        journalEntry.AddLine(accountId, creditTransactionAmount, lineDescription);
        journalEntry.Post();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
        journalEntry.AddLine(accountId, debitTransactionAmount, lineDescription));

        Assert.Equal(AccountingDomainErrorCodes.InvalidStatus,
            exception.Errors.First().Code);
    }

    [Fact]
    public void Post_WithInsufficientJournalEntryLine_ShouldThrowException()
    {
        // Arrange
        var journalEntry = CreateValidJournalEntry();
        var accountId = Guid.NewGuid();
        var debitTransactionAmount = TransactionAmount.FromDebit(10);
        var lineDescription = "Line description";
        journalEntry.AddLine(accountId, debitTransactionAmount, lineDescription);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
        journalEntry.Post());

        Assert.Equal(AccountingDomainErrorCodes.AtLeastTwoLinesRequired,
            exception.Errors.First().Code);
    }

    [Fact]
    public void Post_WithUnbalanceJournalEntryLines_ShouldThrowException()
    {
        // Arrange
        var journalEntry = CreateValidJournalEntry();
        var accountId = Guid.NewGuid();
        var debitTransactionAmount = TransactionAmount.FromDebit(10);
        var creditTransactionAmount = TransactionAmount.FromCredit(20);
        var lineDescription = "Line description";
        journalEntry.AddLine(accountId, debitTransactionAmount, lineDescription);
        journalEntry.AddLine(accountId, creditTransactionAmount, lineDescription);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
        journalEntry.Post());

        Assert.Equal(AccountingDomainErrorCodes.InvalidSumOfCreditAndDebit,
            exception.Errors.First().Code);
    }

    [Fact]
    public void Post_WithValidInformation_ShouldChangeJournalEntryStatusToPostedAndRaiseEvent()
    {
        // Arrange
        var journalEntry = CreateValidJournalEntry();
        var accountId = Guid.NewGuid();
        var debitTransactionAmount = TransactionAmount.FromDebit(10);
        var creditTransactionAmount = TransactionAmount.FromCredit(10);
        var lineDescription = "Line description";
        journalEntry.AddLine(accountId, debitTransactionAmount, lineDescription);
        journalEntry.AddLine(accountId, creditTransactionAmount, lineDescription);

        // Act
        journalEntry.Post();

        // Assert
        Assert.Equal(JournalEntryStatus.Posted, journalEntry.Status);
        Assert.Equal(2, journalEntry.DomainEvents.Count);
        Assert.Contains(journalEntry.DomainEvents,
            e => e is JournalEntryPostedDomainEvent);
    }

    private JournalEntry CreateValidJournalEntry()
    {
        return JournalEntry.Create(Guid.NewGuid(), "Valid Description");
    }
}
