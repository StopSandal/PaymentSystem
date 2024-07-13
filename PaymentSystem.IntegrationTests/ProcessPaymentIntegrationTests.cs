using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using PaymentSystem.DataLayer.EntitiesDTO.Payment;
using PaymentSystem.IntegrationTests.Helpers;
using System.Net;
using System.Text;

namespace PaymentSystem.IntegrationTests
{
    public class ProcessPaymentIntegrationTests : IClassFixture<PaymentSystemApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly PaymentSystemApplicationFactory<Program> _factory;

        public ProcessPaymentIntegrationTests(PaymentSystemApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }
        [Fact]
        public async Task POST_ProcessPayment_When_CardExists_EnoughAmount_RightCurrency()
        {
            // Arrange
            var request = new ProcessPaymentDTO
            {
                CardId = TestConstants.NoLimitCardId,  // Using the seeded card ID
                TotalAmount = TestConstants.RightMoneyAmount,
                Currency = TestConstants.RightCardCurrency
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, TestConstants.MediaType);

            // Act
            var response = await _client.PostAsync(TestConstants.ProcessPaymentPath, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task POST_ProcessPayment_When_CardExists_NotEnoughMoneyOnCard_RightCurrency()
        {
            // Arrange
            var request = new ProcessPaymentDTO
            {
                CardId = TestConstants.NoMoneyCardId,  // Using the seeded card ID
                TotalAmount = TestConstants.RightMoneyAmount,
                Currency = TestConstants.RightCardCurrency
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, TestConstants.MediaType);

            // Act
            var response = await _client.PostAsync(TestConstants.ProcessPaymentPath, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task POST_ProcessPayment_When_CardExists_EnoughAmount_WrongCurrency()
        {
            // Arrange
            var request = new ProcessPaymentDTO
            {
                CardId = TestConstants.NoLimitCardId,  // Using the seeded card ID
                TotalAmount = TestConstants.RightMoneyAmount,
                Currency = TestConstants.WrongCardCurrency
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, TestConstants.MediaType);

            // Act
            var response = await _client.PostAsync(TestConstants.ProcessPaymentPath, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task POST_ProcessPayment_When_CardNotExists()
        {
            // Arrange
            var request = new ProcessPaymentDTO
            {
                CardId = TestConstants.NotExistingCardId,
                TotalAmount = TestConstants.RightMoneyAmount,
                Currency = TestConstants.RightCardCurrency
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, TestConstants.MediaType);

            // Act
            var response = await _client.PostAsync(TestConstants.ProcessPaymentPath, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
