using Microsoft.EntityFrameworkCore;
using DreamCleaningBackend.Data;
using DreamCleaningBackend.DTOs;
using DreamCleaningBackend.Models;
using DreamCleaningBackend.Services.Interfaces;

namespace DreamCleaningBackend.Services
{
    public class SpecialOfferService : ISpecialOfferService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;

        public SpecialOfferService(ApplicationDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<SpecialOfferAdminDto> CreateSpecialOffer(CreateSpecialOfferDto dto, int createdByUserId)
        {
            var offer = new SpecialOffer
            {
                Name = dto.Name,
                Description = dto.Description,
                IsPercentage = dto.IsPercentage,
                DiscountValue = dto.DiscountValue,
                Type = (OfferType)dto.Type,
                ValidFrom = dto.ValidFrom,
                ValidTo = dto.ValidTo,
                Icon = dto.Icon,
                BadgeColor = dto.BadgeColor,
                MinimumOrderAmount = dto.MinimumOrderAmount,
                RequiresFirstTimeCustomer = dto.RequiresFirstTimeCustomer,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = createdByUserId
            };

            _context.SpecialOffers.Add(offer);
            await _context.SaveChangesAsync();

            // Log creation
            await _auditService.LogCreateAsync(offer);

            // If it's not a first-time offer, grant it to eligible users
            if (offer.Type != OfferType.FirstTime)
            {
                await GrantOfferToAllEligibleUsers(offer.Id);
            }

            return await GetSpecialOfferById(offer.Id);
        }

        public async Task<SpecialOfferAdminDto> UpdateSpecialOffer(int id, UpdateSpecialOfferDto dto)
        {
            var offer = await _context.SpecialOffers.FindAsync(id);
            if (offer == null)
                throw new Exception("Special offer not found");

            var originalOffer = new SpecialOffer
            {
                Id = offer.Id,
                Name = offer.Name,
                Description = offer.Description,
                IsPercentage = offer.IsPercentage,
                DiscountValue = offer.DiscountValue,
                IsActive = offer.IsActive
            };

            offer.Name = dto.Name;
            offer.Description = dto.Description;
            offer.IsPercentage = dto.IsPercentage;
            offer.DiscountValue = dto.DiscountValue;
            offer.ValidFrom = dto.ValidFrom;
            offer.ValidTo = dto.ValidTo;
            offer.Icon = dto.Icon;
            offer.BadgeColor = dto.BadgeColor;
            offer.MinimumOrderAmount = dto.MinimumOrderAmount;
            offer.IsActive = dto.IsActive;
            offer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Log update
            await _auditService.LogUpdateAsync(originalOffer, offer);

            return await GetSpecialOfferById(offer.Id);
        }

        public async Task<bool> DeleteSpecialOffer(int id)
        {
            var offer = await _context.SpecialOffers.FindAsync(id);
            if (offer == null)
                return false;

            // Don't allow deletion of first-time offer
            if (offer.Type == OfferType.FirstTime)
                throw new Exception("Cannot delete the first-time customer discount");

            _context.SpecialOffers.Remove(offer);
            await _context.SaveChangesAsync();

            // Log deletion
            await _auditService.LogDeleteAsync(offer);

            return true;
        }

        public async Task<List<SpecialOfferAdminDto>> GetAllSpecialOffers()
        {
            return await _context.SpecialOffers
                .Select(o => new SpecialOfferAdminDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    Description = o.Description,
                    IsPercentage = o.IsPercentage,
                    DiscountValue = o.DiscountValue,
                    Type = o.Type.ToString(),
                    ValidFrom = o.ValidFrom,
                    ValidTo = o.ValidTo,
                    IsActive = o.IsActive,
                    Icon = o.Icon,
                    BadgeColor = o.BadgeColor,
                    CreatedAt = o.CreatedAt,
                    TotalUsersGranted = o.UserSpecialOffers.Count,
                    TimesUsed = o.UserSpecialOffers.Count(uso => uso.IsUsed)
                })
                .OrderBy(o => o.Type)
                .ThenBy(o => o.Name)
                .ToListAsync();
        }

