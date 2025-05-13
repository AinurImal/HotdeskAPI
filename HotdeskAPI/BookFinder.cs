using System.ComponentModel.DataAnnotations;

namespace Hotdesk.Components.Models
{
    public class BookFinder
    {
        [Key]
        public int BookingId { get; set; } // Unique identifier for the booking
        public int DeskId { get; set; } // Associated desk ID
        public string UserId { get; set; } = string.Empty; // User ID who made the booking
        public string UserName { get; set; } = string.Empty; // User's name
        public DateTime BookingDate { get; set; } // Date of the booking
        public DateTime StartTime { get; set; } // Start time of the booking
        public DateTime EndTime { get; set; } // End time of the booking
        public bool CheckedIn { get; set; } // Whether the user has checked in
        public DateTime? CheckInTime { get; set; } // Time of check-in, if applicable
    }
}

