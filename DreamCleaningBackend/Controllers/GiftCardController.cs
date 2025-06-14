// DreamCleaningBackend/Controllers/GiftCardController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DreamCleaningBackend.DTOs;
using DreamCleaningBackend.Services.Interfaces;

namespace DreamCleaningBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GiftCardController : ControllerBase
    {
        private readonly IGiftCardService _giftCardService;

        public GiftCardController(IGiftCardService giftCardService)
        {
            _giftCardService = giftCardService;
        }

        [HttpPost]
        public async Task<ActionResult<GiftCardPurchaseResponseDto>> CreateGiftCard(CreateGiftCardDto createDto)
        {
            try
            {
                var userId = GetUserId();
                var giftCard = await _giftCardService.CreateGiftCard(userId, createDto);

                return Ok(new GiftCardPurchaseResponseDto
                {
                    GiftCardId = giftCard.Id,
                    Code = giftCard.Code,
                    Amount = giftCard.OriginalAmount,
                    Status = "Created",
                    PaymentIntentId = "pi_dummy_" + Guid.NewGuid().ToString()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to create gift card: " + ex.Message });
            }
        }

        [HttpPost("validate")]
        public async Task<ActionResult<GiftCardValidationDto>> ValidateGiftCard(ApplyGiftCardDto applyDto)
        {
            try
            {
                var validation = await _giftCardService.ValidateGiftCard(applyDto.Code);
                return Ok(validation);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to validate gift card: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<GiftCardDto>>> GetUserGiftCards()
        {
            try
            {
                var userId = GetUserId();
                var giftCards = await _giftCardService.GetUserGiftCards(userId);
                return Ok(giftCards);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to get gift cards: " + ex.Message });
            }
        }

        [HttpGet("{code}/usage-history")]
        public async Task<ActionResult<List<GiftCardUsageDto>>> GetGiftCardUsageHistory(string code)
        {
            try
            {
                var userId = GetUserId();
                var usages = await _giftCardService.GetGiftCardUsageHistory(code, userId);
                return Ok(usages);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to get usage history: " + ex.Message });
            }
        }

        [HttpPost("simulate-payment/{giftCardId}")]
        public async Task<ActionResult> SimulateGiftCardPayment(int giftCardId)
        {
            try
            {
                var paymentIntentId = "pi_dummy_" + Guid.NewGuid().ToString();
                var success = await _giftCardService.MarkGiftCardAsPaid(giftCardId, paymentIntentId);

                if (success)
                {
                    return Ok(new { message = "Gift card payment processed successfully", paymentIntentId });
                }
                else
                {
                    return BadRequest(new { message = "Failed to process gift card payment" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to process payment: " + ex.Message });
            }
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }
            return userId;
        }
    }
}