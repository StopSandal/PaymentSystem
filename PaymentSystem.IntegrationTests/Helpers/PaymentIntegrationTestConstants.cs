using PaymentSystem.Services.Helpers.Constants;

namespace PaymentSystem.IntegrationTests.Helpers
{
    public class TestConstants
    {
        internal const string MediaType = "application/json";

        internal const string ApiPath = "/api/payment";
        internal const string ProcessPaymentPath = $"{ApiPath}/process";
        internal const string CancelPaymentPath = $"{ApiPath}/cancel";
        internal const string ConfirmPaymentPath = $"{ApiPath}/confirm";
        internal const string ReturnPaymentPath = $"{ApiPath}/return";

        internal const string RightCardCurrency = "USD";
        internal const string WrongCardCurrency = "EUR";

        internal const decimal RightMoneyAmount = 100;

        internal const long NoLimitCardId = 1L;
        internal const long NoMoneyCardId = 2L;
        internal const long NotExistingCardId = 0L;

        internal const long StatusPendingTransactionId = 1L;
        internal const long ExpiredTransactionId = 2L;
        internal const long NotEnoughMoneyTransactionId = 3L;
        internal const long StatusCanceledTransactionId = 4L;
        internal const long StatusConfirmedTransactionId = 5L;
        internal const long StatusReturnedTransactionId = 6L;
        internal const long NotExistingTransactionId = 0L;

        internal const string RightConfirmationCode = "123456";
        internal const string WrongConfirmationCode = "654321";

        internal const string TransactionStatusPending = TransactionsConstants.TransactionStatusPending;
        internal const string TransactionStatusCanceled = TransactionsConstants.TransactionStatusCanceled;
        internal const string TransactionStatusConfirmed = TransactionsConstants.TransactionStatusConfirmed;
        internal const string TransactionStatusReturned = TransactionsConstants.TransactionStatusReturned;
    }
}
