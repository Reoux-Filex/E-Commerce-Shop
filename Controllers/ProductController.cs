// File: Controllers/ProductController.cs
using E_Commerce_Shop.Data;
using E_Commerce_Shop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Shop.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        // Constructor Injection
        // ASP.NET automatically gives us the AppDbContext we registered in Program.cs
        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // INDEX - Show all products (homepage of the shop)
        // URL: /Product/Index or just /
        // ============================================================
        public async Task<IActionResult> Index(
            string? searchName,
            string sortBy = "Name",
            string sortOrder = "asc",
            int page = 1)
        {
            int pageSize = 6; // Show 6 products per page

            // Start building the query
            // Include(p => p.Category) = also load the Category data
            // so we can show product.Category.Name in the View
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            // SEARCH by name
            if (!string.IsNullOrWhiteSpace(searchName))
                query = query.Where(p => p.Name.Contains(searchName));

            // SORT
            query = (sortBy, sortOrder) switch
            {
                ("Name", "asc") => query.OrderBy(p => p.Name),
                ("Name", "desc") => query.OrderByDescending(p => p.Name),
                ("Price", "asc") => query.OrderBy(p => p.Price),
                ("Price", "desc") => query.OrderByDescending(p => p.Price),
                _ => query.OrderBy(p => p.Name)
            };

            // PAGINATION
            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Pass data to View using ViewBag
            ViewBag.SearchName = searchName;
            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;

            return View(products);
        }

        // ============================================================
        // DETAILS - Show one product
        // URL: /Product/Details/5
        // ============================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // ============================================================
        // CREATE - Show the form (GET)
        // URL: /Product/Create
        // Only Admin can access
        // ============================================================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            // Load categories for the dropdown list in the form
            ViewBag.Categories = new SelectList(
                await _context.Categories.ToListAsync(),
                "CategoryId", "Name");
            return View();
        }

        // CREATE - Handle form submission (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Product added successfully!";
                return RedirectToAction(nameof(Index));
            }

            // If validation fails, reload the form with categories
            ViewBag.Categories = new SelectList(
                await _context.Categories.ToListAsync(),
                "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // ============================================================
        // EDIT - Show edit form (GET)
        // URL: /Product/Edit/5
        // ============================================================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewBag.Categories = new SelectList(
                await _context.Categories.ToListAsync(),
                "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // EDIT - Handle form submission (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.ProductId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Product updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(
                await _context.Categories.ToListAsync(),
                "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // ============================================================
        // DELETE - Show confirmation page (GET)
        // URL: /Product/Delete/5
        // ============================================================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();
            return View(product);
        }

        // DELETE - Handle confirmation (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Product deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}