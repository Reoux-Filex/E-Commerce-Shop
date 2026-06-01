using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_Shop.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, ErrorMessage = "Max 200 characters")]
        [Display(Name = "Product Name")]
        public string Name { get; set; }

        [StringLength(1000)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        // [Range] = value must be between min and max
        // [Column(TypeName)] = tells SQL Server to store as decimal, not float
        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 99999999, ErrorMessage = "Price must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        // How many items are in stock
        [Range(0, 99999, ErrorMessage = "Stock cannot be negative")]
        [Display(Name = "Stock")]
        public int Stock { get; set; }

        // Product image filename (e.g. "iphone.jpg")
        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }

        // FOREIGN KEY - links this product to a Category
        // This creates a "CategoryId" column in the Products table
        [Required(ErrorMessage = "Please select a category")]
        public int CategoryId { get; set; }

        // NAVIGATION PROPERTY
        // Lets you do product.Category to get the full Category object
        // "?" means Category can be null when first creating the object
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
    }
}
