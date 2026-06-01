// File: Controllers/OrderController.cs
using E_Commerce_Shop.Data;
using E_Commerce_Shop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Shop.Controllers
{
    [Authorize] // Must be logged in
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OrderController(
            AppDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ============================================================
        // CHECKOUT - Show order summary before placing order (GET)
        // URL: /Order/Checkout
        // ============================================================
        public async Task<IActionResult> Checkout()
        {
            var userId = _userManager.GetUserId(User);

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            // If cart is empty, go back to cart
            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("Index", "Cart");
            }

            ViewBag.Total = cartItems
                .Sum(c => c.Product!.Price * c.Quantity);
            ViewBag.CartItems = cartItems;

            return View();
        }

        // CHECKOUT - Place the order (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(string shippingAddress)
        {
            var userId = _userManager.GetUserId(User);

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("Index", "Cart");
            }

            // Calculate total
            decimal total = cartItems
                .Sum(c => c.Product!.Price * c.Quantity);

            // Create the Order
            var order = new Order
            {
                UserId = userId!,
                OrderDate = DateTime.Now,
                TotalAmount = total,
                Status = "Pending",
                ShippingAddress = shippingAddress
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // Save to get OrderId

            // Create OrderItems from CartItems
            // We COPY the price right now in case it changes later!
            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product!.Price // Copy current price!
                };
                _context.OrderItems.Add(orderItem);
            }

            // Clear the cart after ordering
            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Order #{order.OrderId} placed successfully!";
            return RedirectToAction(nameof(OrderSuccess),
                new { orderId = order.OrderId });
        }

        // ============================================================
        // ORDER SUCCESS PAGE
        // ============================================================
        public async Task<IActionResult> OrderSuccess(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null) return NotFound();
            return View(order);
        }

        // ============================================================
        // MY ORDERS - User sees their own orders
        // URL: /Order/MyOrders
        // ============================================================
        public async Task<IActionResult> MyOrders()
        {
            var userId = _userManager.GetUserId(User);

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // ============================================================
        // ALL ORDERS - Admin sees all orders
        // URL: /Order/Index
        // ============================================================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // ============================================================
        // UPDATE STATUS - Admin changes order status
        // ============================================================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                _context.Update(order);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Order #{orderId} status updated to {status}!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}