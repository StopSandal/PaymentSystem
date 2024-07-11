using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using PaymentSystem.IntegrationTests.Helpers;
using System.Net;

namespace PaymentSystem.IntegrationTests
{
    public class ReturnPaymentIntegrationTests : IClassFixture<PaymentSystemApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly PaymentSystemApplicationFactory<Program> _factory;

        public ReturnPaymentIntegrationTests(PaymentSystemApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }
        [Fact]
        public async Task POST_ReturnPayment_When_TransactionExists_And_Right()
        {
            // Arrange
            var transactionId = TestConstants.StatusConfirmedTransactionId;
            var requestUri = $"{TestConstants.ReturnPaymentPath\}?transactionId={transactionId}";

            // Act
            var response = await _client.PostAsync(requestUri, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task POST_ReturnPayment_When_TransactionExists_But_Already_Returned()
        {
            // Arrange
            var transactionId = TestConstants.StatusReturnedTransactionId;
            var requestUri = $"{TestConstants.ReturnPaymentPath\}?transactionId={transactionId}";

            // Act
            var response = await _client.PostAsync(requestUri, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task POST_CancelPayment_When_TransactionExists_But_Not_Confirmed()
        {
            // Arrange
            var transactionId = TestConstants.StatusPendingTransactionId;
            var requestUri = $"{TestConstants.ReturnPaymentPath\}?transactionId={transactionId}";

            // Act
            var response = await _client.PostAsync(requestUri, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task POST_CancelPayment_When_TransactionNotExists()
        {
            // Arrange
            var transactionId = TestConstants.NotExistingTransactionId;
            var requestUri = $"{TestConstants.ReturnPaymentPath\}?transactionId={transactionId}";

            // Act
            var response = await _client.PostAsync(requestUri, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
