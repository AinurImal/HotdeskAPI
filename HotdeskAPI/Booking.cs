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

        public required string UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public string DurationType { get; set; } = string.Empty;

        public bool CheckedIn { get; set; }
        public DateTime? CheckInTime { get; set; }

        [JsonIgnore]
        public virtual Desk? Desk { get; set; }
    }
}
