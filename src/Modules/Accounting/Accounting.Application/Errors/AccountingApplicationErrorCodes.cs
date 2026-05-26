namespace Accounting.Application.Errors
{
    public static class AccountingApplicationErrorCodes
    {
        public const string DuplicateDepositRequestReferenceNumber
            = "There is a request with this reference number.";

        public const string AccountNotFound
            = "There is no account with this information.";

        public const string CannotProcessDepositRequest
            = "Can not process deposit request.";


        #region Validation

        public const string IdentityIdRequired
            = "Identity id is required.";

        public const string ReferenceNumberRequired
            = "Reference number is required.";

        public const string ReferenceNumberMaximumSize
            = "Reference number maximum size is 50.";

        public const string AmountMustBeGreaterThanZero
            = "Amount must be greater than zero";

        #endregion
    }
}
