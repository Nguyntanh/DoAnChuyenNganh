using System.ComponentModel.DataAnnotations;

namespace DoAnData.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int SellerId { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Tổng giá phải lớn hơn hoặc bằng 0")]
        public decimal TotalPrice { get; set; }
        [EnumDataType(typeof(OrderStatus))]
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        [Required(ErrorMessage = "Địa chỉ giao hàng là bắt buộc")]
        [StringLength(255)]
        public string ShippingAddress { get; set; }

        // Navigation properties
        public User Customer { get; set; }
        public User Seller { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }

    public enum OrderStatus { pending, confirmed, shipped, delivered, canceled }
}
