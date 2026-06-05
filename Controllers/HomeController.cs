// File: Controllers/HomeController.cs
using E_Commerce_Shop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Shop.Controllers
{
    public class HomeController : Controller
    {
        // These are the two tools this controller needs:
        // 1. _context = to talk to the database
        // 2. _logger  = to write logs (built-in, don't worry about it)
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;

        // Constructor = runs when HomeController is created
        // ASP.NET automatically "injects" (gives) these two objects
        public HomeController(AppDbContext context, ILogger<HomeController> logger)
        {
            _context = context;  // save database tool
            _logger = logger;    // save logger tool
        }

        // ============================================================
        // HOME PAGE
        // URL: / or /Home/Index
        // ============================================================
        public async Task<IActionResult> Index()
        {
            // Get 4 random products to show on homepage
            var featuredProducts = await _context.Products
                .Include(p => p.Category)
                .OrderBy(p => Guid.NewGuid())
                .Take(4)
                .ToListAsync();

            // Get all categories
            var categories = await _context.Categories
                .Include(c => c.Products)
                .ToListAsync();

            // Pass data to View using ViewBag
            ViewBag.TotalProducts = await _context.Products.CountAsync();
            ViewBag.TotalCategories = await _context.Categories.CountAsync();
            ViewBag.FeaturedProducts = featuredProducts;
            ViewBag.Categories = categories;

            return View();
        }

        // ============================================================
        // PRIVACY PAGE (default page, keep it)
        // ============================================================
        public IActionResult Privacy()
        {
            return View();
        }

        // ============================================================
        // ERROR PAGE (default page, keep it)
        // ============================================================
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}