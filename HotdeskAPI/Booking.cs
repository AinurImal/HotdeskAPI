using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hotdesk.Components.Models;
using System.Text.Json.Serialization;

namespace Hotdesk.Models
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-generated
        public int BookingId { get; set; }

        [ForeignKey(nameof(Desk))]
        public int DeskId { get; set; }

        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        public DateTime BookingDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public bool CheckedIn { get; set; }
        public DateTime? CheckInTime { get; set; }

        // Optional: include desk reference but exclude from JSON to prevent circular references
        [JsonIgnore]
        public virtual Desk? Desk { get; set; }
    }
}
