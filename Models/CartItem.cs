namespace E_Commerce_Shop.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }

        // Which user owns this cart item (stores the user's email or ID)
        public string? UserId { get; set; }

        // Which product they added
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        // How many they want to buy
        public int Quantity { get; set; }
    }
}
