using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DoAnData.Models
{
    public class CartItem
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        // Navigation properties
        [JsonIgnore]
        public Cart Cart { get; set; }
        public Product Product { get; set; }
    }
}
