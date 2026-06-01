using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Shop.Models
{
    public class Category
    {
        //Primary key - EF core automatically detects "Id" or "CategoryId" as primary key
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        [Display(Name = "Category Name")]
        public string Name { get; set; }

        // "?" means this field is OPTIONAL (nullable)
        [StringLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        // NAVIGATION PROPERTY
        // This line tells EF Core: "1 Category has MANY Products"
        // It does NOT create a column in DB — it's just for C# to access related data
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
