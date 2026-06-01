// File: Controllers/AccountController.cs
using E_Commerce_Shop.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Shop.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // LOGIN - GET
        public IActionResult Login() => View();

        // LOGIN - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var roles = await _userManager.GetRolesAsync(user!);

                // Admin goes to admin dashboard
                // User goes to product list
                if (roles.Contains("Admin"))
                    return RedirectToAction("Index", "Category");
                else
                    return RedirectToAction("Index", "Product");
            }

            ModelState.AddModelError("", "Wrong email or password!");
            return View(model);
        }

        // LOGOUT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}