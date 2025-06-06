using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DreamCleaningBackend.Data;
using DreamCleaningBackend.DTOs;
using DreamCleaningBackend.Models;

namespace DreamCleaningBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Service Types Management
        [HttpGet("service-types")]
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
                return BadRequest("Cannot delete service type with existing orders. Please deactivate instead.");
            }

            // Delete related services and extra services
            _context.Services.RemoveRange(serviceType.Services);
            _context.ExtraServices.RemoveRange(serviceType.ExtraServices);
            _context.ServiceTypes.Remove(serviceType);

            await _context.SaveChangesAsync();

            return Ok();
        }

        // Services Management
        [HttpGet("services")]
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
                return BadRequest("Cannot delete service with existing orders. Please deactivate instead.");
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // Extra Services Management
        [HttpGet("extra-services")]
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
                return BadRequest("Cannot delete extra service with existing orders. Please deactivate instead.");
            }

            _context.ExtraServices.Remove(extraService);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // Frequencies Management (rest of the code remains the same)
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

        [HttpPost("frequencies")]
        public async Task<ActionResult<FrequencyDto>> CreateFrequency(CreateFrequencyDto dto)
        {
            var frequency = new Frequency
            {
                Name = dto.Name,
                Description = dto.Description,
                DiscountPercentage = dto.DiscountPercentage,
                FrequencyDays = dto.FrequencyDays,
                DisplayOrder = dto.DisplayOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Frequencies.Add(frequency);
            await _context.SaveChangesAsync();

            return Ok(new FrequencyDto
            {
                Id = frequency.Id,
                Name = frequency.Name,
                Description = frequency.Description,
                DiscountPercentage = frequency.DiscountPercentage,
                FrequencyDays = frequency.FrequencyDays
            });
        }

        [HttpPut("frequencies/{id}")]
        public async Task<ActionResult<FrequencyDto>> UpdateFrequency(int id, UpdateFrequencyDto dto)
        {
            var frequency = await _context.Frequencies.FindAsync(id);
            if (frequency == null)
                return NotFound();

            frequency.Name = dto.Name;
            frequency.Description = dto.Description;
            frequency.DiscountPercentage = dto.DiscountPercentage;
            frequency.FrequencyDays = dto.FrequencyDays;
            frequency.DisplayOrder = dto.DisplayOrder;
            frequency.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new FrequencyDto
            {
                Id = frequency.Id,
                Name = frequency.Name,
                Description = frequency.Description,
                DiscountPercentage = frequency.DiscountPercentage,
                FrequencyDays = frequency.FrequencyDays
            });
        }

        [HttpDelete("frequencies/{id}")]
        public async Task<ActionResult> DeleteFrequency(int id)
        {
            var frequency = await _context.Frequencies.FindAsync(id);
            if (frequency == null)
                return NotFound();

            frequency.IsActive = false;
            frequency.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // Promo Codes Management (keeping existing)
        [HttpGet("promo-codes")]
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
            promoCode.IsActive = dto.IsActive;
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
        public async Task<ActionResult> DeletePromoCode(int id)
        {
            var promoCode = await _context.PromoCodes.FindAsync(id);
            if (promoCode == null)
                return NotFound();

            _context.PromoCodes.Remove(promoCode);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // Users Management (keeping existing)
        [HttpGet("users")]
        public async Task<ActionResult<List<UserAdminDto>>> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.Subscription)
                .OrderByDescending(u => u.CreatedAt)
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

            return Ok(users);
        }

        [HttpPut("users/{id}/role")]
        public async Task<ActionResult> UpdateUserRole(int id, UpdateUserRoleDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            if (Enum.TryParse<UserRole>(dto.Role, out var role))
            {
                user.Role = role;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return Ok();
            }

            return BadRequest("Invalid role");
        }

        [HttpPut("users/{id}/status")]
        public async Task<ActionResult> UpdateUserStatus(int id, UpdateUserStatusDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            user.IsActive = dto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}