using Microsoft.EntityFrameworkCore;
using DreamCleaningBackend.Data;
using DreamCleaningBackend.DTOs;
using DreamCleaningBackend.Models;
using DreamCleaningBackend.Services.Interfaces;
using DreamCleaningBackend.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DreamCleaningBackend.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ApplicationDbContext _context;

        public OrderService(IOrderRepository orderRepository, ApplicationDbContext context)
        {
            _orderRepository = orderRepository;
            _context = context;
        }

        public async Task<List<OrderListDto>> GetUserOrders(int userId)
        {
            var orders = await _orderRepository.GetUserOrdersAsync(userId);

            return orders.Select(o => new OrderListDto
            {
                Id = o.Id,
                ServiceTypeName = o.ServiceType?.Name ?? "",
                ServiceDate = o.ServiceDate,
                ServiceTime = o.ServiceTime,
                Status = o.Status,
                Total = o.Total,
                ServiceAddress = o.ServiceAddress + (string.IsNullOrEmpty(o.AptSuite) ? "" : ", " + o.AptSuite),
                OrderDate = o.OrderDate
            }).ToList();
        }

        public async Task<OrderDto> GetOrderById(int orderId, int userId)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);

            if (order == null || order.UserId != userId)
                throw new Exception("Order not found");

            return MapOrderToDto(order);
        }

        public async Task<OrderDto> UpdateOrder(int orderId, int userId, UpdateOrderDto updateOrderDto)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);

            if (order == null || order.UserId != userId)
                throw new Exception("Order not found");

            if (order.Status == "Cancelled")
                throw new Exception("Cannot update a cancelled order");

            if (order.Status == "Done")
                throw new Exception("Cannot update a completed order");

            // Store the original total for comparison
            var originalTotal = order.Total;

            // Log the original total
            Console.WriteLine($"Original total: ${originalTotal:F2}");

            // Calculate the additional amount before updating
            var additionalAmount = await CalculateAdditionalAmount(orderId, updateOrderDto);

            // Log the additional amount
            Console.WriteLine($"Additional amount: ${additionalAmount:F2}");

            // Check if the new total would be less than the original
            if (additionalAmount < 0)
            {
                var newTotal = originalTotal + additionalAmount;
                throw new Exception($"Cannot reduce order total. Original: ${originalTotal:F2}, New: ${newTotal:F2}, Difference: ${Math.Abs(additionalAmount):F2}");
            }

            // Update basic order information
            order.ServiceDate = updateOrderDto.ServiceDate;
            order.ServiceTime = TimeSpan.Parse(updateOrderDto.ServiceTime);
            order.EntryMethod = updateOrderDto.EntryMethod;
            order.SpecialInstructions = updateOrderDto.SpecialInstructions;
            order.ContactFirstName = updateOrderDto.ContactFirstName;
            order.ContactLastName = updateOrderDto.ContactLastName;
            order.ContactEmail = updateOrderDto.ContactEmail;
            order.ContactPhone = updateOrderDto.ContactPhone;
            order.ServiceAddress = updateOrderDto.ServiceAddress;
            order.AptSuite = updateOrderDto.AptSuite;
            order.City = updateOrderDto.City;
            order.State = updateOrderDto.State;
            order.ZipCode = updateOrderDto.ZipCode;
            order.Tips = updateOrderDto.Tips;
            order.UpdatedAt = DateTime.UtcNow;

            // Calculate price multiplier from extra services FIRST
            decimal priceMultiplier = 1.0m;
            decimal deepCleaningFee = 0;

            foreach (var extraServiceDto in updateOrderDto.ExtraServices)
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
                    else if (extraService.IsDeepCleaning && priceMultiplier == 1.0m)
                    {
                        priceMultiplier = extraService.PriceMultiplier;
                        deepCleaningFee = extraService.Price;
                    }
                }
            }

            // Update services
            _context.OrderServices.RemoveRange(order.OrderServices);

            decimal newSubTotal = order.ServiceType.BasePrice * priceMultiplier;
            int newTotalDuration = 0;

            foreach (var serviceDto in updateOrderDto.Services)
            {
                var service = await _context.Services.FindAsync(serviceDto.ServiceId);
                if (service != null)
                {
                    var orderService = new Models.OrderService
                    {
                        Order = order,
                        ServiceId = serviceDto.ServiceId,
                        Quantity = serviceDto.Quantity,
                        Cost = service.Cost * serviceDto.Quantity * priceMultiplier,
                        Duration = service.TimeDuration * serviceDto.Quantity,
                        CreatedAt = DateTime.UtcNow
                    };
                    order.OrderServices.Add(orderService);
                    newSubTotal += orderService.Cost;
                    newTotalDuration += orderService.Duration;
                }
            }

            // Update extra services
            _context.OrderExtraServices.RemoveRange(order.OrderExtraServices);

            foreach (var extraServiceDto in updateOrderDto.ExtraServices)
            {
                var extraService = await _context.ExtraServices.FindAsync(extraServiceDto.ExtraServiceId);
                if (extraService != null)
                {
                    decimal cost = 0;

                    // For deep cleaning services, store their actual price
                    if (extraService.IsDeepCleaning || extraService.IsSuperDeepCleaning)
                    {
                        cost = extraService.Price;
                    }
                    else
                    {
                        // Regular extra services
                        var currentMultiplier = extraService.IsSameDayService ? 1.0m : priceMultiplier;

                        if (extraService.HasHours && extraServiceDto.Hours > 0)
                        {
                            cost = extraService.Price * extraServiceDto.Hours * currentMultiplier;
                        }
                        else if (extraService.HasQuantity && extraServiceDto.Quantity > 0)
                        {
                            cost = extraService.Price * extraServiceDto.Quantity * currentMultiplier;
                        }
                        else if (!extraService.HasHours && !extraService.HasQuantity)
                        {
                            cost = extraService.Price * currentMultiplier;
                        }
                    }

                    var orderExtraService = new OrderExtraService
                    {
                        Order = order,
                        ExtraServiceId = extraServiceDto.ExtraServiceId,
                        Quantity = extraServiceDto.Quantity,
                        Hours = extraServiceDto.Hours,
                        Cost = cost,
                        Duration = extraService.Duration,
                        CreatedAt = DateTime.UtcNow
                    };
                    order.OrderExtraServices.Add(orderExtraService);

                    // Only add non-deep-cleaning costs to subtotal
                    if (!extraService.IsDeepCleaning && !extraService.IsSuperDeepCleaning)
                    {
                        newSubTotal += cost;
                    }
                    newTotalDuration += orderExtraService.Duration;
                }
            }

            // Add deep cleaning fee AFTER all other calculations
            newSubTotal += deepCleaningFee;

            // Recalculate totals
            order.SubTotal = newSubTotal;
            order.TotalDuration = newTotalDuration;

            // Reapply original discount
            var discountedSubTotal = newSubTotal - order.DiscountAmount;
            order.Tax = discountedSubTotal * 0.088m; // 8.8% tax
            order.Total = discountedSubTotal + order.Tax + order.Tips;

            // Final check to ensure the new total is not less than the original
            if (order.Total < originalTotal)
            {
                throw new Exception($"Cannot save changes. The new total (${order.Total:F2}) is less than the original amount paid (${originalTotal:F2}). Please add more services or keep the current selection.");
            }

            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();

            return await GetOrderById(orderId, userId);
        }

        public async Task<bool> CancelOrder(int orderId, int userId, CancelOrderDto cancelOrderDto)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null || order.UserId != userId)
                throw new Exception("Order not found");

            if (order.Status == "Cancelled")
                throw new Exception("Order is already cancelled");

            if (order.Status == "Done")
                throw new Exception("Cannot cancel a completed order");

            // Check if service date is not too close (e.g., within 24 hours)
            if (order.ServiceDate <= DateTime.UtcNow.AddHours(24))
                throw new Exception("Cannot cancel order within 24 hours of service date");

            order.Status = "Cancelled";
            order.UpdatedAt = DateTime.UtcNow;

            // In a real system, you would initiate a refund process here
            // For now, we'll just mark it as cancelled

            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();

            return true;
        }

        // In OrderService.cs, update the CalculateAdditionalAmount method:

        public async Task<decimal> CalculateAdditionalAmount(int orderId, UpdateOrderDto updateOrderDto)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);

            if (order == null)
                throw new Exception("Order not found");

            // Calculate new total based on the original order's service type
            decimal newSubTotal = 0;
            decimal deepCleaningFee = 0;

            // Check for deep cleaning multipliers in the update
            decimal priceMultiplier = 1.0m;

            if (updateOrderDto.ExtraServices != null)
            {
                foreach (var extraServiceDto in updateOrderDto.ExtraServices)
                {
                    var extraService = await _context.ExtraServices.FindAsync(extraServiceDto.ExtraServiceId);
                    if (extraService != null)
                    {
                        if (extraService.IsSuperDeepCleaning)
                        {
                            priceMultiplier = extraService.PriceMultiplier;
                            deepCleaningFee = extraService.Price;
                            Console.WriteLine($"Found Super Deep Cleaning: Multiplier={priceMultiplier}, Fee=${deepCleaningFee}");
                            break;
                        }
                        else if (extraService.IsDeepCleaning && priceMultiplier == 1.0m)
                        {
                            priceMultiplier = extraService.PriceMultiplier;
                            deepCleaningFee = extraService.Price;
                            Console.WriteLine($"Found Deep Cleaning: Multiplier={priceMultiplier}, Fee=${deepCleaningFee}");
                        }
                    }
                }
            }

            // Add base price from service type with multiplier
            var basePrice = 0m;
            if (order.ServiceType != null)
            {
                basePrice = order.ServiceType.BasePrice;
            }
            else
            {
                var serviceType = await _context.ServiceTypes.FindAsync(order.ServiceTypeId);
                if (serviceType != null)
                {
                    basePrice = serviceType.BasePrice;
                }
            }
            newSubTotal = basePrice * priceMultiplier;
            Console.WriteLine($"Base price: ${basePrice} x {priceMultiplier} = ${newSubTotal}");

            // Calculate services cost
            if (updateOrderDto.Services != null && updateOrderDto.Services.Any())
            {
                foreach (var serviceDto in updateOrderDto.Services)
                {
                    var service = await _context.Services.FindAsync(serviceDto.ServiceId);
                    if (service != null)
                    {
                        Console.WriteLine($"Processing service: {service.Name} (Key: {service.ServiceKey}, RelationType: {service.ServiceRelationType})");

                        // Special handling for office cleaning with cleaners/hours relationship
                        if (service.ServiceRelationType == "cleaner")
                        {
                            // Find the hours service
                            var hoursServiceDto = updateOrderDto.Services.FirstOrDefault(s =>
                            {
                                var svc = _context.Services.Find(s.ServiceId);
                                return svc?.ServiceRelationType == "hours" && svc.ServiceTypeId == service.ServiceTypeId;
                            });

                            if (hoursServiceDto != null)
                            {
                                var hours = hoursServiceDto.Quantity;
                                var cleaners = serviceDto.Quantity;
                                var costPerCleanerPerHour = service.Cost * priceMultiplier;
                                var totalCost = costPerCleanerPerHour * cleaners * hours;
                                newSubTotal += totalCost;
                                Console.WriteLine($"  Cleaners calculation: {cleaners} cleaners x {hours} hours x ${costPerCleanerPerHour}/hour = ${totalCost}");
                            }
                            else
                            {
                                var cost = service.Cost * serviceDto.Quantity * priceMultiplier;
                                newSubTotal += cost;
                                Console.WriteLine($"  Cleaner only: {serviceDto.Quantity} x ${service.Cost} x {priceMultiplier} = ${cost}");
                            }
                        }
                        else if (service.ServiceRelationType == "hours")
                        {
                            // Check if there's a cleaner service
                            var hasCleanerService = updateOrderDto.Services.Any(s =>
                            {
                                var svc = _context.Services.Find(s.ServiceId);
                                return svc?.ServiceRelationType == "cleaner" && svc.ServiceTypeId == service.ServiceTypeId;
                            });

                            if (!hasCleanerService)
                            {
                                var cost = service.Cost * serviceDto.Quantity * priceMultiplier;
                                newSubTotal += cost;
                                Console.WriteLine($"  Hours standalone: {serviceDto.Quantity} x ${service.Cost} x {priceMultiplier} = ${cost}");
                            }
                            else
                            {
                                Console.WriteLine($"  Hours skipped (handled with cleaners)");
                            }
                        }
                        else if (service.ServiceKey == "bedrooms" && serviceDto.Quantity == 0)
                        {
                            var cost = 20 * priceMultiplier;
                            newSubTotal += cost;
                            Console.WriteLine($"  Studio: $20 x {priceMultiplier} = ${cost}");
                        }
                        else
                        {
                            var cost = service.Cost * serviceDto.Quantity * priceMultiplier;
                            newSubTotal += cost;
                            Console.WriteLine($"  Regular service: {serviceDto.Quantity} x ${service.Cost} x {priceMultiplier} = ${cost}");
                        }
                    }
                }
            }

            // Calculate extra services cost
            if (updateOrderDto.ExtraServices != null && updateOrderDto.ExtraServices.Any())
            {
                foreach (var extraServiceDto in updateOrderDto.ExtraServices)
                {
                    var extraService = await _context.ExtraServices.FindAsync(extraServiceDto.ExtraServiceId);
                    if (extraService != null)
                    {
                        Console.WriteLine($"Processing extra service: {extraService.Name}");

                        if (!extraService.IsDeepCleaning && !extraService.IsSuperDeepCleaning)
                        {
                            var currentMultiplier = extraService.IsSameDayService ? 1.0m : priceMultiplier;

                            if (extraService.HasHours && extraServiceDto.Hours > 0)
                            {
                                var cost = extraService.Price * extraServiceDto.Hours * currentMultiplier;
                                newSubTotal += cost;
                                Console.WriteLine($"  Hours-based: {extraServiceDto.Hours}h x ${extraService.Price} x {currentMultiplier} = ${cost}");
                            }
                            else if (extraService.HasQuantity && extraServiceDto.Quantity > 0)
                            {
                                var cost = extraService.Price * extraServiceDto.Quantity * currentMultiplier;
                                newSubTotal += cost;
                                Console.WriteLine($"  Quantity-based: {extraServiceDto.Quantity} x ${extraService.Price} x {currentMultiplier} = ${cost}");
                            }
                            else if (!extraService.HasHours && !extraService.HasQuantity)
                            {
                                var cost = extraService.Price * currentMultiplier;
                                newSubTotal += cost;
                                Console.WriteLine($"  Fixed price: ${extraService.Price} x {currentMultiplier} = ${cost}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"  Deep cleaning service - no direct cost added");
                        }
                    }
                }
            }

            // Add deep cleaning fee AFTER all calculations
            newSubTotal += deepCleaningFee;
            Console.WriteLine($"Added deep cleaning fee: ${deepCleaningFee}");
            Console.WriteLine($"Final SubTotal: ${newSubTotal}");

            // Apply original discount amount
            var discountedSubTotal = newSubTotal - order.DiscountAmount;
            if (discountedSubTotal < 0)
            {
                discountedSubTotal = 0;
            }

            var newTax = discountedSubTotal * 0.088m;
            var newTotal = discountedSubTotal + newTax + updateOrderDto.Tips;

            Console.WriteLine($"CalculateAdditionalAmount Summary:");
            Console.WriteLine($"  New SubTotal: ${newSubTotal}");
            Console.WriteLine($"  Discount Amount: ${order.DiscountAmount}");
            Console.WriteLine($"  Discounted SubTotal: ${discountedSubTotal}");
            Console.WriteLine($"  New Tax: ${newTax}");
            Console.WriteLine($"  Tips: ${updateOrderDto.Tips}");
            Console.WriteLine($"  New Total: ${newTotal}");
            Console.WriteLine($"  Original Total: ${order.Total}");
            Console.WriteLine($"  Additional Amount: ${newTotal - order.Total}");

            return newTotal - order.Total;
        }

        public async Task<bool> MarkOrderAsDone(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
                throw new Exception("Order not found");

            if (order.Status == "Cancelled")
                throw new Exception("Cannot complete a cancelled order");

            order.Status = "Done";
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();

            return true;
        }

        private OrderDto MapOrderToDto(Order order)
        {
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
                PromoCode = order.PromoCode,
                FrequencyId = order.FrequencyId,
                FrequencyName = order.Frequency?.Name ?? "",
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
    }
}