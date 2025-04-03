using System.ComponentModel.DataAnnotations;

namespace DoAnData.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        [Range(1, 5, ErrorMessage = "Điểm đánh giá phải từ 1 đến 5")]
        public int Rating { get; set; }
        public string? Comment { get; set; } // Nullable vì Comment không bắt buộc
        public DateTime ReviewDate { get; set; }

        // Navigation properties
        public User Customer { get; set; }
        public Product Product { get; set; }
    }
}
