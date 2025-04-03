using System.ComponentModel.DataAnnotations;

namespace DoAnData.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        [EnumDataType(typeof(UserRole))]
        public string Role { get; set; }
        [EnumDataType(typeof(UserStatus))]
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public enum UserRole { customer, seller, admin }
    public enum UserStatus { active, pending, inactive }
}
