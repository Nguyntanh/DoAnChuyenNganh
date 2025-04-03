namespace DoAnData.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public int SellerId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public int Stock { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public User Seller { get; set; }
        public Category Category { get; set; }
    }
}
