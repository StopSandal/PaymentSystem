using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using PaymentSystem.DataLayer.EntitiesDTO.Payment;
using PaymentSystem.IntegrationTests.Helpers;
using System.Net;
using System.Text;

namespace PaymentSystem.IntegrationTests
{
    public class ConfirmPaymentIntegrationTests : IClassFixture<PaymentSystemApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly PaymentSystemApplicationFactory<Program> _factory;


        public ConfirmPaymentIntegrationTests(PaymentSystemApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task POST_ConfirmPayment_When_TransactionExists_RightConfirmationCode()
        {
            // Arrange
            var request = new ConfirmPaymentDTO
            {
                TransactionId = TestConstants.StatusPendingTransactionId,
                ConfirmationCode = TestConstants.RightConfirmationCode
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, TestConstants.MediaType);

            // Act
            var response = await _client.PostAsync(TestConstants.ConfirmPaymentPath, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task POST_ConfirmPayment_When_TransactionExists_WrongConfirmationCode()
        {
            // Arrange
            var request = new ConfirmPaymentDTO
            {
                TransactionId = TestConstants.StatusPendingTransactionId,
                ConfirmationCode = TestConstants.WrongConfirmationCode
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, TestConstants.MediaType);

            // Act
            var response = await _client.PostAsync(TestConstants.ConfirmPaymentPath, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task POST_ConfirmPayment_When_TransactionExists_But_Expired()
        {
            // Arrange
            var request = new ConfirmPaymentDTO
            {
                TransactionId = TestConstants.ExpiredTransactionId,
                ConfirmationCode = TestConstants.RightConfirmationCode
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, TestConstants.MediaType);

            // Act
            var response = await _client.PostAsync(TestConstants.ConfirmPaymentPath, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task POST_ConfirmPayment_When_TransactionExists_And_Not_Enough_Money_On_Card()
        {
            // Arrange
            var request = new ConfirmPaymentDTO
            {
                TransactionId = TestConstants.NotEnoughMoneyTransactionId,
                ConfirmationCode = TestConstants.RightConfirmationCode
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, TestConstants.MediaType);

            // Act
            var response = await _client.PostAsync(TestConstants.ConfirmPaymentPath, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task POST_ConfirmPayment_When_TransactionExists_And_Already_Canceled()
        {
            // Arrange
            var request = new ConfirmPaymentDTO
            {
                TransactionId = TestConstants.StatusCanceledTransactionId,
                ConfirmationCode = TestConstants.RightConfirmationCode
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, TestConstants.MediaType);

            // Act
            var response = await _client.PostAsync(TestConstants.ConfirmPaymentPath, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task POST_ConfirmPayment_When_TransactionExists_And_Already_Confirmed()
        {
            // Arrange
            var request = new ConfirmPaymentDTO
            {
                TransactionId = TestConstants.StatusConfirmedTransactionId,
                ConfirmationCode = TestConstants.RightConfirmationCode
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, TestConstants.MediaType);

            // Act
            var response = await _client.PostAsync(TestConstants.ConfirmPaymentPath, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task POST_ConfirmPayment_When_TransactionNotExists()
        {
            // Arrange
            var request = new ConfirmPaymentDTO
            {
                TransactionId = TestConstants.NotExistingTransactionId,
                ConfirmationCode = TestConstants.RightConfirmationCode
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, TestConstants.MediaType);

            // Act
            var response = await _client.PostAsync(TestConstants.ConfirmPaymentPath, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

    }
}
