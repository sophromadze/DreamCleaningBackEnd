using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DreamCleaningBackend.Data;
using DreamCleaningBackend.DTOs;
using DreamCleaningBackend.Models;

namespace DreamCleaningBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public BookingController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("service-types")]
        public async Task<ActionResult<List<ServiceTypeDto>>> GetServiceTypes()
        {
            var serviceTypes = await _context.ServiceTypes
                .Include(st => st.Services.Where(s => s.IsActive))
                .Where(st => st.IsActive)
                .OrderBy(st => st.DisplayOrder)
                .ToListAsync();

            // Get all extra services that are available for all
            var universalExtraServices = await _context.ExtraServices
                .Where(es => es.IsActive && es.IsAvailableForAll && es.ServiceTypeId == null)
                .OrderBy(es => es.DisplayOrder)
                .ToListAsync();

            var result = new List<ServiceTypeDto>();

            foreach (var st in serviceTypes)
            {
                // Get extra services specific to this service type
                var specificExtraServices = await _context.ExtraServices
                    .Where(es => es.IsActive && es.ServiceTypeId == st.Id && !es.IsAvailableForAll)
                    .OrderBy(es => es.DisplayOrder)
                    .ToListAsync();

                var serviceTypeDto = new ServiceTypeDto
                {
                    Id = st.Id,
                    Name = st.Name,
                    BasePrice = st.BasePrice,
                    Description = st.Description,
                    Services = st.Services.Select(s => new ServiceDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        ServiceKey = s.ServiceKey,
                        Cost = s.Cost,
                        TimeDuration = s.TimeDuration,
                        ServiceTypeId = s.ServiceTypeId,
                        InputType = s.InputType,
                        MinValue = s.MinValue,
                        MaxValue = s.MaxValue,
                        StepValue = s.StepValue,
                        IsRangeInput = s.IsRangeInput,
                        Unit = s.Unit
                    }).ToList(),
                    ExtraServices = new List<ExtraServiceDto>()
                };

                // Add specific extra services first
                serviceTypeDto.ExtraServices.AddRange(specificExtraServices.Select(es => new ExtraServiceDto
                {
                    Id = es.Id,
                    Name = es.Name,
                    Description = es.Description,
                    Price = es.Price,
                    Duration = es.Duration,
                    Icon = es.Icon,
                    HasQuantity = es.HasQuantity,
                    HasHours = es.HasHours,
                    IsDeepCleaning = es.IsDeepCleaning,
                    IsSuperDeepCleaning = es.IsSuperDeepCleaning,
                    IsSameDayService = es.IsSameDayService,
                    PriceMultiplier = es.PriceMultiplier,
                    IsAvailableForAll = es.IsAvailableForAll
                }));

                // Add universal extra services
                serviceTypeDto.ExtraServices.AddRange(universalExtraServices.Select(es => new ExtraServiceDto
                {
                    Id = es.Id,
                    Name = es.Name,
                    Description = es.Description,
                    Price = es.Price,
                    Duration = es.Duration,
                    Icon = es.Icon,
                    HasQuantity = es.HasQuantity,
                    HasHours = es.HasHours,
                    IsDeepCleaning = es.IsDeepCleaning,
                    IsSuperDeepCleaning = es.IsSuperDeepCleaning,
                    IsSameDayService = es.IsSameDayService,
                    PriceMultiplier = es.PriceMultiplier,
                    IsAvailableForAll = es.IsAvailableForAll
                }));

                result.Add(serviceTypeDto);
            }

            return Ok(result);
        }

        [HttpGet("frequencies")]
        public async Task<ActionResult<List<FrequencyDto>>> GetFrequencies()
        {
            var frequencies = await _context.Frequencies
                .Where(f => f.IsActive)
                .OrderBy(f => f.DisplayOrder)
                .Select(f => new FrequencyDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    Description = f.Description,
                    DiscountPercentage = f.DiscountPercentage,
                    FrequencyDays = f.FrequencyDays
                })
                .ToListAsync();

            return Ok(frequencies);
        }

        [HttpPost("validate-promo")]
        public async Task<ActionResult<PromoCodeValidationDto>> ValidatePromoCode(ValidatePromoCodeDto dto)
        {
            var promoCode = await _context.PromoCodes
                .FirstOrDefaultAsync(p => p.Code.ToLower() == dto.Code.ToLower() && p.IsActive);

            if (promoCode == null)
            {
                return Ok(new PromoCodeValidationDto
                {
                    IsValid = false,
                    Message = "Invalid promo code"
                });
            }

            // Check validity dates
            if (promoCode.ValidFrom.HasValue && promoCode.ValidFrom.Value > DateTime.UtcNow)
            {
                return Ok(new PromoCodeValidationDto
                {
                    IsValid = false,
                    Message = "Promo code is not yet valid"
                });
            }

            if (promoCode.ValidTo.HasValue && promoCode.ValidTo.Value < DateTime.UtcNow)
            {
                return Ok(new PromoCodeValidationDto
                {
                    IsValid = false,
                    Message = "Promo code has expired"
                });
            }

            // Check usage limits
            if (promoCode.MaxUsageCount.HasValue && promoCode.CurrentUsageCount >= promoCode.MaxUsageCount.Value)
            {
                return Ok(new PromoCodeValidationDto
                {
                    IsValid = false,
                    Message = "Promo code usage limit reached"
                });
            }

            return Ok(new PromoCodeValidationDto
            {
                IsValid = true,
                DiscountValue = promoCode.DiscountValue,
                IsPercentage = promoCode.IsPercentage
            });
        }

        [HttpPost("calculate")]
        public async Task<ActionResult<BookingCalculationDto>> CalculateBooking(CreateBookingDto dto)
        {
            // This would contain the same calculation logic as the frontend
            // For now, we'll return a simple calculation
            var calculation = new BookingCalculationDto
            {
                SubTotal = 150,
                Tax = 13.20m,
                DiscountAmount = 0,
                Tips = dto.Tips,
                Total = 163.20m + dto.Tips,
                TotalDuration = 120
            };

            return Ok(calculation);
        }

        // In BookingController.cs, update the CreateBooking method:

        [HttpPost("create")]
        [Authorize]
        public async Task<ActionResult<BookingResponseDto>> CreateBooking(CreateBookingDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (userId == 0)
                    return Unauthorized();

                // Validate service type exists
                var serviceType = await _context.ServiceTypes.FindAsync(dto.ServiceTypeId);
                if (serviceType == null)
                    return BadRequest(new { message = "Invalid service type" });

                // Validate frequency exists
                var frequency = await _context.Frequencies.FindAsync(dto.FrequencyId);
                if (frequency == null)
                    return BadRequest(new { message = "Invalid frequency" });

                // Ensure the service date is properly set with UTC kind
                var serviceDate = dto.ServiceDate.Date;
                if (serviceDate.Kind == DateTimeKind.Unspecified)
                {
                    serviceDate = DateTime.SpecifyKind(serviceDate, DateTimeKind.Utc);
                }

                // Create the order
                var order = new Order
                {
                    UserId = userId,
                    ServiceTypeId = dto.ServiceTypeId,
                    FrequencyId = dto.FrequencyId,
                    OrderDate = DateTime.UtcNow,
                    ServiceDate = serviceDate,
                    ServiceTime = TimeSpan.Parse(dto.ServiceTime),
                    EntryMethod = dto.EntryMethod,
                    SpecialInstructions = dto.SpecialInstructions,
                    ContactFirstName = dto.ContactFirstName,
                    ContactLastName = dto.ContactLastName,
                    ContactEmail = dto.ContactEmail,
                    ContactPhone = dto.ContactPhone,
                    ServiceAddress = dto.ServiceAddress,
                    AptSuite = dto.AptSuite,
                    City = dto.City,
                    State = dto.State,
                    ZipCode = dto.ZipCode,
                    ApartmentId = dto.ApartmentId,
                    PromoCode = dto.PromoCode,
                    Tips = dto.Tips,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                // Calculate pricing (simplified for now)
                decimal subTotal = serviceType.BasePrice;
                int totalDuration = 0;

                // Add services
                foreach (var serviceDto in dto.Services)
                {
                    var service = await _context.Services.FindAsync(serviceDto.ServiceId);
                    if (service != null)
                    {
                        var orderService = new OrderService
                        {
                            ServiceId = serviceDto.ServiceId,
                            Quantity = serviceDto.Quantity,
                            Cost = service.Cost * serviceDto.Quantity,
                            Duration = service.TimeDuration * serviceDto.Quantity,
                            CreatedAt = DateTime.UtcNow
                        };
                        order.OrderServices.Add(orderService);
                        subTotal += orderService.Cost;
                        totalDuration += orderService.Duration;
                    }
                }

                // Add extra services
                foreach (var extraServiceDto in dto.ExtraServices)
                {
                    var extraService = await _context.ExtraServices.FindAsync(extraServiceDto.ExtraServiceId);
                    if (extraService != null)
                    {
                        var cost = extraService.HasHours
                            ? extraService.Price * extraServiceDto.Hours
                            : extraService.Price * extraServiceDto.Quantity;

                        var orderExtraService = new OrderExtraService
                        {
                            ExtraServiceId = extraServiceDto.ExtraServiceId,
                            Quantity = extraServiceDto.Quantity,
                            Hours = extraServiceDto.Hours,
                            Cost = cost,
                            Duration = extraService.Duration,
                            CreatedAt = DateTime.UtcNow
                        };
                        order.OrderExtraServices.Add(orderExtraService);
                        subTotal += cost;
                        totalDuration += orderExtraService.Duration;
                    }
                }

                // Apply frequency discount
                if (frequency.DiscountPercentage > 0)
                {
                    order.DiscountAmount = subTotal * (frequency.DiscountPercentage / 100);
                }

                // Apply first-time discount
                var user = await _context.Users.FindAsync(userId);
                if (user != null && user.FirstTimeOrder)
                {
                    order.DiscountAmount += subTotal * 0.20m; // 20% first-time discount
                    user.FirstTimeOrder = false; // Mark as used
                }

                // Calculate totals
                order.SubTotal = subTotal;
                order.Tax = (subTotal - order.DiscountAmount) * 0.088m; // 8.8% tax
                order.Total = order.SubTotal - order.DiscountAmount + order.Tax + order.Tips;
                order.TotalDuration = totalDuration;

                // Add order to database
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Here you would integrate with Stripe to create a payment intent
                // For now, we'll return a dummy response
                return Ok(new BookingResponseDto
                {
                    OrderId = order.Id,
                    Status = order.Status,
                    Total = order.Total,
                    PaymentIntentId = "pi_dummy_" + Guid.NewGuid().ToString()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to create booking: " + ex.Message });
            }
        }

        // Add this method to BookingController.cs after the CreateBooking method:

        [HttpPost("simulate-payment/{orderId}")]
        [Authorize]
        public async Task<ActionResult> SimulatePayment(int orderId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (userId == 0)
                    return Unauthorized();

                var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

                if (order == null)
                    return NotFound(new { message = "Order not found" });

                if (order.IsPaid)
                    return BadRequest(new { message = "Order is already paid" });

                // Simulate payment completion
                order.IsPaid = true;
                order.PaidAt = DateTime.UtcNow;
                order.Status = "Active";
                order.PaymentIntentId = "pi_simulated_" + Guid.NewGuid().ToString();

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Payment completed successfully",
                    orderId = order.Id,
                    status = order.Status
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to process payment: " + ex.Message });
            }
        }

        [HttpGet("available-times")]
        public ActionResult<List<string>> GetAvailableTimeSlots(DateTime date, int serviceTypeId)
        {
            // Simple time slot generation - in a real app, you'd check existing bookings
            var timeSlots = new List<string>
            {
                "08:00", "09:00", "10:00", "11:00", "12:00",
                "13:00", "14:00", "15:00", "16:00"
            };

            return Ok(timeSlots);
        }
    }
}