        public async Task<SpecialOfferAdminDto> GetSpecialOfferById(int id)
        {
            return await _context.SpecialOffers
                .Where(o => o.Id == id)
                .Select(o => new SpecialOfferAdminDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    Description = o.Description,
                    IsPercentage = o.IsPercentage,
                    DiscountValue = o.DiscountValue,
                    Type = o.Type.ToString(),
                    ValidFrom = o.ValidFrom,
                    ValidTo = o.ValidTo,
                    IsActive = o.IsActive,
                    Icon = o.Icon,
                    BadgeColor = o.BadgeColor,
                    CreatedAt = o.CreatedAt,
                    TotalUsersGranted = o.UserSpecialOffers.Count,
                    TimesUsed = o.UserSpecialOffers.Count(uso => uso.IsUsed)
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> GrantOfferToAllEligibleUsers(int offerId)
        {
            var offer = await _context.SpecialOffers.FindAsync(offerId);
            if (offer == null || !offer.IsActive)
                return 0;

            // Get all active users who don't have this offer yet
            var eligibleUsers = await _context.Users
                .Where(u => u.IsActive &&
                           !u.UserSpecialOffers.Any(uso => uso.SpecialOfferId == offerId))
                .ToListAsync();

            // Filter by first-time requirement if needed
            if (offer.RequiresFirstTimeCustomer)
            {
                eligibleUsers = eligibleUsers.Where(u => u.FirstTimeOrder).ToList();
            }

            var count = 0;
            foreach (var user in eligibleUsers)
            {
                var userOffer = new UserSpecialOffer
                {
                    UserId = user.Id,
                    SpecialOfferId = offerId,
                    GrantedAt = DateTime.UtcNow,
                    ExpiresAt = offer.ValidTo
                };

                _context.UserSpecialOffers.Add(userOffer);
                count++;
            }

            await _context.SaveChangesAsync();
            return count;
        }

        public async Task<bool> GrantOfferToUser(int offerId, int userId)
        {
            // Check if user already has this offer
            var exists = await _context.UserSpecialOffers
                .AnyAsync(uso => uso.UserId == userId && uso.SpecialOfferId == offerId);

            if (exists)
                return false;

            var offer = await _context.SpecialOffers.FindAsync(offerId);
            if (offer == null || !offer.IsActive)
                return false;

            var userOffer = new UserSpecialOffer
            {
                UserId = userId,
                SpecialOfferId = offerId,
                GrantedAt = DateTime.UtcNow,
                ExpiresAt = offer.ValidTo
            };

            _context.UserSpecialOffers.Add(userOffer);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<UserSpecialOfferDto>> GetUserAvailableOffers(int userId)
        {
            var now = DateTime.UtcNow;

            return await _context.UserSpecialOffers
                .Where(uso => uso.UserId == userId &&
                             !uso.IsUsed &&
                             uso.SpecialOffer.IsActive &&
                             (uso.SpecialOffer.ValidFrom == null || uso.SpecialOffer.ValidFrom <= now) &&
                             (uso.ExpiresAt == null || uso.ExpiresAt > now))
                .Select(uso => new UserSpecialOfferDto
                {
                    Id = uso.Id,
                    SpecialOfferId = uso.SpecialOfferId,
                    Name = uso.SpecialOffer.Name,
                    Description = uso.SpecialOffer.Description,
                    IsPercentage = uso.SpecialOffer.IsPercentage,
                    DiscountValue = uso.SpecialOffer.DiscountValue,
                    ExpiresAt = uso.ExpiresAt,
                    IsUsed = uso.IsUsed,
                    Icon = uso.SpecialOffer.Icon,
                    BadgeColor = uso.SpecialOffer.BadgeColor,
                    MinimumOrderAmount = uso.SpecialOffer.MinimumOrderAmount
                })
                .ToListAsync();
        }

        public async Task<bool> UseSpecialOffer(int userId, int offerId, int orderId)
        {
            var userOffer = await _context.UserSpecialOffers
                .FirstOrDefaultAsync(uso => uso.UserId == userId &&
                                           uso.SpecialOfferId == offerId &&
                                           !uso.IsUsed);

            if (userOffer == null)
                return false;

            userOffer.IsUsed = true;
            userOffer.UsedAt = DateTime.UtcNow;
            userOffer.UsedOnOrderId = orderId;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<UserSpecialOfferDto?> ValidateSpecialOffer(int userId, int offerId)
        {
            var now = DateTime.UtcNow;

            return await _context.UserSpecialOffers
                .Where(uso => uso.UserId == userId &&
                             uso.SpecialOfferId == offerId &&
                             !uso.IsUsed &&
                             uso.SpecialOffer.IsActive &&
                             (uso.SpecialOffer.ValidFrom == null || uso.SpecialOffer.ValidFrom <= now) &&
                             (uso.ExpiresAt == null || uso.ExpiresAt > now))
                .Select(uso => new UserSpecialOfferDto
                {
                    Id = uso.Id,
                    SpecialOfferId = uso.SpecialOfferId,
                    Name = uso.SpecialOffer.Name,
                    Description = uso.SpecialOffer.Description,
                    IsPercentage = uso.SpecialOffer.IsPercentage,
                    DiscountValue = uso.SpecialOffer.DiscountValue,
                    MinimumOrderAmount = uso.SpecialOffer.MinimumOrderAmount
                })
                .FirstOrDefaultAsync();
        }

        public async Task GrantFirstTimeOfferIfEligible(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || !user.FirstTimeOrder)
                return;

            // Find the first-time offer
            var firstTimeOffer = await _context.SpecialOffers
                .FirstOrDefaultAsync(o => o.Type == OfferType.FirstTime && o.IsActive);

            if (firstTimeOffer != null)
            {
                await GrantOfferToUser(firstTimeOffer.Id, userId);
            }
        }

        public async Task<decimal> GetFirstTimeDiscountPercentage()
        {
            var firstTimeOffer = await _context.SpecialOffers
                .FirstOrDefaultAsync(o => o.Type == OfferType.FirstTime && o.IsActive);

            return firstTimeOffer?.DiscountValue ?? 20m; // Default to 20% if not found
        }
    }
}