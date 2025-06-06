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

            // Calculate the additional amount before updating
            var additionalAmount = await CalculateAdditionalAmount(orderId, updateOrderDto);

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

            // Update services
            _context.OrderServices.RemoveRange(order.OrderServices);

            decimal newSubTotal = order.ServiceType.BasePrice;
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
                        Cost = service.Cost * serviceDto.Quantity,
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
                    var cost = extraService.HasHours
                        ? extraService.Price * extraServiceDto.Hours
                        : extraService.Price * extraServiceDto.Quantity;

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
                    newSubTotal += cost;
                    newTotalDuration += orderExtraService.Duration;
                }
            }

            // Recalculate totals
            order.SubTotal = newSubTotal;
            order.TotalDuration = newTotalDuration;

            // Reapply original discount
            var discountedSubTotal = newSubTotal - order.DiscountAmount;
            order.Tax = discountedSubTotal * 0.088m; // 8.8% tax
            order.Total = discountedSubTotal + order.Tax + order.Tips;

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
                            break; // Super deep cleaning takes precedence
                        }
                        else if (extraService.IsDeepCleaning && priceMultiplier == 1.0m)
                        {
                            priceMultiplier = extraService.PriceMultiplier;
                        }
                    }
                }
            }

            // Add base price from service type with multiplier
            if (order.ServiceType != null)
            {
                newSubTotal = order.ServiceType.BasePrice * priceMultiplier;
            }
            else
            {
                // If ServiceType is null, try to load it
                var serviceType = await _context.ServiceTypes.FindAsync(order.ServiceTypeId);
                if (serviceType != null)
                {
                    newSubTotal = serviceType.BasePrice * priceMultiplier;
                }
            }

            // Calculate services cost
            if (updateOrderDto.Services != null && updateOrderDto.Services.Any())
            {
                foreach (var serviceDto in updateOrderDto.Services)
                {
                    var service = await _context.Services.FindAsync(serviceDto.ServiceId);
                    if (service != null)
                    {
                        // Special handling for office cleaning
                        if (service.ServiceKey == "cleaners" && order.ServiceType?.Name == "Office Cleaning")
                        {
                            // Find the hours service
                            var hoursServiceDto = updateOrderDto.Services.FirstOrDefault(s =>
                            {
                                var svc = _context.Services.Find(s.ServiceId);
                                return svc?.ServiceKey == "hours";
                            });

                            if (hoursServiceDto != null)
                            {
                                var hours = hoursServiceDto.Quantity;
                                var cleaners = serviceDto.Quantity;
                                var costPerCleanerPerHour = service.Cost * priceMultiplier;
                                newSubTotal += costPerCleanerPerHour * cleaners * hours;
                            }
                        }
                        else if (service.ServiceKey == "bedrooms" && serviceDto.Quantity == 0)
                        {
                            // Studio apartment - flat rate
                            newSubTotal += 20 * priceMultiplier;
                        }
                        else if (service.ServiceKey != "hours")
                        {
                            // Regular service calculation
                            newSubTotal += service.Cost * serviceDto.Quantity * priceMultiplier;
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
                        // Don't add cost for deep cleaning services as they affect the multiplier
                        if (!extraService.IsDeepCleaning && !extraService.IsSuperDeepCleaning)
                        {
                            // Apply multiplier EXCEPT for Same Day Service
                            var currentMultiplier = extraService.IsSameDayService ? 1.0m : priceMultiplier;

                            if (extraService.HasHours && extraServiceDto.Hours > 0)
                            {
                                newSubTotal += extraService.Price * extraServiceDto.Hours * currentMultiplier;
                            }
                            else if (extraService.HasQuantity && extraServiceDto.Quantity > 0)
                            {
                                newSubTotal += extraService.Price * extraServiceDto.Quantity * currentMultiplier;
                            }
                            else if (!extraService.HasHours && !extraService.HasQuantity)
                            {
                                // Fixed price service
                                newSubTotal += extraService.Price * currentMultiplier;
                            }
                        }
                        else
                        {
                            // Deep cleaning services - add their base price as a fee
                            newSubTotal += extraService.Price;
                        }
                    }
                }
            }

            // Apply original discount amount (not percentage, to keep the same discount)
            var discountedSubTotal = newSubTotal - order.DiscountAmount;

            // Make sure we don't go negative
            if (discountedSubTotal < 0)
            {
                discountedSubTotal = 0;
            }

            var newTax = discountedSubTotal * 0.088m; // 8.8% tax
            var newTotal = discountedSubTotal + newTax + updateOrderDto.Tips;

            // Calculate the difference
            var additionalAmount = newTotal - order.Total;

            // Return the actual difference (can be negative if reducing services)
            return additionalAmount;
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