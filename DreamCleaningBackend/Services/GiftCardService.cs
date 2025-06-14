﻿using Microsoft.EntityFrameworkCore;
using DreamCleaningBackend.Data;
using DreamCleaningBackend.DTOs;
using DreamCleaningBackend.Models;
using DreamCleaningBackend.Services.Interfaces;
using System.Text;

namespace DreamCleaningBackend.Services
{
    public class GiftCardService : IGiftCardService
    {
        private readonly ApplicationDbContext _context;

        public GiftCardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GenerateUniqueGiftCardCode()
        {
            string code;
            bool exists;

            do
            {
                code = GenerateRandomCode();
                exists = _context.GiftCards.Any(g => g.Code == code);
            }
            while (exists);

            return code;
        }

        private string GenerateRandomCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new StringBuilder();

            for (int i = 0; i < 12; i++)
            {
                if (i > 0 && i % 4 == 0)
                {
                    result.Append('-');
                }
                result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString();
        }

        public async Task<GiftCard> CreateGiftCard(int userId, CreateGiftCardDto createDto)
        {
            var giftCard = new GiftCard
            {
                Code = GenerateUniqueGiftCardCode(),
                OriginalAmount = createDto.Amount,
                CurrentBalance = createDto.Amount,
                RecipientName = createDto.RecipientName,
                RecipientEmail = createDto.RecipientEmail,
                SenderName = createDto.SenderName,
                SenderEmail = createDto.SenderEmail,
                Message = createDto.Message,
                PurchasedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.GiftCards.Add(giftCard);
            await _context.SaveChangesAsync();

            return giftCard;
        }

        public async Task<GiftCardValidationDto> ValidateGiftCard(string code)
        {
            var giftCard = await _context.GiftCards
                .FirstOrDefaultAsync(g => g.Code == code);

            if (giftCard == null)
            {
                return new GiftCardValidationDto
                {
                    IsValid = false,
                    AvailableBalance = 0,
                    Message = "Invalid gift card code"
                };
            }

            if (!giftCard.IsActive)
            {
                return new GiftCardValidationDto
                {
                    IsValid = false,
                    AvailableBalance = 0,
                    Message = "This gift card has been deactivated"
                };
            }

            if (!giftCard.IsPaid)
            {
                return new GiftCardValidationDto
                {
                    IsValid = false,
                    AvailableBalance = 0,
                    Message = "This gift card payment is still pending"
                };
            }

            if (giftCard.CurrentBalance <= 0)
            {
                return new GiftCardValidationDto
                {
                    IsValid = false,
                    AvailableBalance = 0,
                    Message = "This gift card has been fully used"
                };
            }

            return new GiftCardValidationDto
            {
                IsValid = true,
                AvailableBalance = giftCard.CurrentBalance,
                RecipientName = giftCard.RecipientName
            };
        }

        public async Task<decimal> ApplyGiftCardToOrder(string code, decimal orderAmount, int orderId, int userId)
        {
            var giftCard = await _context.GiftCards
                .FirstOrDefaultAsync(g => g.Code == code);

            if (giftCard == null || !giftCard.IsActive || !giftCard.IsPaid || giftCard.CurrentBalance <= 0)
            {
                throw new InvalidOperationException("Invalid or unusable gift card");
            }

            // Calculate the amount to apply (minimum of gift card balance and order amount)
            var amountToApply = Math.Min(giftCard.CurrentBalance, orderAmount);

            Console.WriteLine($"=== GIFT CARD SERVICE DEBUG ===");
            Console.WriteLine($"Gift Card Current Balance: {giftCard.CurrentBalance}");
            Console.WriteLine($"Order Amount: {orderAmount}");
            Console.WriteLine($"Amount to Apply: {amountToApply}");

            // Update gift card balance
            giftCard.CurrentBalance -= amountToApply;
            giftCard.UpdatedAt = DateTime.UtcNow;

            // Create usage record with UserId
            var usage = new GiftCardUsage
            {
                GiftCardId = giftCard.Id,
                OrderId = orderId,
                UserId = userId,  // NOW we track who used it
                AmountUsed = amountToApply,
                BalanceAfterUsage = giftCard.CurrentBalance,
                UsedAt = DateTime.UtcNow
            };

            _context.GiftCardUsages.Add(usage);
            await _context.SaveChangesAsync();

            Console.WriteLine($"New Gift Card Balance: {giftCard.CurrentBalance}");
            Console.WriteLine("Gift card usage recorded successfully!");

            return amountToApply;
        }

        public async Task<List<GiftCardDto>> GetUserGiftCards(int userId)
        {
            var giftCards = await _context.GiftCards
                .Include(g => g.PurchasedByUser)
                .Where(g => g.PurchasedByUserId == userId)
                .OrderByDescending(g => g.CreatedAt)
                .Select(g => new GiftCardDto
                {
                    Id = g.Id,
                    Code = g.Code,
                    OriginalAmount = g.OriginalAmount,
                    CurrentBalance = g.CurrentBalance,
                    RecipientName = g.RecipientName,
                    RecipientEmail = g.RecipientEmail,
                    SenderName = g.SenderName,
                    SenderEmail = g.SenderEmail,
                    Message = g.Message,
                    IsActive = g.IsActive,
                    IsUsed = g.CurrentBalance <= 0,  // Calculate based on balance
                    CreatedAt = g.CreatedAt,
                    UsedAt = null,  // We'll get this from usage history if needed
                    PurchasedByUserName = g.PurchasedByUser.FirstName + " " + g.PurchasedByUser.LastName,
                    UsedByUserName = null  // Remove this or get from usage history
                })
                .ToListAsync();

            return giftCards;
        }

        public async Task<List<GiftCardUsageDto>> GetGiftCardUsageHistory(string code, int requestingUserId)
        {
            var giftCard = await _context.GiftCards
                .FirstOrDefaultAsync(g => g.Code == code);

            if (giftCard == null)
                throw new InvalidOperationException("Gift card not found");

            // Check if user has permission to view this gift card
            if (giftCard.PurchasedByUserId != requestingUserId)
                throw new UnauthorizedAccessException("You don't have permission to view this gift card");

            var usages = await _context.GiftCardUsages
                .Include(u => u.Order)
                .Include(u => u.User)
                .Where(u => u.GiftCardId == giftCard.Id)
                .OrderByDescending(u => u.UsedAt)
                .Select(u => new GiftCardUsageDto
                {
                    Id = u.Id,
                    GiftCardCode = code,
                    AmountUsed = u.AmountUsed,
                    BalanceAfterUsage = u.BalanceAfterUsage,
                    UsedAt = u.UsedAt,
                    OrderReference = $"Order #{u.OrderId}",
                    UsedByName = u.User.FirstName + " " + u.User.LastName,
                    UsedByEmail = u.User.Email
                })
                .ToListAsync();

            return usages;
        }

        public async Task<List<GiftCardAdminDto>> GetAllGiftCardsForAdmin()
        {
            var giftCards = await _context.GiftCards
                .Include(g => g.PurchasedByUser)
                .Include(g => g.GiftCardUsages)
                    .ThenInclude(u => u.User)
                .OrderByDescending(g => g.CreatedAt)
                .Select(g => new GiftCardAdminDto
                {
                    Id = g.Id,
                    Code = g.Code,
                    OriginalAmount = g.OriginalAmount,
                    CurrentBalance = g.CurrentBalance,
                    RecipientName = g.RecipientName,
                    RecipientEmail = g.RecipientEmail,
                    SenderName = g.SenderName,
                    SenderEmail = g.SenderEmail,
                    Message = g.Message,
                    IsActive = g.IsActive,
                    IsPaid = g.IsPaid,
                    CreatedAt = g.CreatedAt,
                    PaidAt = g.PaidAt,
                    PurchasedByUserName = g.PurchasedByUser.FirstName + " " + g.PurchasedByUser.LastName,
                    TotalAmountUsed = g.OriginalAmount - g.CurrentBalance,
                    TimesUsed = g.GiftCardUsages.Count,
                    LastUsedAt = g.GiftCardUsages.OrderByDescending(u => u.UsedAt).FirstOrDefault() != null
                        ? g.GiftCardUsages.OrderByDescending(u => u.UsedAt).First().UsedAt
                        : (DateTime?)null,
                    Usages = g.GiftCardUsages.Select(u => new GiftCardUsageDto
                    {
                        Id = u.Id,
                        AmountUsed = u.AmountUsed,
                        BalanceAfterUsage = u.BalanceAfterUsage,
                        UsedAt = u.UsedAt,
                        OrderReference = $"Order #{u.OrderId}",
                        UsedByName = u.User.FirstName + " " + u.User.LastName,
                        UsedByEmail = u.User.Email
                    }).ToList()
                })
                .ToListAsync();

            return giftCards;
        }

        public async Task<GiftCard> GetGiftCardByCode(string code)
        {
            return await _context.GiftCards
                .Include(g => g.PurchasedByUser)
                .FirstOrDefaultAsync(g => g.Code == code);
        }

        public async Task<bool> MarkGiftCardAsPaid(int giftCardId, string paymentIntentId)
        {
            var giftCard = await _context.GiftCards.FindAsync(giftCardId);
            if (giftCard == null) return false;

            giftCard.IsPaid = true;
            giftCard.PaidAt = DateTime.UtcNow;
            giftCard.PaymentIntentId = paymentIntentId;
            giftCard.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SimulateGiftCardPayment(int giftCardId)
        {
            var giftCard = await _context.GiftCards.FindAsync(giftCardId);
            if (giftCard == null) return false;

            // Simulate payment by marking as paid
            giftCard.IsPaid = true;
            giftCard.PaidAt = DateTime.UtcNow;
            giftCard.PaymentIntentId = "pi_simulated_" + Guid.NewGuid().ToString();
            giftCard.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // You could also send email notification here if needed

            return true;
        }
    }
}