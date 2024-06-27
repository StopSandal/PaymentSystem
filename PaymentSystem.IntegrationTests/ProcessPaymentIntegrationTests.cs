using Azure.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using PaymentSystem.DataLayer.EntitiesDTO.Payment;
using PaymentSystem.IntegrationTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
                CardId = TestConstants.NO_LIMIT_CARD_ID,  // Using the seeded card ID
                TotalAmount = TestConstants.RIGHT_MONEY_AMOUNT,
                Currency = TestConstants.RIGHT_CARD_CURRENCY
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, TestConstants.MEDIA_TYPE);

            // Act
            var response = await _client.PostAsync(TestConstants.PROCESS_PAYMENT_PATH, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task POST_ProcessPayment_When_CardExists_NotEnoughMoneyOnCard_RightCurrency()
        {
            // Arrange
            var request = new ProcessPaymentDTO
            {
                CardId = TestConstants.NO_MONEY_CARD_ID,  // Using the seeded card ID
                TotalAmount = TestConstants.RIGHT_MONEY_AMOUNT,
                Currency = TestConstants.RIGHT_CARD_CURRENCY
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, TestConstants.MEDIA_TYPE);

            // Act
            var response = await _client.PostAsync(TestConstants.PROCESS_PAYMENT_PATH, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task POST_ProcessPayment_When_CardExists_EnoughAmount_WrongCurrency()
        {
            // Arrange
            var request = new ProcessPaymentDTO
            {
                CardId = TestConstants.NO_LIMIT_CARD_ID,  // Using the seeded card ID
                TotalAmount = TestConstants.RIGHT_MONEY_AMOUNT,
                Currency = TestConstants.WRONG_CARD_CURRENCY
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, TestConstants.MEDIA_TYPE);

            // Act
            var response = await _client.PostAsync(TestConstants.PROCESS_PAYMENT_PATH, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task POST_ProcessPayment_When_CardNotExists()
        {
            // Arrange
            var request = new ProcessPaymentDTO
            {
                CardId = TestConstants.NOT_EXISTING_CARD_ID,
                TotalAmount = TestConstants.RIGHT_MONEY_AMOUNT,
                Currency = TestConstants.RIGHT_CARD_CURRENCY
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, TestConstants.MEDIA_TYPE);

            // Act
            var response = await _client.PostAsync(TestConstants.PROCESS_PAYMENT_PATH, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
