using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using PaymentSystem.IntegrationTests.Helpers;
using System.Net;

namespace PaymentSystem.IntegrationTests
{
    public class CancelPaymentIntegrationTests : IClassFixture<PaymentSystemApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly PaymentSystemApplicationFactory<Program> _factory;

        public CancelPaymentIntegrationTests(PaymentSystemApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }
        [Fact]
        public async Task POST_CancelPayment_When_TransactionExists_And_Right()
        {
            // Arrange
            var transactionId = TestConstants.StatusPendingTransactionId;
            var requestUri = $"{TestConstants.CancelPaymentPath}/{transactionId}";

            // Act
            var response = await _client.PostAsync(requestUri, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task POST_CancelPayment_When_TransactionExists_But_Already_Canceled()
        {
            // Arrange
            var transactionId = TestConstants.StatusCanceledTransactionId;
            var requestUri = $"{TestConstants.CancelPaymentPath}/{transactionId}";

            // Act
            var response = await _client.PostAsync(requestUri, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task POST_CancelPayment_When_TransactionExists_But_Already_Confirmed()
        {
            // Arrange
            var transactionId = TestConstants.StatusConfirmedTransactionId;
            var requestUri = $"{TestConstants.CancelPaymentPath}/{transactionId}";

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
            var requestUri = $"{TestConstants.CancelPaymentPath}/{transactionId}";

            // Act
            var response = await _client.PostAsync(requestUri, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
