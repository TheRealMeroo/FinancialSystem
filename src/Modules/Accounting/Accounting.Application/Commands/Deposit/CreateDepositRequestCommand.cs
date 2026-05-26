using BuildingBlocks.Application.Abstractions.Commands;

namespace Accounting.Application.Commands.Deposit;

public class CreateDepositRequestCommand : ICommand<Guid>
{
    public Guid IdentityId { get; init; }
    public decimal Amount { get; init; }
    public string ReferenceNumber { get; init; }
    public string? Desciption { get; init; }
}
