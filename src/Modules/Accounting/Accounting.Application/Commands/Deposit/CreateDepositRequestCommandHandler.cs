using Accounting.Application.Errors;
using Accounting.Domain.Aggregates;
using Accounting.Domain.Enums;
using Accounting.Domain.Repositories;
using Accounting.Domain.ValueObjects;
using BuildingBlocks.Application.Abstractions.Commands;
using BuildingBlocks.Application.Abstractions.Persistance;
using BuildingBlocks.Application.Results;

namespace Accounting.Application.Commands.Deposit;

public class CreateDepositRequestCommandHandler : ICommandHandler<CreateDepositRequestCommand, Guid>
{
    private readonly IIdempotencyRepository _idempotencyRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDepositRequestRepository _depositRequestRepository;
    private readonly IJournalEntryRepository _journalEntryRepository;

    public CreateDepositRequestCommandHandler(
        IIdempotencyRepository idempotencyRepository,
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork,
        IDepositRequestRepository depositRequestRepository,
        IJournalEntryRepository journalEntryRepository)
    {
        _idempotencyRepository = idempotencyRepository;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _depositRequestRepository = depositRequestRepository;
        _journalEntryRepository = journalEntryRepository;
    }

    public async Task<Result<Guid>> Handle(CreateDepositRequestCommand request, CancellationToken cancellationToken)
    {
        // 1. Pre-Processing
        var idempotentRequest = await _idempotencyRepository
            .GetAsync(request.ReferenceNumber,
                nameof(CreateDepositRequestCommand),
                cancellationToken);

        if (idempotentRequest != null)
            return Result<Guid>.Success(Guid.Parse(idempotentRequest.ResponseData));

        // 2. State Retrival
        var userAccount = await _accountRepository
            .GetByIdentityIdAsync(request.IdentityId);

        if (userAccount == null)
            return Result<Guid>.Failure(
                new ApplicationError(AccountingApplicationErrorCodes.AccountNotFound,
                AccountingApplicationErrorCodes.AccountNotFound.ToString()));

        // 3. Domain Logic Execution
        userAccount.EnsureActive();

        var bankAccount = await _accountRepository
            .GetByAccountTypeAsync(AccountType.Bank);

        if (bankAccount == null)
            return Result<Guid>.Failure(
                new ApplicationError(AccountingApplicationErrorCodes.AccountNotFound,
                AccountingApplicationErrorCodes.AccountNotFound.ToString()));

        var depositRequest = DepositRequest.Create(
            userAccount.Id,
            request.Amount,
            request.ReferenceNumber);

        var journalEntry = JournalEntry.Create(
            correlationId: depositRequest.Id,
            description: request.Desciption == null ? string.Empty : request.Desciption);

        journalEntry.AddLine(
            accountId: bankAccount.Id,
            amount: TransactionAmount.FromDebit(request.Amount),
            description: journalEntry.Description);

        journalEntry.AddLine(
            accountId: userAccount.Id,
            amount: TransactionAmount.FromCredit(request.Amount),
            description: journalEntry.Description);

        // 4. Finalization
        journalEntry.Post();

        depositRequest.Complete(journalEntry.Id);

        // 5. Persistence & Transaction
        var persistedDepositRequest = await _depositRequestRepository.AddAsync(depositRequest);

        await _journalEntryRepository.AddAsync(journalEntry);

        await _idempotencyRepository.SaveAsync(
            request.ReferenceNumber,
            nameof(depositRequest),
            persistedDepositRequest.Id.ToString(),
            cancellationToken);

        await _unitOfWork.SaveChangesAsync();

        return Result<Guid>.Success(persistedDepositRequest!.Id);
    }
}
