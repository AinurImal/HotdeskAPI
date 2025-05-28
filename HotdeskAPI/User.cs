using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Hotdesk.Models
{
    [Index(nameof(UserName), IsUnique = true)]
    [Index(nameof(PhoneNumber), IsUnique = true)]
    public class User
    {
        [Key]
        [MaxLength(8)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must be in the format 0123456789.")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}



