using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DreamCleaningBackend.DTOs;
using DreamCleaningBackend.Services.Interfaces;
using System.Threading.Tasks;

namespace DreamCleaningBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult> GetUserOrders()
        {
            try
            {
                var userId = GetUserId();
                var orders = await _orderService.GetUserOrders(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult> GetOrderById(int orderId)
        {
            try
            {
                var userId = GetUserId();
                var order = await _orderService.GetOrderById(orderId, userId);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{orderId}")]
        public async Task<ActionResult> UpdateOrder(int orderId, UpdateOrderDto updateOrderDto)
        {
            try
            {
                var userId = GetUserId();
                var order = await _orderService.UpdateOrder(orderId, userId, updateOrderDto);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{orderId}/cancel")]
        public async Task<ActionResult> CancelOrder(int orderId, CancelOrderDto cancelOrderDto)
        {
            try
            {
                var userId = GetUserId();
                await _orderService.CancelOrder(orderId, userId, cancelOrderDto);
                return Ok(new { message = "Order cancelled successfully. Refund will be processed within 7 working days." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{orderId}/calculate-additional")]
        public async Task<ActionResult> CalculateAdditionalAmount(int orderId, UpdateOrderDto updateOrderDto)
        {
            try
            {
                var userId = GetUserId();
                var additionalAmount = await _orderService.CalculateAdditionalAmount(orderId, updateOrderDto);
                return Ok(new { additionalAmount });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                throw new Exception("Invalid user");

            return userId;
        }
    }
}