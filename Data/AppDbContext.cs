using E_Commerce_Shop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Shop.Data
{
    // IdentityDbContext = DbContext + built-in Login/Register/Roles system
    // We pass IdentityUser because we use the default user (no custom fields needed)
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {

        // This constructor receives database settings from Program.cs
        // and passes them to the parent class (DbContext)
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // Each DbSet = one table in the database
        // DbSet<Category> = the "Categories" table
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // MUST call base first when using Identity
            // Without this, the Identity tables (AspNetUsers etc.) won't be created
            base.OnModelCreating(modelBuilder);

            // Configure 1-MANY: Category -> Products
            // One category has many products
            // If you try to delete a category that has products = ERROR (Restrict)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure 1-MANY: Order -> OrderItems
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            // Cascade = if Order is deleted, all its OrderItems are deleted too

            // ===== SEED DATA (sample data auto-inserted when DB is created) =====
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    CategoryId = 1,
                    Name = "Electronics",
                    Description = "Phones, laptops, gadgets"
                },
                new Category
                {
                    CategoryId = 2,
                    Name = "Clothing",
                    Description = "T-shirts, pants, shoes"
                },
                new Category
                {
                    CategoryId = 3,
                    Name = "Food & Drinks",
                    Description = "Snacks, beverages"
                }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 1,
                    Name = "iPhone 15",
                    Description = "Latest Apple phone",
                    Price = 25000000,
                    Stock = 10,
                    CategoryId = 1
                },
                new Product
                {
                    ProductId = 2,
                    Name = "Samsung Galaxy S24",
                    Description = "Android flagship",
                    Price = 22000000,
                    Stock = 15,
                    CategoryId = 1
                },
                new Product
                {
                    ProductId = 3,
                    Name = "Nike T-Shirt",
                    Description = "Cotton sports shirt",
                    Price = 350000,
                    Stock = 50,
                    CategoryId = 2
                },
                new Product
                {
                    ProductId = 4,
                    Name = "Laptop Dell XPS",
                    Description = "Core i7 16GB RAM",
                    Price = 32000000,
                    Stock = 5,
                    CategoryId = 1
                },
                new Product
                {
                    ProductId = 5,
                    Name = "Coca Cola 24 pack",
                    Description = "330ml cans",
                    Price = 180000,
                    Stock = 100,
                    CategoryId = 3
                },
                new Product
                {
                    ProductId = 6,
                    Name = "Adidas Sneakers",
                    Description = "Running shoes",
                    Price = 1200000,
                    Stock = 30,
                    CategoryId = 2
                }
            );
        }
    }
}
