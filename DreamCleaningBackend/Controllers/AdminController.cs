using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DreamCleaningBackend.Data;
using DreamCleaningBackend.DTOs;
using DreamCleaningBackend.Models;
using DreamCleaningBackend.Services.Interfaces;
using DreamCleaningBackend.Attributes;

namespace DreamCleaningBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IPermissionService _permissionService;
        private readonly IOrderService _orderService;

        public AdminController(ApplicationDbContext context, IPermissionService permissionService, IOrderService orderService)
        {
            _context = context;
            _permissionService = permissionService;
            _orderService = orderService;
        }

        // Service Types Management
        [HttpGet("service-types")]
        [RequirePermission(Permission.View)]
        public async Task<ActionResult<List<ServiceTypeDto>>> GetServiceTypes()
        {
            var serviceTypes = await _context.ServiceTypes
                .Include(st => st.Services)
                .OrderBy(st => st.DisplayOrder)
                .ToListAsync();

            // Get all extra services that are available for all
            var universalExtraServices = await _context.ExtraServices
                .Where(es => es.IsAvailableForAll && es.ServiceTypeId == null)
                .OrderBy(es => es.DisplayOrder)
                .ToListAsync();

            var result = new List<ServiceTypeDto>();

            foreach (var st in serviceTypes)
            {
                // Get extra services specific to this service type
                var specificExtraServices = await _context.ExtraServices
                    .Where(es => es.ServiceTypeId == st.Id && !es.IsAvailableForAll)
                    .OrderBy(es => es.DisplayOrder)
                    .ToListAsync();

                var serviceTypeDto = new ServiceTypeDto
                {
                    Id = st.Id,
                    Name = st.Name,
                    BasePrice = st.BasePrice,
                    Description = st.Description,
                    IsActive = st.IsActive,
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
                    IsAvailableForAll = es.IsAvailableForAll,
                    IsActive = es.IsActive
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
                    IsAvailableForAll = es.IsAvailableForAll,
                    IsActive = es.IsActive
                }));

                result.Add(serviceTypeDto);
            }

            return Ok(result);
        }

        [HttpPost("service-types")]
        [RequirePermission(Permission.Create)]
        public async Task<ActionResult<ServiceTypeDto>> CreateServiceType(CreateServiceTypeDto dto)
        {
            var serviceType = new ServiceType
            {
                Name = dto.Name,
                BasePrice = dto.BasePrice,
                Description = dto.Description,
                DisplayOrder = dto.DisplayOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.ServiceTypes.Add(serviceType);
            await _context.SaveChangesAsync();

            return Ok(new ServiceTypeDto
            {
                Id = serviceType.Id,
                Name = serviceType.Name,
                BasePrice = serviceType.BasePrice,
                Description = serviceType.Description,
                IsActive = serviceType.IsActive
            });
        }

        [HttpPut("service-types/{id}")]
        [RequirePermission(Permission.Update)]
        public async Task<ActionResult<ServiceTypeDto>> UpdateServiceType(int id, UpdateServiceTypeDto dto)
        {
            var serviceType = await _context.ServiceTypes.FindAsync(id);
            if (serviceType == null)
                return NotFound();

            serviceType.Name = dto.Name;
            serviceType.BasePrice = dto.BasePrice;
            serviceType.Description = dto.Description;
            serviceType.DisplayOrder = dto.DisplayOrder;
            serviceType.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ServiceTypeDto
            {
                Id = serviceType.Id,
                Name = serviceType.Name,
                BasePrice = serviceType.BasePrice,
                Description = serviceType.Description,
                IsActive = serviceType.IsActive
            });
        }

        [HttpPut("service-types/{id}/deactivate")]
        [RequirePermission(Permission.Deactivate)]
        public async Task<ActionResult> DeactivateServiceType(int id)
        {
            var serviceType = await _context.ServiceTypes.FindAsync(id);
            if (serviceType == null)
                return NotFound();

            serviceType.IsActive = false;
            serviceType.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("service-types/{id}/activate")]
        [RequirePermission(Permission.Activate)]
        public async Task<ActionResult> ActivateServiceType(int id)
        {
            var serviceType = await _context.ServiceTypes.FindAsync(id);
            if (serviceType == null)
                return NotFound();

            serviceType.IsActive = true;
            serviceType.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("service-types/{id}")]
        [RequirePermission(Permission.Delete)]
        public async Task<ActionResult> DeleteServiceType(int id)
        {
            var serviceType = await _context.ServiceTypes
                .Include(st => st.Services)
                .Include(st => st.ExtraServices)
                .Include(st => st.Orders)
                .FirstOrDefaultAsync(st => st.Id == id);

            if (serviceType == null)
                return NotFound();

            // Check if there are any orders
            if (serviceType.Orders.Any())
            {
                // CHANGED: Return JSON object instead of plain text
                return BadRequest(new { message = "Cannot delete service type with existing orders. Please deactivate instead." });
            }

            _context.ServiceTypes.Remove(serviceType);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // Services Management
        [HttpGet("services")]
        [RequirePermission(Permission.View)]
        public async Task<ActionResult<List<ServiceDto>>> GetServices()
        {
            var services = await _context.Services
                .OrderBy(s => s.ServiceTypeId)
                .ThenBy(s => s.DisplayOrder)
                .Select(s => new ServiceDto
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
                    IsActive = s.IsActive
                })
                .ToListAsync();

            return Ok(services);
        }

        [HttpPost("services")]
        [RequirePermission(Permission.Create)]
        public async Task<ActionResult<ServiceDto>> CreateService(CreateServiceDto dto)
        {
            var service = new Service
            {
                Name = dto.Name,
                ServiceKey = dto.ServiceKey,
                Cost = dto.Cost,
                TimeDuration = dto.TimeDuration,
                ServiceTypeId = dto.ServiceTypeId,
                InputType = dto.InputType,
                MinValue = dto.MinValue,
                MaxValue = dto.MaxValue,
                StepValue = dto.StepValue,
                IsRangeInput = dto.IsRangeInput,
                Unit = dto.Unit,
                ServiceRelationType = dto.ServiceRelationType, // ADD THIS
                DisplayOrder = dto.DisplayOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return Ok(new ServiceDto
            {
                Id = service.Id,
                Name = service.Name,
                ServiceKey = service.ServiceKey,
                Cost = service.Cost,
                TimeDuration = service.TimeDuration,
                ServiceTypeId = service.ServiceTypeId,
                InputType = service.InputType,
                MinValue = service.MinValue,
                MaxValue = service.MaxValue,
                StepValue = service.StepValue,
                IsRangeInput = service.IsRangeInput,
                Unit = service.Unit,
                ServiceRelationType = service.ServiceRelationType, // ADD THIS
                IsActive = service.IsActive
            });
        }

        [HttpPost("services/copy")]
        [RequirePermission(Permission.Create)]
        public async Task<ActionResult<ServiceDto>> CopyService(CopyServiceDto dto)
        {
            var sourceService = await _context.Services.FindAsync(dto.SourceServiceId);
            if (sourceService == null)
                return NotFound("Source service not found");

            var newService = new Service
            {
                Name = sourceService.Name,
                ServiceKey = sourceService.ServiceKey,
                Cost = sourceService.Cost,
                TimeDuration = sourceService.TimeDuration,
                ServiceTypeId = dto.TargetServiceTypeId,
                InputType = sourceService.InputType,
                MinValue = sourceService.MinValue,
                MaxValue = sourceService.MaxValue,
                StepValue = sourceService.StepValue,
                IsRangeInput = sourceService.IsRangeInput,
                Unit = sourceService.Unit,
                ServiceRelationType = sourceService.ServiceRelationType, // ADD THIS
                DisplayOrder = sourceService.DisplayOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Services.Add(newService);
            await _context.SaveChangesAsync();

            return Ok(new ServiceDto
            {
                Id = newService.Id,
                Name = newService.Name,
                ServiceKey = newService.ServiceKey,
                Cost = newService.Cost,
                TimeDuration = newService.TimeDuration,
                ServiceTypeId = newService.ServiceTypeId,
                InputType = newService.InputType,
                MinValue = newService.MinValue,
                MaxValue = newService.MaxValue,
                StepValue = newService.StepValue,
                IsRangeInput = newService.IsRangeInput,
                Unit = newService.Unit,
                ServiceRelationType = newService.ServiceRelationType, // ADD THIS
                IsActive = newService.IsActive
            });
        }

        [HttpPut("services/{id}")]
        [RequirePermission(Permission.Update)]
        public async Task<ActionResult<ServiceDto>> UpdateService(int id, UpdateServiceDto dto)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return NotFound();

            service.Name = dto.Name;
            service.ServiceKey = dto.ServiceKey;
            service.Cost = dto.Cost;
            service.TimeDuration = dto.TimeDuration;
            service.ServiceTypeId = dto.ServiceTypeId;
            service.InputType = dto.InputType;
            service.MinValue = dto.MinValue;
            service.MaxValue = dto.MaxValue;
            service.StepValue = dto.StepValue;
            service.IsRangeInput = dto.IsRangeInput;
            service.Unit = dto.Unit;
            service.ServiceRelationType = dto.ServiceRelationType; // ADD THIS
            service.DisplayOrder = dto.DisplayOrder;
            service.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ServiceDto
            {
                Id = service.Id,
                Name = service.Name,
                ServiceKey = service.ServiceKey,
                Cost = service.Cost,
                TimeDuration = service.TimeDuration,
                ServiceTypeId = service.ServiceTypeId,
                InputType = service.InputType,
                MinValue = service.MinValue,
                MaxValue = service.MaxValue,
                StepValue = service.StepValue,
                IsRangeInput = service.IsRangeInput,
                Unit = service.Unit,
                ServiceRelationType = service.ServiceRelationType, // ADD THIS
                IsActive = service.IsActive
            });
        }

        [HttpPut("services/{id}/deactivate")]
        [RequirePermission(Permission.Deactivate)]
        public async Task<ActionResult> DeactivateService(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return NotFound();

            service.IsActive = false;
            service.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("services/{id}/activate")]
        [RequirePermission(Permission.Activate)]
        public async Task<ActionResult> ActivateService(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return NotFound();

            service.IsActive = true;
            service.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("services/{id}")]
        [RequirePermission(Permission.Delete)]
        public async Task<ActionResult> DeleteService(int id)
        {
            var service = await _context.Services
                .Include(s => s.OrderServices)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
                return NotFound();

            // Check if there are any orders using this service
            if (service.OrderServices.Any())
            {
                // CHANGED: Return JSON object instead of plain text
                return BadRequest(new { message = "Cannot delete service with existing orders. Please deactivate instead." });
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("extra-services")]
        [RequirePermission(Permission.View)]
        public async Task<ActionResult<List<ExtraServiceDto>>> GetExtraServices()
        {
            var extraServices = await _context.ExtraServices
                .OrderBy(es => es.DisplayOrder)
                .Select(es => new ExtraServiceDto
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
                    IsAvailableForAll = es.IsAvailableForAll,
                    IsActive = es.IsActive
                })
                .ToListAsync();

            return Ok(extraServices);
        }

        [HttpPost("extra-services")]
        [RequirePermission(Permission.Create)]
        public async Task<ActionResult<ExtraServiceDto>> CreateExtraService(CreateExtraServiceDto dto)
        {
            var extraService = new ExtraService
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Duration = dto.Duration,
                Icon = dto.Icon,
                HasQuantity = dto.HasQuantity,
                HasHours = dto.HasHours,
                IsDeepCleaning = dto.IsDeepCleaning,
                IsSuperDeepCleaning = dto.IsSuperDeepCleaning,
                IsSameDayService = dto.IsSameDayService,
                PriceMultiplier = dto.PriceMultiplier,
                ServiceTypeId = dto.ServiceTypeId,
                IsAvailableForAll = dto.IsAvailableForAll,
                DisplayOrder = dto.DisplayOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.ExtraServices.Add(extraService);
            await _context.SaveChangesAsync();

            return Ok(new ExtraServiceDto
            {
                Id = extraService.Id,
                Name = extraService.Name,
                Description = extraService.Description,
                Price = extraService.Price,
                Duration = extraService.Duration,
                Icon = extraService.Icon,
                HasQuantity = extraService.HasQuantity,
                HasHours = extraService.HasHours,
                IsDeepCleaning = extraService.IsDeepCleaning,
                IsSuperDeepCleaning = extraService.IsSuperDeepCleaning,
                IsSameDayService = extraService.IsSameDayService,
                PriceMultiplier = extraService.PriceMultiplier,
                IsAvailableForAll = extraService.IsAvailableForAll,
                IsActive = extraService.IsActive
            });
        }

        [HttpPost("extra-services/copy")]
        [RequirePermission(Permission.Create)]
        public async Task<ActionResult<ExtraServiceDto>> CopyExtraService(CopyExtraServiceDto dto)
        {
            var sourceExtraService = await _context.ExtraServices.FindAsync(dto.SourceExtraServiceId);
            if (sourceExtraService == null)
                return NotFound("Source extra service not found");

            var newExtraService = new ExtraService
            {
                Name = sourceExtraService.Name,
                Description = sourceExtraService.Description,
                Price = sourceExtraService.Price,
                Duration = sourceExtraService.Duration,
                Icon = sourceExtraService.Icon,
                HasQuantity = sourceExtraService.HasQuantity,
                HasHours = sourceExtraService.HasHours,
                IsDeepCleaning = sourceExtraService.IsDeepCleaning,
                IsSuperDeepCleaning = sourceExtraService.IsSuperDeepCleaning,
                IsSameDayService = sourceExtraService.IsSameDayService,
                PriceMultiplier = sourceExtraService.PriceMultiplier,
                ServiceTypeId = dto.TargetServiceTypeId,
                IsAvailableForAll = false, // When copying to specific service type
                DisplayOrder = sourceExtraService.DisplayOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.ExtraServices.Add(newExtraService);
            await _context.SaveChangesAsync();

            return Ok(new ExtraServiceDto
            {
                Id = newExtraService.Id,
                Name = newExtraService.Name,
                Description = newExtraService.Description,
                Price = newExtraService.Price,
                Duration = newExtraService.Duration,
                Icon = newExtraService.Icon,
                HasQuantity = newExtraService.HasQuantity,
                HasHours = newExtraService.HasHours,
                IsDeepCleaning = newExtraService.IsDeepCleaning,
                IsSuperDeepCleaning = newExtraService.IsSuperDeepCleaning,
                IsSameDayService = newExtraService.IsSameDayService,
                PriceMultiplier = newExtraService.PriceMultiplier,
                IsAvailableForAll = newExtraService.IsAvailableForAll,
                IsActive = newExtraService.IsActive
            });
        }

        [HttpPut("extra-services/{id}")]
        [RequirePermission(Permission.Update)]
        public async Task<ActionResult<ExtraServiceDto>> UpdateExtraService(int id, UpdateExtraServiceDto dto)
        {
            var extraService = await _context.ExtraServices.FindAsync(id);
            if (extraService == null)
                return NotFound();

            extraService.Name = dto.Name;
            extraService.Description = dto.Description;
            extraService.Price = dto.Price;
            extraService.Duration = dto.Duration;
            extraService.Icon = dto.Icon;
            extraService.HasQuantity = dto.HasQuantity;
            extraService.HasHours = dto.HasHours;
            extraService.IsDeepCleaning = dto.IsDeepCleaning;
            extraService.IsSuperDeepCleaning = dto.IsSuperDeepCleaning;
            extraService.IsSameDayService = dto.IsSameDayService;
            extraService.PriceMultiplier = dto.PriceMultiplier;
            extraService.ServiceTypeId = dto.ServiceTypeId;
            extraService.IsAvailableForAll = dto.IsAvailableForAll;
            extraService.DisplayOrder = dto.DisplayOrder;
            extraService.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ExtraServiceDto
            {
                Id = extraService.Id,
                Name = extraService.Name,
                Description = extraService.Description,
                Price = extraService.Price,
                Duration = extraService.Duration,
                Icon = extraService.Icon,
                HasQuantity = extraService.HasQuantity,
                HasHours = extraService.HasHours,
                IsDeepCleaning = extraService.IsDeepCleaning,
                IsSuperDeepCleaning = extraService.IsSuperDeepCleaning,
                IsSameDayService = extraService.IsSameDayService,
                PriceMultiplier = extraService.PriceMultiplier,
                IsAvailableForAll = extraService.IsAvailableForAll,
                IsActive = extraService.IsActive
            });
        }

        [HttpPut("extra-services/{id}/deactivate")]
        [RequirePermission(Permission.Deactivate)]
        public async Task<ActionResult> DeactivateExtraService(int id)
        {
            var extraService = await _context.ExtraServices.FindAsync(id);
            if (extraService == null)
                return NotFound();

            extraService.IsActive = false;
            extraService.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("extra-services/{id}/activate")]
        [RequirePermission(Permission.Activate)]
        public async Task<ActionResult> ActivateExtraService(int id)
        {
            var extraService = await _context.ExtraServices.FindAsync(id);
            if (extraService == null)
                return NotFound();

            extraService.IsActive = true;
            extraService.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("extra-services/{id}")]
        [RequirePermission(Permission.Delete)]
        public async Task<ActionResult> DeleteExtraService(int id)
        {
            var extraService = await _context.ExtraServices
                .Include(es => es.OrderExtraServices)
                .FirstOrDefaultAsync(es => es.Id == id);

            if (extraService == null)
                return NotFound();

            // Check if there are any orders using this extra service
            if (extraService.OrderExtraServices.Any())
            {
                // CHANGED: Return JSON object instead of plain text
                return BadRequest(new { message = "Cannot delete extra service with existing orders. Please deactivate instead." });
            }

            _context.ExtraServices.Remove(extraService);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // Subscriptions Management
        [HttpGet("subscriptions")]
        [RequirePermission(Permission.View)]
        public async Task<ActionResult<List<SubscriptionDto>>> GetSubscriptions()
        {
            var subscriptions = await _context.Subscriptions
                .OrderBy(s => s.DisplayOrder)
                .Select(s => new SubscriptionDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    DiscountPercentage = s.DiscountPercentage,
                    SubscriptionDays = s.SubscriptionDays,
                    IsActive = s.IsActive
                })
                .ToListAsync();
            return Ok(subscriptions);
        }

        [HttpPost("subscriptions")]
        [RequirePermission(Permission.Create)]
        public async Task<ActionResult<SubscriptionDto>> CreateSubscription(CreateSubscriptionDto dto)
        {
            var subscription = new Subscription
            {
                Name = dto.Name,
                Description = dto.Description,
                DiscountPercentage = dto.DiscountPercentage,
                SubscriptionDays = dto.SubscriptionDays,
                DisplayOrder = dto.DisplayOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            return Ok(new SubscriptionDto
            {
                Id = subscription.Id,
                Name = subscription.Name,
                Description = subscription.Description,
                DiscountPercentage = subscription.DiscountPercentage,
                SubscriptionDays = subscription.SubscriptionDays
            });
        }

        [HttpPut("subscriptions/{id}")]
        [RequirePermission(Permission.Update)]
        public async Task<ActionResult<SubscriptionDto>> UpdateSubscription(int id, UpdateSubscriptionDto dto)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null)
                return NotFound();

            subscription.Name = dto.Name;
            subscription.Description = dto.Description;
            subscription.DiscountPercentage = dto.DiscountPercentage;
            subscription.SubscriptionDays = dto.SubscriptionDays;
            subscription.DisplayOrder = dto.DisplayOrder;
            subscription.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new SubscriptionDto
            {
                Id = subscription.Id,
                Name = subscription.Name,
                Description = subscription.Description,
                DiscountPercentage = subscription.DiscountPercentage,
                SubscriptionDays = subscription.SubscriptionDays
            });
        }

        [HttpDelete("subscriptions/{id}")]
        [RequirePermission(Permission.Delete)]
        public async Task<ActionResult> DeleteSubscription(int id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null)
                return NotFound();

            subscription.IsActive = false;
            subscription.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("subscriptions/{id}/deactivate")]
        [RequirePermission(Permission.Deactivate)]
        public async Task<ActionResult> DeactivateSubscription(int id)
        {
            try
            {
                var subscription = await _context.Subscriptions.FindAsync(id);
                if (subscription == null)
                {
                    return NotFound(new { message = "Subscription not found" });
                }

                subscription.IsActive = false;
                subscription.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Subscription deactivated successfully", subscription });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deactivating subscription", error = ex.Message });
            }
        }

        [HttpPost("subscriptions/{id}/activate")]
        [RequirePermission(Permission.Activate)]
        public async Task<ActionResult> ActivateSubscription(int id)
        {
            try
            {
                var subscription = await _context.Subscriptions.FindAsync(id);
                if (subscription == null)
                {
                    return NotFound(new { message = "Subscription not found" });
                }

                subscription.IsActive = true;
                subscription.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Subscription activated successfully", subscription });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error activating subscription", error = ex.Message });
            }
        }


        // Promo Codes Management (keeping existing)
        [HttpGet("promo-codes")]
        [RequirePermission(Permission.View)]
        public async Task<ActionResult<List<PromoCodeDto>>> GetPromoCodes()
        {
            var promoCodes = await _context.PromoCodes
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PromoCodeDto
                {
                    Id = p.Id,
                    Code = p.Code,
                    Description = p.Description,
                    IsPercentage = p.IsPercentage,
                    DiscountValue = p.DiscountValue,
                    MaxUsageCount = p.MaxUsageCount,
                    CurrentUsageCount = p.CurrentUsageCount,
                    MaxUsagePerUser = p.MaxUsagePerUser,
                    ValidFrom = p.ValidFrom,
                    ValidTo = p.ValidTo,
                    MinimumOrderAmount = p.MinimumOrderAmount,
                    IsActive = p.IsActive
                })
                .ToListAsync();

            return Ok(promoCodes);
        }

        [HttpPost("promo-codes")]
        [RequirePermission(Permission.Create)]
        public async Task<ActionResult<PromoCodeDto>> CreatePromoCode(CreatePromoCodeDto dto)
        {
            var promoCode = new PromoCode
            {
                Code = dto.Code.ToUpper(),
                Description = dto.Description,
                IsPercentage = dto.IsPercentage,
                DiscountValue = dto.DiscountValue,
                MaxUsageCount = dto.MaxUsageCount,
                MaxUsagePerUser = dto.MaxUsagePerUser,
                ValidFrom = dto.ValidFrom,
                ValidTo = dto.ValidTo,
                MinimumOrderAmount = dto.MinimumOrderAmount,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.PromoCodes.Add(promoCode);
            await _context.SaveChangesAsync();

            return Ok(new PromoCodeDto
            {
                Id = promoCode.Id,
                Code = promoCode.Code,
                Description = promoCode.Description,
                IsPercentage = promoCode.IsPercentage,
                DiscountValue = promoCode.DiscountValue,
                MaxUsageCount = promoCode.MaxUsageCount,
                CurrentUsageCount = promoCode.CurrentUsageCount,
                MaxUsagePerUser = promoCode.MaxUsagePerUser,
                ValidFrom = promoCode.ValidFrom,
                ValidTo = promoCode.ValidTo,
                MinimumOrderAmount = promoCode.MinimumOrderAmount,
                IsActive = promoCode.IsActive
            });
        }

        [HttpPut("promo-codes/{id}")]
        [RequirePermission(Permission.Update)]
        public async Task<ActionResult<PromoCodeDto>> UpdatePromoCode(int id, UpdatePromoCodeDto dto)
        {
            var promoCode = await _context.PromoCodes.FindAsync(id);
            if (promoCode == null)
                return NotFound();

            promoCode.Description = dto.Description;
            promoCode.IsPercentage = dto.IsPercentage;
            promoCode.DiscountValue = dto.DiscountValue;
            promoCode.MaxUsageCount = dto.MaxUsageCount;
            promoCode.MaxUsagePerUser = dto.MaxUsagePerUser;
            promoCode.ValidFrom = dto.ValidFrom;
            promoCode.ValidTo = dto.ValidTo;
            promoCode.MinimumOrderAmount = dto.MinimumOrderAmount;
            promoCode.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new PromoCodeDto
            {
                Id = promoCode.Id,
                Code = promoCode.Code,
                Description = promoCode.Description,
                IsPercentage = promoCode.IsPercentage,
                DiscountValue = promoCode.DiscountValue,
                MaxUsageCount = promoCode.MaxUsageCount,
                CurrentUsageCount = promoCode.CurrentUsageCount,
                MaxUsagePerUser = promoCode.MaxUsagePerUser,
                ValidFrom = promoCode.ValidFrom,
                ValidTo = promoCode.ValidTo,
                MinimumOrderAmount = promoCode.MinimumOrderAmount,
                IsActive = promoCode.IsActive
            });
        }

        [HttpDelete("promo-codes/{id}")]
        [RequirePermission(Permission.Delete)]
        public async Task<ActionResult> DeletePromoCode(int id)
        {
            var promoCode = await _context.PromoCodes.FindAsync(id);
            if (promoCode == null)
                return NotFound();

            _context.PromoCodes.Remove(promoCode);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("promo-codes/{id}/deactivate")]
        [RequirePermission(Permission.Deactivate)]
        public async Task<ActionResult> DeactivatePromoCode(int id)
        {
            try
            {
                var promoCode = await _context.PromoCodes.FindAsync(id);
                if (promoCode == null)
                {
                    return NotFound(new { message = "PromoCode not found" });
                }

                promoCode.IsActive = false;
                promoCode.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "PromoCode deactivated successfully", promoCode });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deactivating promocode", error = ex.Message });
            }
        }

        [HttpPost("promo-codes/{id}/activate")]
        [RequirePermission(Permission.Activate)]
        public async Task<ActionResult> ActivatePromoCode(int id)
        {
            try
            {
                var promoCode = await _context.PromoCodes.FindAsync(id);
                if (promoCode == null)
                {
                    return NotFound(new { message = "PromoCode not found" });
                }

                promoCode.IsActive = true;
                promoCode.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "PromoCode activated successfully", promoCode });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error activating promocode", error = ex.Message });
            }
        }

        // Users Management (keeping existing)
        [HttpGet("users")]
        [RequirePermission(Permission.View)]
        public async Task<ActionResult<List<UserAdminDto>>> GetUsers()
        {
            var currentUserRole = GetCurrentUserRole();

            var users = await _context.Users
                .Include(u => u.Subscription)
                .Select(u => new UserAdminDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Role = u.Role.ToString(),
                    AuthProvider = u.AuthProvider,
                    SubscriptionName = u.Subscription != null ? u.Subscription.Name : null,
                    FirstTimeOrder = u.FirstTimeOrder,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            // Include current user role in response for frontend to use
            return Ok(new
            {
                users = users,
                currentUserRole = currentUserRole.ToString()
            });

        }

        [HttpPut("users/{id}/role")]
        [RequirePermission(Permission.Update)]
        public async Task<ActionResult> UpdateUserRole(int id, UpdateUserRoleDto dto)
        {
            // Get current user's role
            var currentUserRole = GetCurrentUserRole();

            // Get the target user
            var targetUser = await _context.Users.FindAsync(id);
            if (targetUser == null)
                return NotFound();

            // Parse the new role
            if (!Enum.TryParse<UserRole>(dto.Role, out var newRole))
                return BadRequest("Invalid role");

            // Apply role change restrictions
            var validationResult = ValidateRoleChange(currentUserRole, targetUser.Role, newRole);
            if (!validationResult.IsValid)
                return BadRequest(new { message = validationResult.ErrorMessage });

            // Update the role
            targetUser.Role = newRole;
            targetUser.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Role updated successfully" });
        }

        private UserRole GetCurrentUserRole()
        {
            var roleClaim = User.FindFirst("Role")?.Value;
            Enum.TryParse<UserRole>(roleClaim, out var role);
            return role;
        }

        private (bool IsValid, string ErrorMessage) ValidateRoleChange(UserRole currentUserRole, UserRole targetCurrentRole, UserRole newRole)
        {
            // Moderators cannot change roles at all (they don't have Update permission, but double-check)
            if (currentUserRole == UserRole.Moderator)
                return (false, "Moderators cannot change user roles");

            // Admins cannot assign SuperAdmin role
            if (currentUserRole == UserRole.Admin && newRole == UserRole.SuperAdmin)
                return (false, "Admins cannot assign SuperAdmin role");

            // Admins cannot remove SuperAdmin role from a SuperAdmin
            if (currentUserRole == UserRole.Admin && targetCurrentRole == UserRole.SuperAdmin)
                return (false, "Admins cannot modify SuperAdmin users");

            // Users cannot demote themselves from SuperAdmin (optional safety check)
            var currentUserId = User.FindFirst("UserId")?.Value;
            if (currentUserId != null && targetCurrentRole == UserRole.SuperAdmin && newRole != UserRole.SuperAdmin)
            {
                // Check if user is trying to demote themselves
                // This is optional - you may want to allow SuperAdmins to demote themselves
                // return (false, "Cannot remove your own SuperAdmin role");
            }

            return (true, string.Empty);
        }

        [HttpPut("users/{id}/status")]
        [RequirePermission(Permission.Update)]
        public async Task<ActionResult> UpdateUserStatus(int id, UpdateUserStatusDto dto)
        {
            var targetUser = await _context.Users.FindAsync(id);
            if (targetUser == null)
                return NotFound();

            var currentUserRole = GetCurrentUserRole();
            var targetUserRole = targetUser.Role;

            // Prevent admins from deactivating/activating SuperAdmins
            if (currentUserRole == UserRole.Admin && targetUserRole == UserRole.SuperAdmin)
            {
                return BadRequest(new { message = "Admins cannot modify SuperAdmin status" });
            }

            // Optional: Prevent users from deactivating themselves
            var currentUserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            if (currentUserId == id && !dto.IsActive)
            {
                return BadRequest(new { message = "You cannot deactivate yourself" });
            }

            targetUser.IsActive = dto.IsActive;
            targetUser.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("permissions")]
        [Authorize]
        public ActionResult<object> GetUserPermissions()
        {
            var roleClaim = User.FindFirst("Role")?.Value;
            if (!Enum.TryParse<UserRole>(roleClaim, out var userRole))
            {
                return BadRequest("Invalid role");
            }

            return Ok(new
            {
                role = userRole.ToString(),
                permissions = new
                {
                    canView = _permissionService.CanView(userRole),
                    canCreate = _permissionService.CanCreate(userRole),
                    canUpdate = _permissionService.CanUpdate(userRole),
                    canDelete = _permissionService.CanDelete(userRole),
                    canActivate = _permissionService.CanActivate(userRole),
                    canDeactivate = _permissionService.CanDeactivate(userRole)
                }
            });
        }

        // Orders Management
        [HttpGet("orders")]
        [RequirePermission(Permission.View)]
        public async Task<ActionResult<List<OrderListDto>>> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersForAdmin();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpGet("orders/{orderId}")]
        [RequirePermission(Permission.View)]
        public async Task<ActionResult<OrderDto>> GetOrderDetails(int orderId)
        {
            try
            {
                // For admin, we don't need to check userId
                var order = await _context.Orders
                    .Include(o => o.ServiceType)
                    .Include(o => o.Subscription)
                    .Include(o => o.OrderServices)
                        .ThenInclude(os => os.Service)
                    .Include(o => o.OrderExtraServices)
                        .ThenInclude(oes => oes.ExtraService)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                    return NotFound();

                return new OrderDto
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    ServiceTypeId = order.ServiceTypeId,
                    ServiceTypeName = order.ServiceType?.Name ?? "",
                    OrderDate = order.OrderDate,
                    ServiceDate = order.ServiceDate,
                    ServiceTime = order.ServiceTime,
                    Status = order.Status,
                    SubTotal = order.SubTotal,
                    Tax = order.Tax,
                    Tips = order.Tips,
                    Total = order.Total,
                    DiscountAmount = order.DiscountAmount,
                    SubscriptionDiscountAmount = order.SubscriptionDiscountAmount,
                    PromoCode = order.PromoCode,
                    SubscriptionId = order.SubscriptionId,
                    SubscriptionName = order.Subscription?.Name ?? "",
                    EntryMethod = order.EntryMethod,
                    SpecialInstructions = order.SpecialInstructions,
                    ContactFirstName = order.ContactFirstName,
                    ContactLastName = order.ContactLastName,
                    ContactEmail = order.ContactEmail,
                    ContactPhone = order.ContactPhone,
                    ServiceAddress = order.ServiceAddress,
                    AptSuite = order.AptSuite,
                    City = order.City,
                    State = order.State,
                    ZipCode = order.ZipCode,
                    TotalDuration = order.TotalDuration,
                    MaidsCount = order.MaidsCount,
                    IsPaid = order.IsPaid,
                    PaidAt = order.PaidAt,
                    Services = order.OrderServices?.Select(os => new OrderServiceDto
                    {
                        Id = os.Id,
                        ServiceId = os.ServiceId,
                        ServiceName = os.Service?.Name ?? "",
                        Quantity = os.Quantity,
                        Cost = os.Cost,
                        Duration = os.Duration
                    }).ToList() ?? new List<OrderServiceDto>(),
                    ExtraServices = order.OrderExtraServices?.Select(oes => new OrderExtraServiceDto
                    {
                        Id = oes.Id,
                        ExtraServiceId = oes.ExtraServiceId,
                        ExtraServiceName = oes.ExtraService?.Name ?? "",
                        Quantity = oes.Quantity,
                        Hours = oes.Hours,
                        Cost = oes.Cost,
                        Duration = oes.Duration
                    }).ToList() ?? new List<OrderExtraServiceDto>()
                };
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("orders/{orderId}/status")]
        [RequirePermission(Permission.Update)]
        public async Task<ActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDto dto)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                    return NotFound();

                // No special role check needed - any user with Update permission can change status
                order.Status = dto.Status;
                order.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return Ok(new { message = $"Order status updated to {dto.Status}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("orders/{orderId}/cancel")]
        [RequirePermission(Permission.Update)]  // Using Update permission
        public async Task<ActionResult> CancelOrder(int orderId, [FromBody] CancelOrderDto dto)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                    return NotFound();

                // Use proper casing for status values
                if (order.Status == "Cancelled" || order.Status == "Done")
                    return BadRequest(new { message = "Cannot cancel an order that is already cancelled or done." });

                order.Status = "Cancelled";  // Capital C to match enum
                order.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return Ok(new { message = "Order cancelled successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}