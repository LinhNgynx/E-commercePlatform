using E_commerceData.Data;
using E_commerceData.Models;
using E_commerceData.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_commerceData.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;

namespace E_commerceData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IServiceProvider _serviceProvider;

        public OrdersController(ApplicationDbContext context, IEmailSender emailSender, IServiceProvider serviceProvider)
        {
            _context = context;
            _emailSender = emailSender;
            _serviceProvider = serviceProvider;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users.FindAsync(request.UserId);
                var address = await _context.Addresses.FindAsync(request.AddressId);
                if (user == null || address == null)
                    return BadRequest("Invalid user or address.");

                decimal total = 0;
                var orderItems = new List<OrderItem>();
                var itemSummaries = new List<object>();

                foreach (var item in request.Items)
                {
                    var variant = await _context.ProductVariants
                        .Include(v => v.Product)
                        .FirstOrDefaultAsync(v => v.Id == item.ProductVariantId);

                    if (variant == null || variant.StockQuantity < item.Quantity)
                        return BadRequest($"Variant {item.ProductVariantId} is invalid or out of stock.");

                    variant.StockQuantity -= item.Quantity;

                    decimal itemTotal = variant.Product.Price * item.Quantity;
                    total += itemTotal;

                    orderItems.Add(new OrderItem
                    {
                        ProductVariantId = item.ProductVariantId,
                        Quantity = item.Quantity,
                        PriceAtOrder = variant.Product.Price
                    });

                    itemSummaries.Add(new
                    {
                        product_name = variant.Product.Name,
                        quantity = item.Quantity,
                        price = variant.Product.Price
                    });
                }

                if (request.VoucherIds != null && request.VoucherIds.Any())
                {
                    var vouchers = await _context.Vouchers
                        .Where(v => request.VoucherIds.Contains(v.Id))
                        .ToListAsync();

                    foreach (var voucher in vouchers)
                    {
                        if (voucher.IsPercentage)
                            total -= total * (voucher.DiscountAmount / 100m);
                        else
                            total -= voucher.DiscountAmount;
                    }
                }

                var paymentStatus = request.PaymentMethod.ToLower() == "credit_card" ? "Success" : "Failed";
                if (paymentStatus != "Success")
                    return StatusCode(402, new { message = "Payment failed." });

                var order = new Order
                {
                    UserId = request.UserId,
                    AddressId = request.AddressId,
                    TotalPrice = total,
                    PaymentStatus = paymentStatus,
                    OrderDate = DateTime.UtcNow,
                    OrderItems = orderItems
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                BackgroundJob.Enqueue<IOrderEmailService>(service =>
                    service.SendOrderEmailAsync(user.Email, user.Name, order.Id, itemSummaries, total));

                return Ok(new
                {
                    order_id = order.Id,
                    total_price = total,
                    payment_status = paymentStatus
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Order processing failed.", error = ex.Message });
            }
        }
    }
}