

namespace PaymentSystem.UnitTest.Helpers
{
    internal class UnitTestConstants
    {
        internal const long NotExistingCardId = 0L;
        internal const long ExistingCardId = 1L;
        internal const long OtherExistingCardId = 2L;

        internal const string RightCardNumber = "1234567890123456";
        internal const string OtherRightCardNumber = "6543210987654321";
        internal const string WrongCardNumber = "6543210987654321";


        internal const string RightCurrencyType = "USD";
        internal const string OtherCurrencyType = "EUR";
        internal const int RightCVV = 111;

        internal const decimal InvalidAmount = -100;
        internal const decimal OneHundredAmount = 100;
        internal const decimal ThreeHundredAmount = 300;

        internal const long NotExistingTransactionId = 0L;
        internal const long ExistingTransactionId = 1L;
        internal const long OtherExistingTransactionId = 2L;

        internal const string RightConfirmationCode = "123456";
        internal const string InvalidConfirmationCode = "654321";

        internal const string ConfirmationCodeExpirationMinutes = "Confirmation:ConfirmationCodeValidityInMinutes";
        internal const int ConfirmationCodeExpirationMinutesInNumber = 5;

        internal const string CardNotFoundExceptionMessage = "Card not found.";
        internal const string CurrencyMismatchExceptionMessage = "Currency mismatch.";
        internal const string InsufficientFundsExceptionMessage = "Insufficient funds.";
        internal const string TransactionNotConfirmedExceptionMessage = "Transaction not confirmed.";
        internal const string TransactionNotCanceledExceptionMessage = "Transaction not confirmed.";
    }
}
