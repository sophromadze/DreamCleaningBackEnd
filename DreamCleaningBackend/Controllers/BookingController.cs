using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DreamCleaningBackend.Data;
using DreamCleaningBackend.DTOs;
using DreamCleaningBackend.Models;
using DreamCleaningBackend.Services.Interfaces;

namespace DreamCleaningBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ISubscriptionService _subscriptionService;

        public BookingController(ApplicationDbContext context, IConfiguration configuration, ISubscriptionService subscriptionService)
        {
            _context = context;
            _configuration = configuration;
            _subscriptionService = subscriptionService;
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

        [HttpGet("user-subscription")]
        [Authorize]
        public async Task<ActionResult> GetUserSubscription()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
                return Unauthorized();

            var user = await _context.Users
                .Include(u => u.Subscription)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            // Check and update subscription status
            await _subscriptionService.CheckAndUpdateSubscriptionStatus(userId);

            if (user.SubscriptionId == null)
            {
                return Ok(new { hasSubscription = false });
            }

            return Ok(new
            {
                hasSubscription = true,
                subscriptionId = user.SubscriptionId,
                subscriptionName = user.Subscription.Name,
                discountPercentage = user.Subscription.DiscountPercentage,
                expiryDate = user.SubscriptionExpiryDate,
            });
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

        // In the CreateBooking method of BookingController.cs, add these Console.WriteLine statements:

        [HttpPost("create")]
        [Authorize]
        public async Task<ActionResult<BookingResponseDto>> CreateBooking(CreateBookingDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (userId == 0)
                    return Unauthorized();

                // LOG: Initial values
                Console.WriteLine($"=== BOOKING CREATION START ===");
                Console.WriteLine($"TotalDuration from Frontend: {dto.TotalDuration}");
                Console.WriteLine($"ServiceTypeId: {dto.ServiceTypeId}");

                // Find the user
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return Unauthorized();

                // Get service type to check base price
                var serviceType = await _context.ServiceTypes
                    .Include(st => st.Services)
                    .FirstOrDefaultAsync(st => st.Id == dto.ServiceTypeId);

                if (serviceType == null)
                    return BadRequest(new { message = "Invalid service type" });

                // Create order
                var order = new Order
                {
                    UserId = userId,
                    ServiceTypeId = dto.ServiceTypeId,
                    ApartmentId = dto.ApartmentId,
                    ApartmentName = dto.ApartmentName,
                    ServiceAddress = dto.ServiceAddress,
                    AptSuite = dto.AptSuite,
                    City = dto.City,
                    State = dto.State,
                    ZipCode = dto.ZipCode,
                    ServiceDate = dto.ServiceDate,
                    ServiceTime = TimeSpan.Parse(dto.ServiceTime),
                    EntryMethod = dto.EntryMethod,
                    SpecialInstructions = dto.SpecialInstructions,
                    ContactFirstName = dto.ContactFirstName,
                    ContactLastName = dto.ContactLastName,
                    ContactEmail = dto.ContactEmail,
                    ContactPhone = dto.ContactPhone,
                    PromoCode = dto.PromoCode,
                    Tips = dto.Tips,
                    Status = "Pending",
                    OrderDate = DateTime.UtcNow,
                    FrequencyId = dto.FrequencyId,
                    OrderServices = new List<OrderService>(),
                    OrderExtraServices = new List<OrderExtraService>()
                };

                // Calculate subtotal
                decimal subTotal = 0;
                int totalDuration = 0;
                decimal priceMultiplier = 1;
                decimal deepCleaningFee = 0;

                // Check for deep cleaning multipliers FIRST
                foreach (var extraServiceDto in dto.ExtraServices)
                {
                    var extraService = await _context.ExtraServices.FindAsync(extraServiceDto.ExtraServiceId);
                    if (extraService != null && (extraService.IsDeepCleaning || extraService.IsSuperDeepCleaning))
                    {
                        if (extraService.IsSuperDeepCleaning)
                        {
                            priceMultiplier = extraService.PriceMultiplier;
                            deepCleaningFee = extraService.Price;
                            Console.WriteLine($"Super Deep Cleaning detected - Multiplier: {priceMultiplier}, Fee: {deepCleaningFee}");
                        }
                        else if (extraService.IsDeepCleaning)
                        {
                            priceMultiplier = extraService.PriceMultiplier;
                            deepCleaningFee = extraService.Price;
                            Console.WriteLine($"Deep Cleaning detected - Multiplier: {priceMultiplier}, Fee: {deepCleaningFee}");
                        }
                    }
                }

                // Add base price
                subTotal += serviceType.BasePrice * priceMultiplier;
                Console.WriteLine($"Base Price: {serviceType.BasePrice} x {priceMultiplier} = {serviceType.BasePrice * priceMultiplier}");

                // Add services
                Console.WriteLine($"\n--- SERVICES CALCULATION ---");
                foreach (var serviceDto in dto.Services)
                {
                    var service = await _context.Services.FindAsync(serviceDto.ServiceId);
                    if (service != null)
                    {
                        Console.WriteLine($"\nService: {service.Name} (ID: {service.Id})");
                        Console.WriteLine($"  ServiceRelationType: {service.ServiceRelationType}");
                        Console.WriteLine($"  Quantity: {serviceDto.Quantity}");
                        Console.WriteLine($"  TimeDuration: {service.TimeDuration}");

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

                                Console.WriteLine($"  Cleaner-Hours calculation:");
                                Console.WriteLine($"    Hours: {hours}");
                                Console.WriteLine($"    Cleaners: {cleaners}");
                                Console.WriteLine($"    Cost: {costPerCleanerPerHour} x {cleaners} x {hours} = {serviceCost}");
                                Console.WriteLine($"    Duration: {hours} hours x 60 = {serviceDuration} minutes");
                            }
                            else
                            {
                                serviceCost = service.Cost * serviceDto.Quantity * priceMultiplier;
                                serviceDuration = service.TimeDuration * serviceDto.Quantity;
                                Console.WriteLine($"  No hours service found, using default calculation");
                                Console.WriteLine($"    Duration: {service.TimeDuration} x {serviceDto.Quantity} = {serviceDuration} minutes");
                            }
                        }
                        else if (service.ServiceKey == "bedrooms" && serviceDto.Quantity == 0)
                        {
                            // Studio apartment
                            serviceCost = 20 * priceMultiplier;
                            serviceDuration = 20;
                            Console.WriteLine($"  Studio apartment - Fixed duration: 20 minutes");
                        }
                        else if (service.ServiceRelationType == "hours")
                        {
                            // Hours service - don't add to order or duration
                            shouldAddToOrder = false;
                            Console.WriteLine($"  Hours service - skipping (already counted in cleaner calculation)");
                        }
                        else
                        {
                            serviceCost = service.Cost * serviceDto.Quantity * priceMultiplier;
                            serviceDuration = service.TimeDuration * serviceDto.Quantity;
                            Console.WriteLine($"  Regular service calculation:");
                            Console.WriteLine($"    Duration: {service.TimeDuration} x {serviceDto.Quantity} = {serviceDuration} minutes");
                        }

                        // Add to order if it should be added
                        if (shouldAddToOrder)
                        {
                            var orderService = new OrderService
                            {
                                ServiceId = serviceDto.ServiceId,
                                Quantity = serviceDto.Quantity,
                                Cost = serviceCost,
                                Duration = serviceDuration,
                                PriceMultiplier = priceMultiplier,
                                CreatedAt = DateTime.UtcNow
                            };
                            order.OrderServices.Add(orderService);
                            subTotal += serviceCost;
                            totalDuration += serviceDuration;

                            Console.WriteLine($"  Added to total - Running total duration: {totalDuration} minutes");
                        }
                    }
                }

                // Add extra services
                Console.WriteLine($"\n--- EXTRA SERVICES CALCULATION ---");
                foreach (var extraServiceDto in dto.ExtraServices)
                {
                    var extraService = await _context.ExtraServices.FindAsync(extraServiceDto.ExtraServiceId);
                    if (extraService != null)
                    {
                        Console.WriteLine($"\nExtra Service: {extraService.Name} (ID: {extraService.Id})");
                        Console.WriteLine($"  Quantity: {extraServiceDto.Quantity}");
                        Console.WriteLine($"  Hours: {extraServiceDto.Hours}");
                        Console.WriteLine($"  Duration per unit: {extraService.Duration}");
                        Console.WriteLine($"  HasHours: {extraService.HasHours}");
                        Console.WriteLine($"  HasQuantity: {extraService.HasQuantity}");

                        decimal cost = 0;
                        int duration = 0;

                        // For deep cleaning services, store their actual price
                        if (extraService.IsDeepCleaning || extraService.IsSuperDeepCleaning)
                        {
                            cost = extraService.Price; // Store the actual deep cleaning fee
                            duration = extraService.Duration;
                            Console.WriteLine($"  Deep cleaning service - Duration: {duration} minutes");
                        }
                        else
                        {
                            // Regular extra services - apply multiplier EXCEPT for Same Day Service
                            var currentMultiplier = extraService.IsSameDayService ? 1 : priceMultiplier;

                            if (extraService.HasHours)
                            {
                                cost = extraService.Price * extraServiceDto.Hours * currentMultiplier;
                                duration = (int)(extraService.Duration * extraServiceDto.Hours);
                                Console.WriteLine($"  HasHours calculation - Duration: {extraService.Duration} x {extraServiceDto.Hours} = {duration} minutes");
                            }
                            else if (extraService.HasQuantity)
                            {
                                cost = extraService.Price * extraServiceDto.Quantity * currentMultiplier;
                                duration = extraService.Duration * extraServiceDto.Quantity;
                                Console.WriteLine($"  HasQuantity calculation - Duration: {extraService.Duration} x {extraServiceDto.Quantity} = {duration} minutes");
                            }
                            else
                            {
                                cost = extraService.Price * currentMultiplier;
                                duration = extraService.Duration;
                                Console.WriteLine($"  Regular calculation - Duration: {duration} minutes");
                            }
                        }

                        var orderExtraService = new OrderExtraService
                        {
                            ExtraServiceId = extraServiceDto.ExtraServiceId,
                            Quantity = extraServiceDto.Quantity,
                            Hours = extraServiceDto.Hours,
                            Cost = cost,
                            Duration = duration,
                            CreatedAt = DateTime.UtcNow
                        };
                        order.OrderExtraServices.Add(orderExtraService);

                        if (!extraService.IsDeepCleaning && !extraService.IsSuperDeepCleaning)
                        {
                            subTotal += cost;
                        }
                        totalDuration += duration;

                        Console.WriteLine($"  Added to total - Running total duration: {totalDuration} minutes");
                    }
                }

                // Add deep cleaning fee after discount calculations
                subTotal += deepCleaningFee;

                Console.WriteLine($"\n=== FINAL CALCULATIONS ===");
                Console.WriteLine($"Backend Calculated Total Duration: {totalDuration} minutes");
                Console.WriteLine($"Frontend Sent Total Duration: {dto.TotalDuration} minutes");
                Console.WriteLine($"Frontend Sent Maids Count: {dto.MaidsCount}");
                Console.WriteLine($"DIFFERENCE: {dto.TotalDuration - totalDuration} minutes");

                // If there's a mismatch, use the frontend value but log it
                if (dto.TotalDuration != totalDuration)
                {
                    Console.WriteLine($"WARNING: Duration mismatch! Using frontend value: {dto.TotalDuration}");
                    // Uncomment the line below to use frontend duration instead of backend calculation
                    // totalDuration = dto.TotalDuration;
                }

                // If there's a significant mismatch, use the frontend value
                // The frontend has all the user selections and should be the source of truth
                if (Math.Abs(dto.TotalDuration - totalDuration) > 5) // Allow 5 minutes tolerance
                {
                    Console.WriteLine($"WARNING: Duration mismatch! Using frontend value: {dto.TotalDuration}");
                    totalDuration = dto.TotalDuration;
                }

                //// Set MaidsCount from the frontend
                //order.MaidsCount = dto.MaidsCount;

                //// If MaidsCount is 0 (not sent from frontend), calculate it
                //if (order.MaidsCount == 0)
                //{
                //    // Check if cleaners are explicitly selected
                //    var cleanerService = order.OrderServices.FirstOrDefault(os =>
                //    {
                //        var service = _context.Services.Find(os.ServiceId);
                //        return service?.ServiceRelationType == "cleaner";
                //    });

                //    if (cleanerService != null)
                //    {
                //        // Use the cleaner count
                //        order.MaidsCount = cleanerService.Quantity;
                //        Console.WriteLine($"Calculated MaidsCount from cleaner service: {order.MaidsCount}");
                //    }
                //    else
                //    {
                //        // Calculate based on duration (every 6 hours = 1 maid)
                //        decimal totalHours = totalDuration / 60m;
                //        order.MaidsCount = Math.Max(1, (int)Math.Ceiling(totalHours / 6m));
                //        Console.WriteLine($"Calculated MaidsCount from duration: {order.MaidsCount}");
                //    }
                //}

                //// Apply frequency discount
                //order.DiscountAmount = 0;
                var frequency = await _context.Frequencies.FindAsync(dto.FrequencyId);
                //if (frequency != null && frequency.DiscountPercentage > 0)
                //{
                //    order.DiscountAmount = subTotal * (frequency.DiscountPercentage / 100);
                //}

                // Set MaidsCount from the frontend
                order.MaidsCount = dto.MaidsCount;

                // If MaidsCount is 0 (not sent from frontend), calculate it
                if (order.MaidsCount == 0)
                {
                    // Check if cleaners are explicitly selected
                    var cleanerService = order.OrderServices.FirstOrDefault(os =>
                    {
                        var service = _context.Services.Find(os.ServiceId);
                        return service?.ServiceRelationType == "cleaner";
                    });

                    if (cleanerService != null)
                    {
                        // Use the cleaner count
                        order.MaidsCount = cleanerService.Quantity;
                    }
                    else
                    {
                        // Calculate based on duration (every 6 hours = 1 maid)
                        decimal totalHours = totalDuration / 60m;
                        order.MaidsCount = Math.Max(1, (int)Math.Ceiling(totalHours / 6m));
                    }
                }

                //if (dto.DiscountAmount > 0)
                //{
                //    order.DiscountAmount = dto.DiscountAmount;
                //    Console.WriteLine($"Using frontend discount amount: ${dto.DiscountAmount}");
                //}
                //else
                //{
                //    // Only calculate discount if not provided by frontend
                //    order.DiscountAmount = 0;

                //    // Apply frequency discount
                //    if (frequency != null && frequency.DiscountPercentage > 0)
                //    {
                //        order.DiscountAmount += subTotal * (frequency.DiscountPercentage / 100m);
                //    }

                //    // Apply first time discount (20%) if promo code is "firstUse"
                //    if (dto.PromoCode == "firstUse" && user.FirstTimeOrder)
                //    {
                //        order.DiscountAmount += subTotal * 0.20m;
                //    }

                //    // Apply promo code discount
                //    if (!string.IsNullOrEmpty(dto.PromoCode) && dto.PromoCode != "firstUse")
                //    {
                //        var promoCode = await _context.PromoCodes
                //            .FirstOrDefaultAsync(p => p.Code == dto.PromoCode && p.IsActive);

                //        if (promoCode != null)
                //        {
                //            if (promoCode.IsPercentage)
                //            {
                //                order.DiscountAmount += subTotal * (promoCode.DiscountValue / 100m);
                //            }
                //            else
                //            {
                //                order.DiscountAmount += promoCode.DiscountValue;
                //            }
                //        }
                //    }
                //}

                order.DiscountAmount = dto.DiscountAmount; // This is promo/first-time discount ONLY
                order.SubscriptionDiscountAmount = dto.SubscriptionDiscountAmount; // Add this line if property exists

                // Complete order calculations
                order.SubTotal = subTotal;
                order.Tax = (subTotal - order.DiscountAmount - order.SubscriptionDiscountAmount) * 0.088m; // 8.8% tax
                order.Total = order.SubTotal - order.DiscountAmount - order.SubscriptionDiscountAmount + order.Tax + order.Tips;
                order.TotalDuration = totalDuration;

                Console.WriteLine($"Final values saved to DB:");
                Console.WriteLine($"- SubTotal: ${order.SubTotal}");
                Console.WriteLine($"- DiscountAmount (promo/first-time): ${order.DiscountAmount}");
                Console.WriteLine($"- SubscriptionDiscountAmount: ${order.SubscriptionDiscountAmount}");
                Console.WriteLine($"- Tax: ${order.Tax}");
                Console.WriteLine($"- Tips: ${order.Tips}");
                Console.WriteLine($"- Total: ${order.Total}");
                Console.WriteLine($"- Total Duration: {order.TotalDuration} minutes");
                Console.WriteLine($"- Maids Count: {order.MaidsCount}");
                Console.WriteLine($"=== BOOKING CREATION END ===\n");

                // Add order to database
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Handle subscription activation/renewal
                if (frequency != null && frequency.FrequencyDays > 0)
                {
                    var userForSubscription = await _context.Users
                        .Include(u => u.Subscription)
                        .FirstOrDefaultAsync(u => u.Id == userId);

                    bool hasActiveSubscription = await _subscriptionService.CheckAndUpdateSubscriptionStatus(userId);

                    if (!hasActiveSubscription)
                    {
                        // Find corresponding subscription based on frequency days
                        var subscription = await _context.Subscriptions
                            .FirstOrDefaultAsync(s => s.FrequencyDays == frequency.FrequencyDays);

                        if (subscription != null)
                        {
                            await _subscriptionService.ActivateSubscription(userId, subscription.Id);
                        }
                    }
                    else if (userForSubscription.SubscriptionId.HasValue)
                    {
                        // Renew existing subscription
                        await _subscriptionService.RenewSubscription(userId);
                    }
                }

                // Return response...
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

                var order = await _context.Orders
                    .Include(o => o.OrderServices)
                    .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

                if (order == null)
                    return NotFound(new { message = "Order not found" });

                if (order.IsPaid)
                    return BadRequest(new { message = "Order is already paid" });

                // Get the user to update phone number and check apartments
                var user = await _context.Users
                    .Include(u => u.Apartments)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user != null)
                {
                    // Update phone number if needed
                    if (string.IsNullOrEmpty(user.Phone) && !string.IsNullOrEmpty(order.ContactPhone))
                    {
                        user.Phone = order.ContactPhone;
                        user.UpdatedAt = DateTime.UtcNow;
                    }

                    // Handle first-time discount
                    if (order.PromoCode == "firstUse" && user.FirstTimeOrder)
                    {
                        user.FirstTimeOrder = false;
                        user.UpdatedAt = DateTime.UtcNow;
                    }

                    // FIXED: Only auto-save apartment if conditions are met
                    // 1. Order doesn't already have an apartment (not using saved address)
                    // 2. User provided an apartment name (intentional save)
                    // 3. All address fields are filled
                    if (order.ApartmentId == null && // Not using an existing apartment
                        !string.IsNullOrEmpty(order.ApartmentName) && // User provided apartment name
                        !string.IsNullOrEmpty(order.ServiceAddress) &&
                        !string.IsNullOrEmpty(order.City) &&
                        !string.IsNullOrEmpty(order.State) &&
                        !string.IsNullOrEmpty(order.ZipCode))
                    {
                        // Check if user has less than 10 apartments
                        if (user.Apartments.Count < 10)
                        {
                            // First, check if apartment with same address exists
                            var existingApartmentByAddress = user.Apartments.FirstOrDefault(a =>
                                a.IsActive &&
                                a.Address.ToLower() == order.ServiceAddress.ToLower() &&
                                a.City.ToLower() == order.City.ToLower() &&
                                a.State.ToLower() == order.State.ToLower() &&
                                a.PostalCode.ToLower() == order.ZipCode.ToLower()
                            );

                            if (existingApartmentByAddress != null)
                            {
                                // Found existing apartment with same address - link to it but DON'T update
                                order.ApartmentId = existingApartmentByAddress.Id;

                                // IMPORTANT: Use the actual apartment name from the matched apartment
                                order.ApartmentName = existingApartmentByAddress.Name;

                                Console.WriteLine($"Found existing apartment '{existingApartmentByAddress.Name}' with same address at {existingApartmentByAddress.Address}, linking to it without updating");
                            }
                            else
                            {
                                // No apartment with same address, now check by name
                                var existingApartmentByName = user.Apartments.FirstOrDefault(a =>
                                    a.IsActive &&
                                    a.Name.ToLower() == order.ApartmentName.ToLower()
                                );

                                if (existingApartmentByName != null)
                                {
                                    // Found existing apartment with same name - link and UPDATE
                                    order.ApartmentId = existingApartmentByName.Id;

                                    existingApartmentByName.Name = order.ApartmentName;
                                    existingApartmentByName.Address = order.ServiceAddress;
                                    existingApartmentByName.AptSuite = order.AptSuite;
                                    existingApartmentByName.City = order.City;
                                    existingApartmentByName.State = order.State;
                                    existingApartmentByName.PostalCode = order.ZipCode;
                                    existingApartmentByName.SpecialInstructions = order.SpecialInstructions;
                                    existingApartmentByName.UpdatedAt = DateTime.UtcNow;

                                    Console.WriteLine($"Found existing apartment with name '{existingApartmentByName.Name}', updating all fields");
                                }
                                else
                                {
                                    // No apartment with this name or address exists - create new one
                                    var newApartment = new Apartment
                                    {
                                        UserId = userId,
                                        Name = order.ApartmentName,
                                        Address = order.ServiceAddress,
                                        AptSuite = order.AptSuite,
                                        City = order.City,
                                        State = order.State,
                                        PostalCode = order.ZipCode,
                                        SpecialInstructions = order.SpecialInstructions,
                                        CreatedAt = DateTime.UtcNow,
                                        IsActive = true
                                    };

                                    _context.Apartments.Add(newApartment);
                                    await _context.SaveChangesAsync();

                                    // Update the order to link to the new apartment
                                    order.ApartmentId = newApartment.Id;

                                    Console.WriteLine($"Created new apartment '{order.ApartmentName}' at {order.ServiceAddress} for user {userId}");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"User {userId} has reached the maximum of 10 apartments");
                        }
                    }
                    // If no apartment name provided or apartment already exists, order remains without apartmentId
                }

                // Simulate payment completion
                order.IsPaid = true;
                order.PaidAt = DateTime.UtcNow;
                order.Status = "Active";
                order.PaymentIntentId = "pi_simulated_" + Guid.NewGuid().ToString();

                // Handle subscription activation for paid orders
                var frequency = await _context.Frequencies.FindAsync(order.FrequencyId);
                if (frequency != null && frequency.FrequencyDays > 0)
                {
                    var userForSubscription = await _context.Users
                        .Include(u => u.Subscription)
                        .FirstOrDefaultAsync(u => u.Id == userId);

                    bool hasActiveSubscription = await _subscriptionService.CheckAndUpdateSubscriptionStatus(userId);

                    if (!hasActiveSubscription)
                    {
                        // Find corresponding subscription based on frequency days
                        var subscription = await _context.Subscriptions
                            .FirstOrDefaultAsync(s => s.FrequencyDays == frequency.FrequencyDays);

                        if (subscription != null)
                        {
                            await _subscriptionService.ActivateSubscription(userId, subscription.Id);
                        }
                    }
                    else if (userForSubscription.SubscriptionId.HasValue)
                    {
                        // Renew existing subscription
                        await _subscriptionService.RenewSubscription(userId);
                    }
                }

                // Clear first-time discount after payment
                if (user.FirstTimeOrder && order.PromoCode == "firstUse")
                {
                    user.FirstTimeOrder = false;
                    user.UpdatedAt = DateTime.UtcNow;
                }

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
                Console.WriteLine($"Error in SimulatePayment: {ex.Message}");
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