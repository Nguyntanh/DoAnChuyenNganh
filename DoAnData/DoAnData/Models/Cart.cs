namespace DoAnData.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public User Customer { get; set; }
        public List<CartItem> CartItems { get; set; }
    }
}
