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
                .Include(st => st.ExtraServices)
                .OrderBy(st => st.DisplayOrder)
                .Select(st => new ServiceTypeDto
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
                    ExtraServices = st.ExtraServices.Select(es => new ExtraServiceDto
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
                    }).ToList()
                })
                .ToListAsync();

            return Ok(serviceTypes);
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
                Description = serviceType.Description
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
                Description = serviceType.Description
            });
        }

        [HttpDelete("service-types/{id}")]
        public async Task<ActionResult> DeleteServiceType(int id)
        {
            var serviceType = await _context.ServiceTypes.FindAsync(id);
            if (serviceType == null)
                return NotFound();

            serviceType.IsActive = false;
            serviceType.UpdatedAt = DateTime.UtcNow;
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
                    Unit = s.Unit
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
                Unit = service.Unit
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
                Unit = service.Unit
            });
        }

        [HttpDelete("services/{id}")]
        public async Task<ActionResult> DeleteService(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return NotFound();

            service.IsActive = false;
            service.UpdatedAt = DateTime.UtcNow;
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
                    IsAvailableForAll = es.IsAvailableForAll
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
                IsAvailableForAll = extraService.IsAvailableForAll
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
                IsAvailableForAll = extraService.IsAvailableForAll
            });
        }

        [HttpDelete("extra-services/{id}")]
        public async Task<ActionResult> DeleteExtraService(int id)
        {
            var extraService = await _context.ExtraServices.FindAsync(id);
            if (extraService == null)
                return NotFound();

            extraService.IsActive = false;
            extraService.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // Frequencies Management
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