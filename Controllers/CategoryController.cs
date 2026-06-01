// File: Controllers/CategoryController.cs
using E_Commerce_Shop.Data;
using E_Commerce_Shop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Shop.Controllers
{
    // Whole controller = Admin only
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // READ - List all categories
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.Products) // Load products count
                .ToListAsync();
            return View(categories);
        }

        // CREATE GET
        public IActionResult Create() => View();

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category added!";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // EDIT GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.CategoryId) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category updated!";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // DELETE GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null) return NotFound();
            return View(category);
        }

        // DELETE POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Check if category has products first
            bool hasProducts = await _context.Products
                .AnyAsync(p => p.CategoryId == id);

            if (hasProducts)
            {
                TempData["Error"] = "Cannot delete category that has products!";
                return RedirectToAction(nameof(Index));
            }

            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category deleted!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}