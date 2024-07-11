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
                TransactionId = TestConstants.STATUS_PENDING_TRANSACTION_ID,
                ConfirmationCode = TestConstants.RIGHT_CONFIRMATION_CODE
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, TestConstants.MEDIA_TYPE);

            // Act
            var response = await _client.PostAsync(TestConstants.CONFIRM_PAYMENT_PATH, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task POST_ConfirmPayment_When_TransactionExists_WrongConfirmationCode()
        {
            // Arrange
            var request = new ConfirmPaymentDTO
            {
                TransactionId = TestConstants.STATUS_PENDING_TRANSACTION_ID,
                ConfirmationCode = TestConstants.WRONG_CONFIRMATION_CODE
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, TestConstants.MEDIA_TYPE);

            // Act
            var response = await _client.PostAsync(TestConstants.CONFIRM_PAYMENT_PATH, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task POST_ConfirmPayment_When_TransactionExists_But_Expired()
        {
            // Arrange
            var request = new ConfirmPaymentDTO
            {
                TransactionId = TestConstants.EXPIRED_TRANSACTION_ID,
                ConfirmationCode = TestConstants.RIGHT_CONFIRMATION_CODE
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, TestConstants.MEDIA_TYPE);

            // Act
            var response = await _client.PostAsync(TestConstants.CONFIRM_PAYMENT_PATH, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task POST_ConfirmPayment_When_TransactionExists_And_Not_Enough_Money_On_Card()
        {
            // Arrange
            var request = new ConfirmPaymentDTO
            {
                TransactionId = TestConstants.NOT_ENOUGH_MONEY_TRANSACTION_ID,
                ConfirmationCode = TestConstants.RIGHT_CONFIRMATION_CODE
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, TestConstants.MEDIA_TYPE);

            // Act
            var response = await _client.PostAsync(TestConstants.CONFIRM_PAYMENT_PATH, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task POST_ConfirmPayment_When_TransactionExists_And_Already_Canceled()
        {
            // Arrange
            var request = new ConfirmPaymentDTO
            {
                TransactionId = TestConstants.STATUS_CANCELED_TRANSACTION_ID,
                ConfirmationCode = TestConstants.RIGHT_CONFIRMATION_CODE
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, TestConstants.MEDIA_TYPE);

            // Act
            var response = await _client.PostAsync(TestConstants.CONFIRM_PAYMENT_PATH, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task POST_ConfirmPayment_When_TransactionExists_And_Already_Confirmed()
        {
            // Arrange
            var request = new ConfirmPaymentDTO
            {
                TransactionId = TestConstants.STATUS_CONFIRMED_TRANSACTION_ID,
                ConfirmationCode = TestConstants.RIGHT_CONFIRMATION_CODE
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, TestConstants.MEDIA_TYPE);

            // Act
            var response = await _client.PostAsync(TestConstants.CONFIRM_PAYMENT_PATH, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task POST_ConfirmPayment_When_TransactionNotExists()
        {
            // Arrange
            var request = new ConfirmPaymentDTO
            {
                TransactionId = TestConstants.NOT_EXISTING_TRANSACTION_ID,
                ConfirmationCode = TestConstants.RIGHT_CONFIRMATION_CODE
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, TestConstants.MEDIA_TYPE);

            // Act
            var response = await _client.PostAsync(TestConstants.CONFIRM_PAYMENT_PATH, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

    }
}
