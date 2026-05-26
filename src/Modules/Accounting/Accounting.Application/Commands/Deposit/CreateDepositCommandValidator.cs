using Accounting.Application.Errors;
using FluentValidation;

namespace Accounting.Application.Commands.Deposit;

public class CreateDepositCommandValidator : AbstractValidator<CreateDepositRequestCommand>
{
    public CreateDepositCommandValidator()
    {
        RuleFor(v => v.IdentityId)
            .NotEmpty()
            .WithMessage(AccountingApplicationErrorCodes.IdentityIdRequired.ToString());

        RuleFor(v => v.Amount)
            .GreaterThan(0)
            .WithMessage(AccountingApplicationErrorCodes.AmountMustBeGreaterThanZero.ToString());

        RuleFor(v => v.ReferenceNumber)
            .NotEmpty()
            .WithMessage(AccountingApplicationErrorCodes.ReferenceNumberRequired.ToString())
            .MaximumLength(50)
            .WithMessage(AccountingApplicationErrorCodes.ReferenceNumberMaximumSize.ToString());
    }
}
