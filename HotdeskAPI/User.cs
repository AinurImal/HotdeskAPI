using System.ComponentModel.DataAnnotations;

namespace Hotdesk.Models
{
    public class User
    {
        [Key]
        [MaxLength(8)]
        public string UserId { get; set; } = string.Empty; // Unique, auto-generated (e.g., "1001", "1002", ...)

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty; // Input by user

        [Required]
        [MaxLength(50)]
        public string UserName { get; set; } = string.Empty; // Input by user, unique

        [Required]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must be in the format 0123456789.")]
        public string PhoneNumber { get; set; } = string.Empty; // Input by user, format 0123456789
    }
}

