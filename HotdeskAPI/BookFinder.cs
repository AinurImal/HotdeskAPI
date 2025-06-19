
namespace Hotdesk.Components.Models// Define the namespace where this model resides; helps organize code and prevent naming conflicts
{
    // Define the BookFinder class - this is a Data Transfer Object (DTO) used for presenting
    // combined booking information such as user details, desk info, and booking details.
    public class BookFinder
    {
        
        public int BookingId { get; set; }// Unique identifier for a specific booking
        public string UserName { get; set; } = string.Empty;// Name of the user who made the booking
        public string UserId { get; set; } = string.Empty;// User ID of the person who made the booking
        public string PhoneNumber { get; set; } = string.Empty;// Phone number of the user for contact purposes
        public string DeskName { get; set; } = string.Empty;// Name of the desk that has been booked
        public string Location { get; set; } = string.Empty;// Location of the booked desk (e.g., office, floor, room)
        public DateTime BookingDate { get; set; } // Date and time when the booking was made or is scheduled for
        public string DurationType { get; set; } = string.Empty;// Duration of the booking (e.g., "Full Day", "Half Day", "Hourly")
    }
}


