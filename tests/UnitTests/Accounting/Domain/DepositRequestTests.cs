using Accounting.Domain.Aggregates;
using Accounting.Domain.Enums;
using Accounting.Domain.Errors;
using Accounting.Domain.Events.DepositRequest;
using BuildingBlocks.Domain.Exceptions;

namespace UnitTests.Accounting.Domain
{
    public class DepositRequestTests
    {
        [Fact]
        public void Create_WithValidInputs_ShouldCreateDepositRequestAndRaiseDomainEvent()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            decimal amount = 10;
            string referenceNumber = "Ref0001";

            // Act
            var depositRequest = DepositRequest.Create(
                accountId, amount, referenceNumber);

            // Assert
            Assert.Equal(accountId, depositRequest.AccountId);
            Assert.Equal(amount, depositRequest.Amount);
            Assert.Equal(referenceNumber, depositRequest.ReferenceNumber);
            Assert.Equal(DepositRequestStatus.Pending, depositRequest.Status);

            Assert.Single(depositRequest.DomainEvents);
            Assert.IsType<DepositRequestedDomainEvent>(depositRequest.DomainEvents.First());
        }

        [Fact]
        public void Create_WithEmptyAccountId_ShouldThrowException()
        {
            // Arrange
            var accountId = Guid.Empty;
            decimal amount = 10;
            string referenceNumber = "Ref0001";

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() =>
            DepositRequest.Create(accountId, amount, referenceNumber));

            Assert.Equal(BuildingBlocksErrorCodes.EmptyGuid, exception.Errors.First().Code);
        }

        [Fact]
        public void Create_WithInvalidAmount_ShouldThrowException()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            decimal amount = 0;
            string referenceNumber = "Ref0001";

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() =>
            DepositRequest.Create(accountId, amount, referenceNumber));

            Assert.Equal(AccountingDomainErrorCodes.InvalidAmount, exception.Errors.First().Code);
        }

        [Fact]
        public void Create_WithEmptyReferenceNumber_ShouldThrowException()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            decimal amount = 10;
            string referenceNumber = string.Empty;

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() =>
            DepositRequest.Create(accountId, amount, referenceNumber));

            Assert.Equal(BuildingBlocksErrorCodes.EmptyString, exception.Errors.First().Code);
        }

        [Fact]
        public void Complete_WhenDepositRequestNotFinalized_ShouldSetDepositRequestStatusToCompleteAndRaiseDomainEvent()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            decimal amount = 10;
            string referenceNumber = "Ref0001";
            var journalEntryId = Guid.NewGuid();

            // Act
            var depositRequest = DepositRequest.Create(
                accountId, amount, referenceNumber);
            depositRequest.Complete(journalEntryId);

            // Assert
            Assert.Equal(DepositRequestStatus.Completed, depositRequest.Status);
            Assert.Equal(journalEntryId, depositRequest.JournalEntryId);
            Assert.Equal(2, depositRequest.DomainEvents.Count);
            Assert.Contains(depositRequest.DomainEvents,
                e => e is DepositRequestedCompletedDomainEvent);
        }

        [Theory]
        [InlineData(DepositRequestStatus.Completed)]
        [InlineData(DepositRequestStatus.Rejected)]
        public void CompleteAndReject_WhenAlreadyFinalized_ShouldThrowDomainException(DepositRequestStatus initialStatus)
        {
            // Arrange
            var accountId = Guid.NewGuid();
            decimal amount = 10;
            string referenceNumber = "Ref0001";
            var journalEntryId = Guid.NewGuid();
            var depositRequest = DepositRequest.Create(accountId, amount, referenceNumber);
            var reason = "Some Reason";

            if (initialStatus == DepositRequestStatus.Completed)
                depositRequest.Complete(journalEntryId);
            else
                depositRequest.Reject(reason);

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => depositRequest.Complete(journalEntryId));
            Assert.Equal(AccountingDomainErrorCodes.DepositRequestAlreadyFinalized, exception.Errors.First().Code);
        }

        [Fact]
        public void Complete_WithEmptyJournalEntryId_ShouldThrowException()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            decimal amount = 10;
            string referenceNumber = "Ref0001";
            var journalEntryId = Guid.Empty;

            // Act & Assert
            var depositRequest = DepositRequest.Create(
                accountId, amount, referenceNumber);

            var exception = Assert.Throws<DomainException>(() =>
            depositRequest.Complete(journalEntryId));

            Assert.Equal(BuildingBlocksErrorCodes.EmptyGuid, exception.Errors.First().Code);
        }

        [Fact]
        public void Reject_WhenDepositRequestNotFinalized_ShouldSetDepositRequestStatusToRejectedAndRaiseDomainEvent()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            decimal amount = 10;
            string referenceNumber = "Ref0001";
            var reason = "Some Reason";

            // Act
            var depositRequest = DepositRequest.Create(
                accountId, amount, referenceNumber);
            depositRequest.Reject(reason);

            // Assert
            Assert.Equal(DepositRequestStatus.Rejected, depositRequest.Status);
            Assert.Equal(reason, depositRequest.RejectReason);
            Assert.Equal(2, depositRequest.DomainEvents.Count);
            Assert.Contains(depositRequest.DomainEvents,
                e => e is DepositRequestedRejectedDomainEvent);
        }

        [Theory]
        [InlineData(DepositRequestStatus.Completed)]
        [InlineData(DepositRequestStatus.Rejected)]
        public void Reject_WhenAlreadyFinalized_ShouldThrowDomainException(DepositRequestStatus initialStatus)
        {
            // Arrange
            var accountId = Guid.NewGuid();
            decimal amount = 10;
            string referenceNumber = "Ref0001";
            var journalEntryId = Guid.NewGuid();
            var depositRequest = DepositRequest.Create(accountId, amount, referenceNumber);
            var reason = "Some Reason";

            if (initialStatus == DepositRequestStatus.Completed)
                depositRequest.Complete(journalEntryId);
            else
                depositRequest.Reject(reason);


            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => depositRequest.Reject(reason));
            Assert.Equal(AccountingDomainErrorCodes.DepositRequestAlreadyFinalized, exception.Errors.First().Code);
        }

        [Fact]
        public void Reject_WithEmptyReason_ShouldThrowException()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            decimal amount = 10;
            string referenceNumber = "Ref0001";
            var reason = string.Empty;

            // Act & Assert
            var depositRequest = DepositRequest.Create(
                accountId, amount, referenceNumber);

            var exception = Assert.Throws<DomainException>(() =>
            depositRequest.Reject(reason));

            Assert.Equal(BuildingBlocksErrorCodes.EmptyString,
                exception.Errors.First().Code);
        }
    }
}
