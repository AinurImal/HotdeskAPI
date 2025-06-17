using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hotdesk.Components.Models;
using System.Text.Json.Serialization;
using HotdeskAPI;

namespace Hotdesk.Models
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingId { get; set; } // Primary key, auto-increment

        [ForeignKey(nameof(Desk))]
        public int DeskId { get; set; } // Foreign key to Desk

        [Required]
        [MaxLength(50)]
        public string UserName { get; set; } = string.Empty; // Foreign key to User.UserName

        public DateTime BookingDate { get; set; } // Date of booking

        public string DurationType { get; set; } = string.Empty; // Duration type (e.g., "FullDay", "HalfDay")

        public bool CheckedIn { get; set; } // Check-in status

        public DateTime? CheckInTime { get; set; } // Optional check-in time

        [JsonIgnore]
        public virtual Desk? Desk { get; set; } // Navigation property to Desk

        // Optionally, you can add a navigation property to User if you configure it in your DbContext
        // [JsonIgnore]
        // public virtual User? User { get; set; }
    }
}




