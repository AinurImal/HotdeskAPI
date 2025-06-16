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
        public int BookingId { get; set; }

        [ForeignKey(nameof(Desk))]
        public int DeskId { get; set; }

        [Required]
        [MaxLength(8)]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; } // FK to User.UserId


        public DateTime BookingDate { get; set; }
        public string DurationType { get; set; } = string.Empty;
        public bool CheckedIn { get; set; }
        public DateTime? CheckInTime { get; set; }

        [JsonIgnore]
        public virtual Desk? Desk { get; set; }

        [JsonIgnore]
        public virtual User? User { get; set; }
    }



}



