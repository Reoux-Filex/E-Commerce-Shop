using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_Shop.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        // Who placed this order
        public string UserId { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        // Total price of the whole order
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        // Status: "Pending", "Processing", "Shipped", "Delivered", "Cancelled"
        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending";

        [StringLength(500)]
        [Display(Name = "Shipping Address")]
        public string? ShippingAddress { get; set; }

        // One Order has MANY OrderItems (like a receipt with multiple lines)
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public int OrderItemId { get; set; }

        // Which order this line belongs to
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        // Which product was ordered
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        // We COPY the price at time of purchase
        // Because product price might change later!
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }
    }
}
