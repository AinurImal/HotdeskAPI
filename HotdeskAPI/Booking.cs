using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hotdesk.Components.Models;
using System.Text.Json.Serialization;

namespace Hotdesk.Models
{
    public enum BookingDurationType
    {
        Custom,
        WholeDay,
        WholeWeek
    }

    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingId { get; set; }

        [ForeignKey(nameof(Desk))]
        public int DeskId { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserName { get; set; } = string.Empty; // Align with User.UserName

        [Required]
        [MaxLength(8)]
        public string UserId { get; set; } = string.Empty; // Align with User.UserId

        public DateTime BookingDate { get; set; }
        public string DurationType { get; set; } = string.Empty;

        public bool CheckedIn { get; set; }
        public DateTime? CheckInTime { get; set; }

        [JsonIgnore]
        public virtual Desk? Desk { get; set; }
    }
}

