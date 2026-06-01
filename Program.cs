// File: Program.cs
using E_Commerce_Shop.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;  // ← ADD THIS
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// =====================================================
// REGISTER SERVICES (telling the app what it can use)
// =====================================================

// 1. Connect to SQL Server using our AppDbContext
//    Reads the connection string from appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Add Identity (Login/Register/Roles system)
//    IdentityUser = default user with email, password
//    IdentityRole = roles like "Admin", "User"
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Make password rules simple for development/testing
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()               // Enable role support
.AddEntityFrameworkStores<AppDbContext>(); // Store users in our database

// 3. Add MVC (Controllers + Views)
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Required for Identity login UI

// =====================================================
// BUILD THE APP
// =====================================================
var app = builder.Build();

// =====================================================
// AUTO-CREATE ROLES AND DEFAULT USERS ON STARTUP
// =====================================================
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<IdentityUser>>();

    // Create "Admin" role if it doesn't exist yet
    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    // Create "User" role if it doesn't exist yet
    if (!await roleManager.RoleExistsAsync("User"))
        await roleManager.CreateAsync(new IdentityRole("User"));

    // Create default Admin account
    var adminEmail = "admin@shop.com";
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var admin = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(admin, "admin123");
        await userManager.AddToRoleAsync(admin, "Admin");
    }

    // Create default User account
    var userEmail = "user@shop.com";
    if (await userManager.FindByEmailAsync(userEmail) == null)
    {
        var user = new IdentityUser
        {
            UserName = userEmail,
            Email = userEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(user, "user123");
        await userManager.AddToRoleAsync(user, "User");
    }
}

// =====================================================
// MIDDLEWARE PIPELINE (order matters!)
// =====================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();       // Serve CSS, JS, images
app.UseRouting();
app.UseAuthentication();    // WHO are you? (must be before Authorization)
app.UseAuthorization();     // WHAT can you do?
app.MapStaticAssets();

// Default route: goes to Product/Index first
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages(); // For Identity pages

app.Run();