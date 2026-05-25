namespace Accounting.Domain.Errors;

public static class AccountingDomainErrorCodes
{
    public const string InvalidStatus = "Invalid_Status";
    public const string AtLeastTwoLinesRequired = "At_Least_Two_Lines_Required";
    public const string InvalidSumOfCreditAndDebit = "Invalid_Sum_Of_Credit_And_Debit";
    public const string InactiveAccount = "Inactive_Account";
    public const string InvalidAmount = "Invalid_Amount";
    public const string DepositRequestAlreadyFinalized = "Deposit_Request_Already_Finalized";

}
