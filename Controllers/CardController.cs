﻿using Microsoft.AspNetCore.Mvc;
using PaymentSystem.DataLayer.EntitiesDTO;
using PaymentSystem.DataLayer.EntitiesDTO.Card;
using PaymentSystem.Services.Interfaces;

namespace PaymentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;
        private readonly ILogger<CardController> _logger;

        public CardController(ICardService cardService, ILogger<CardController> logger)
        {
            _cardService = cardService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves card by id
        /// </summary>
        /// <param name="id">Card id</param>
        /// <returns>Info about asked card</returns>
        /// <response code="200">Успешное выполнение</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCardAsync(long id)
        {
            try
            {
                var card = await _cardService.GetCardAsync(id);
                if (card == null)
                {
                    return NotFound();
                }
                return Ok(card);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting card with ID {CardId}", id);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("byCardNumber/{cardNumber}")]
        public async Task<IActionResult> GetCardByCardNumberAsync(string cardNumber)
        {
            try
            {
                var card = await _cardService.GetCardByCardNumberAsync(cardNumber);
                if (card == null)
                {
                    return NotFound();
                }
                return Ok(card);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting card with number {CardNumber}", cardNumber);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCardsAsync()
        {
            try
            {
                var cards = await _cardService.GetCardsAsync();
                return Ok(cards);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cards");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCardAsync([FromBody] AddCardDTO newCard)
        {
            try
            {
                await _cardService.CreateCardAsync(newCard);
                return Created();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating card");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCardAsync(long id, [FromBody] EditCardDTO editCard)
        {
            try
            {
                await _cardService.UpdateCardAsync(id, editCard);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating card with ID {CardId}", id);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCardAsync(long id)
        {
            try
            {
                await _cardService.DeleteCardAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting card with ID {CardId}", id);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("increaseBalance")]
        public async Task<IActionResult> IncreaseBalanceAsync([FromBody] BalanceUpdateDTO balanceUpdate)
        {
            try
            {
                await _cardService.IncreaseBalanceAsync(balanceUpdate.CardId, balanceUpdate.Amount);
                return NoContent();
            }
            catch (InvalidDataException ex)
            {
                _logger.LogError(ex, "Error increasing balance for card ID {CardId}", balanceUpdate.CardId);
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error increasing balance for card ID {CardId}", balanceUpdate.CardId);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("decreaseBalance")]
        public async Task<IActionResult> DecreaseBalanceAsync([FromBody] BalanceUpdateDTO balanceUpdate)
        {
            try
            {
                await _cardService.DecreaseBalanceAsync(balanceUpdate.CardId, balanceUpdate.Amount);
                return NoContent();
            }
            catch (InvalidDataException ex)
            {
                _logger.LogError(ex, "Error decreasing balance for card ID {CardId}", balanceUpdate.CardId);
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error decreasing balance for card ID {CardId}", balanceUpdate.CardId);
                return BadRequest(ex.Message);
            }
        }
    }
}
