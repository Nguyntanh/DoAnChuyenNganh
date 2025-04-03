using System.ComponentModel.DataAnnotations;

namespace DoAnData.Models
{
    public class SupportTicket
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OrderId { get; set; }
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [StringLength(100)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Mô tả là bắt buộc")]
        public string Description { get; set; }
        [EnumDataType(typeof(TicketStatus))]
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; } // Nullable vì có thể chưa giải quyết

        // Navigation properties
        public User User { get; set; }
        public Order Order { get; set; }
    }

    public enum TicketStatus { open, resolved }
}
