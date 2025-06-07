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
                        Unit = s.Unit,
                        ServiceRelationType = s.ServiceRelationType,
                        IsActive = s.IsActive
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

                // Calculate pricing
                decimal subTotal = serviceType.BasePrice;
                int totalDuration = 0;
                decimal priceMultiplier = 1;
                decimal deepCleaningFee = 0;

                // Check for deep cleaning multipliers
                foreach (var extraServiceDto in dto.ExtraServices)
                {
                    var extraService = await _context.ExtraServices.FindAsync(extraServiceDto.ExtraServiceId);
                    if (extraService != null)
                    {
                        if (extraService.IsSuperDeepCleaning)
                        {
                            priceMultiplier = extraService.PriceMultiplier;
                            deepCleaningFee = extraService.Price;
                            break; // Super deep cleaning takes precedence
                        }
                        else if (extraService.IsDeepCleaning && priceMultiplier == 1)
                        {
                            priceMultiplier = extraService.PriceMultiplier;
                            deepCleaningFee = extraService.Price;
                        }
                    }
                }

                // Apply multiplier to base price
                subTotal *= priceMultiplier;

                // Create order - assuming your Order entity expects string for ServiceTime
                var order = new Order
                {
                    UserId = userId,
                    ServiceTypeId = dto.ServiceTypeId,
                    OrderDate = DateTime.UtcNow,
                    ServiceDate = dto.ServiceDate,
                    ServiceTime = dto.ServiceTime, // Keep as string since Order expects string
                    Status = "Pending",
                    FrequencyId = dto.FrequencyId,
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
                    PromoCode = dto.PromoCode,
                    Tips = dto.Tips,
                    IsPaid = false,
                    OrderServices = new List<OrderService>(),
                    OrderExtraServices = new List<OrderExtraService>()
                };

                // Calculate services cost and duration
                foreach (var serviceDto in dto.Services)
                {
                    var service = await _context.Services.FindAsync(serviceDto.ServiceId);
                    if (service != null)
                    {
                        decimal serviceCost = 0;
                        int serviceDuration = 0;
                        bool shouldAddToOrder = true;

                        // Special handling for cleaner-hours relationship
                        if (service.ServiceRelationType == "cleaner")
                        {
                            // Find the hours service in the same order
                            var hoursServiceDto = dto.Services.FirstOrDefault(s =>
                            {
                                var svc = _context.Services.Find(s.ServiceId);
                                return svc?.ServiceRelationType == "hours" && svc.ServiceTypeId == service.ServiceTypeId;
                            });

                            if (hoursServiceDto != null)
                            {
                                var hours = hoursServiceDto.Quantity;
                                var cleaners = serviceDto.Quantity;
                                var costPerCleanerPerHour = service.Cost * priceMultiplier;
                                serviceCost = costPerCleanerPerHour * cleaners * hours;
                                serviceDuration = hours * 60; // Convert to minutes
                            }
                            else
                            {
                                // If no hours service found, calculate based on cleaner count only
                                serviceCost = service.Cost * serviceDto.Quantity * priceMultiplier;
                                serviceDuration = service.TimeDuration * serviceDto.Quantity;
                            }
                        }
                        else if (service.ServiceKey == "bedrooms" && serviceDto.Quantity == 0)
                        {
                            // Studio apartment - flat rate
                            serviceCost = 20 * priceMultiplier;
                            serviceDuration = 20; // 20 minutes for studio
                        }
                        else if (service.ServiceRelationType != "hours")
                        {
                            // Regular service calculation
                            serviceCost = service.Cost * serviceDto.Quantity * priceMultiplier;
                            serviceDuration = service.TimeDuration * serviceDto.Quantity;
                        }
                        else
                        {
                            // Skip hours service as it's already processed with cleaners
                            shouldAddToOrder = service.ServiceRelationType != "hours" ||
                                             !dto.Services.Any(s =>
                                             {
                                                 var svc = _context.Services.Find(s.ServiceId);
                                                 return svc?.ServiceRelationType == "cleaner" && svc.ServiceTypeId == service.ServiceTypeId;
                                             });

                            if (shouldAddToOrder)
                            {
                                serviceCost = service.Cost * serviceDto.Quantity * priceMultiplier;
                                serviceDuration = service.TimeDuration * serviceDto.Quantity;
                            }
                        }

                        if (shouldAddToOrder)
                        {
                            subTotal += serviceCost;
                            totalDuration += serviceDuration;

                            // Add to order services
                            order.OrderServices.Add(new OrderService
                            {
                                ServiceId = service.Id,
                                Quantity = serviceDto.Quantity,
                                Cost = (int)Math.Round(serviceCost), // Convert decimal to int
                                Duration = serviceDuration,
                                PriceMultiplier = priceMultiplier
                            });
                        }
                    }
                }

                // Calculate extra services cost and duration
                foreach (var extraServiceDto in dto.ExtraServices)
                {
                    var extraService = await _context.ExtraServices.FindAsync(extraServiceDto.ExtraServiceId);
                    if (extraService != null)
                    {
                        decimal extraCost = 0;
                        int extraDuration = 0;

                        if (!extraService.IsDeepCleaning && !extraService.IsSuperDeepCleaning)
                        {
                            var currentMultiplier = extraService.IsSameDayService ? 1 : priceMultiplier;

                            if (extraService.HasHours)
                            {
                                extraCost = extraService.Price * extraServiceDto.Hours * currentMultiplier;
                                extraDuration = (int)(extraService.Duration * extraServiceDto.Hours);
                            }
                            else if (extraService.HasQuantity)
                            {
                                extraCost = extraService.Price * extraServiceDto.Quantity * currentMultiplier;
                                extraDuration = extraService.Duration * extraServiceDto.Quantity;
                            }
                            else
                            {
                                extraCost = extraService.Price * currentMultiplier;
                                extraDuration = extraService.Duration;
                            }

                            subTotal += extraCost;
                        }
                        else
                        {
                            // Deep cleaning services - only duration
                            extraDuration = extraService.Duration;
                        }

                        totalDuration += extraDuration;

                        // Add to order extra services
                        order.OrderExtraServices.Add(new OrderExtraService
                        {
                            ExtraServiceId = extraService.Id,
                            Quantity = extraServiceDto.Quantity,
                            Hours = (int)Math.Round(extraServiceDto.Hours), // Convert decimal to int
                            Cost = (int)Math.Round(extraCost), // Convert decimal to int
                            Duration = extraDuration
                        });
                    }
                }

                // Calculate maids count
                int maidsCount = 1;
                int displayDuration = totalDuration;

                // Check if cleaners are explicitly selected
                var cleanerService = dto.Services.FirstOrDefault(s =>
                {
                    var service = _context.Services.Find(s.ServiceId);
                    return service?.ServiceRelationType == "cleaner";
                });

                if (cleanerService != null)
                {
                    // Use the selected cleaner count
                    maidsCount = cleanerService.Quantity;

                    // Check if hours service is also selected
                    var hoursService = dto.Services.FirstOrDefault(s =>
                    {
                        var service = _context.Services.Find(s.ServiceId);
                        return service?.ServiceRelationType == "hours";
                    });

                    if (hoursService != null)
                    {
                        // When both cleaners and hours are selected, use hours for duration
                        displayDuration = hoursService.Quantity * 60;
                    }
                }
                else
                {
                    // Calculate based on duration (every 6 hours = 1 maid)
                    var totalHours = totalDuration / 60.0;
                    if (totalHours > 6)
                    {
                        maidsCount = (int)Math.Ceiling(totalHours / 6);
                        displayDuration = (int)Math.Ceiling((double)totalDuration / maidsCount);
                    }
                }

                // Add deep cleaning fee after all calculations
                subTotal += deepCleaningFee;

                // Apply frequency discount
                var discountAmount = 0m;
                if (frequency.DiscountPercentage > 0)
                {
                    discountAmount = (subTotal - deepCleaningFee) * (frequency.DiscountPercentage / 100);
                }

                // Apply promo code if provided
                if (!string.IsNullOrEmpty(dto.PromoCode))
                {
                    var promoCode = await _context.PromoCodes.FirstOrDefaultAsync(pc =>
                        pc.Code == dto.PromoCode && pc.IsActive);

                    if (promoCode != null)
                    {
                        if (promoCode.IsPercentage)
                        {
                            discountAmount += (subTotal - deepCleaningFee) * (promoCode.DiscountValue / 100);
                        }
                        else
                        {
                            discountAmount += promoCode.DiscountValue;
                        }
                    }
                }

                // Final calculations
                order.SubTotal = subTotal;
                order.DiscountAmount = discountAmount;
                order.Tax = (subTotal - discountAmount) * 0.088m; // 8.8% tax
                order.Total = order.SubTotal - order.DiscountAmount + order.Tax + order.Tips;
                order.TotalDuration = displayDuration;

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