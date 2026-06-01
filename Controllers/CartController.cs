// File: Controllers/CartController.cs
using E_Commerce_Shop.Data;
using E_Commerce_Shop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Shop.Controllers
{
    [Authorize] // Must be logged in to use cart
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(
            AppDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ============================================================
        // INDEX - Show cart contents
        // URL: /Cart/Index
        // ============================================================
        public async Task<IActionResult> Index()
        {
            // Get current logged-in user's ID
            var userId = _userManager.GetUserId(User);

            // Get all cart items for this user
            // Include Product so we can show product name, price etc.
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            // Calculate total price
            // Sum of (price × quantity) for each item
            ViewBag.Total = cartItems
                .Sum(c => c.Product!.Price * c.Quantity);

            return View(cartItems);
        }

        // ============================================================
        // ADD TO CART
        // URL: /Cart/AddToCart/5
        // ============================================================
        public async Task<IActionResult> AddToCart(int productId)
        {
            var userId = _userManager.GetUserId(User);

            // Check if this product is already in the user's cart
            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c =>
                    c.UserId == userId &&
                    c.ProductId == productId);

            if (existingItem != null)
            {
                // Product already in cart → just increase quantity
                existingItem.Quantity++;
                _context.Update(existingItem);
            }
            else
            {
                // New item → add to cart
                var cartItem = new CartItem
                {
                    UserId = userId!,
                    ProductId = productId,
                    Quantity = 1
                };
                _context.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Product added to cart!";
            return RedirectToAction("Index", "Product");
        }

        // ============================================================
        // INCREASE QUANTITY
        // ============================================================
        public async Task<IActionResult> Increase(int cartItemId)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);
            if (item != null)
            {
                item.Quantity++;
                _context.Update(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // DECREASE QUANTITY
        // ============================================================
        public async Task<IActionResult> Decrease(int cartItemId)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);
            if (item != null)
            {
                if (item.Quantity > 1)
                {
                    // Reduce quantity by 1
                    item.Quantity--;
                    _context.Update(item);
                }
                else
                {
                    // Quantity is 1 → remove from cart completely
                    _context.CartItems.Remove(item);
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // REMOVE ITEM FROM CART
        // ============================================================
        public async Task<IActionResult> Remove(int cartItemId)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Item removed from cart!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}