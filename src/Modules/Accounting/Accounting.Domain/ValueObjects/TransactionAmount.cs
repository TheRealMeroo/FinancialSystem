namespace Accounting.Domain.ValueObjects;

public record TransactionAmount
{
    public decimal Value { get; init; }
    private TransactionAmount(decimal value) => Value = value;

    public static TransactionAmount FromDebit(decimal amount)
        => new(Math.Abs(amount));

    public static TransactionAmount FromCredit(decimal amount)
        => new(-Math.Abs(amount));

    public bool IsDebit => Value > 0;
    public bool IsCredit => Value < 0;

    public decimal AbsoluteValue => Math.Abs(Value);
}
