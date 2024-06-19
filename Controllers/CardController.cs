using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCard(long id)
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
        public async Task<IActionResult> GetCardByCardNumber(string cardNumber)
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
        public async Task<IActionResult> GetCards()
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
        public async Task<IActionResult> CreateCard([FromBody] AddCardDTO newCard)
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
        public async Task<IActionResult> UpdateCard(long id, [FromBody] EditCardDTO editCard)
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
        public async Task<IActionResult> DeleteCard(long id)
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
        public async Task<IActionResult> IncreaseBalance([FromBody] BalanceUpdateDTO balanceUpdate)
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
        public async Task<IActionResult> DecreaseBalance([FromBody] BalanceUpdateDTO balanceUpdate)
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
