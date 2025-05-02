using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Hotdesk.Models;

namespace Hotdesk.Components.Models
{
    public class Desk
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Desk ID manually entered
        public int DeskId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool HasMonitor { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string? Description { get; set; }

        // Optional: keep navigation property, but ignore in JSON
        [JsonIgnore]
        public virtual ICollection<Booking>? Bookings { get; set; }
    }
}
