namespace PaymentSystem.IntegrationTests.Helpers
{
    public class TestConstants
    {
        internal const string MEDIA_TYPE = "application/json";

        internal const string API_PATH = "/api/payment";
        internal const string PROCESS_PAYMENT_PATH = $"{API_PATH}/process";
        internal const string CANCEL_PAYMENT_PATH = $"{API_PATH}/cancel";
        internal const string CONFIRM_PAYMENT_PATH = $"{API_PATH}/confirm";
        internal const string RETURN_PAYMENT_PATH = $"{API_PATH}/return";

        internal const string RIGHT_CARD_CURRENCY = "USD";
        internal const string WRONG_CARD_CURRENCY = "EUR";

        internal const decimal RIGHT_MONEY_AMOUNT = 100;

        internal const long NO_LIMIT_CARD_ID = 1L;
        internal const long NO_MONEY_CARD_ID = 2L;
        internal const long NOT_EXISTING_CARD_ID = 0L;

        internal const long STATUS_PENDING_TRANSACTION_ID = 1L;
        internal const long EXPIRED_TRANSACTION_ID = 2L;
        internal const long NOT_ENOUGH_MONEY_TRANSACTION_ID = 3L;
        internal const long STATUS_CANCELED_TRANSACTION_ID = 4L;
        internal const long STATUS_CONFIRMED_TRANSACTION_ID = 5L;
        internal const long STATUS_RETURNED_TRANSACTION_ID = 6L;
        internal const long NOT_EXISTING_TRANSACTION_ID = 0L;

        internal const string RIGHT_CONFIRMATION_CODE = "123456";
        internal const string WRONG_CONFIRMATION_CODE = "654321";

        internal const string TRANSACTION_STATUS_PENDING = "Pending";
        internal const string TRANSACTION_STATUS_CANCELED = "Canceled";
        internal const string TRANSACTION_STATUS_CONFIRMED = "Confirmed";
        internal const string TRANSACTION_STATUS_RETUNED = "Returned";
    }
}
