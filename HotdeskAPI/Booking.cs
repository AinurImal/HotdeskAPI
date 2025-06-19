using System.ComponentModel.DataAnnotations; // Provides attributes for data validation like [Key], [Required], etc.
using System.ComponentModel.DataAnnotations.Schema; // Allows configuring how the class maps to the database schema
using Hotdesk.Components.Models; // Imports the Desk model (assumed to be defined in Components.Models)
using System.Text.Json.Serialization; // Allows customization of JSON serialization (e.g., ignore navigation properties)
using HotdeskAPI; // Possibly includes shared types, services, or configuration used in the API project

namespace Hotdesk.Models 
{
    public class Booking // Defines the Booking class to represent a desk reservation
    {
        [Key] // Specifies this property as the primary key of the table
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Tells EF to auto-increment the BookingId
        public int BookingId { get; set; } // Unique identifier for each booking (primary key)

        [ForeignKey(nameof(Desk))] // Specifies that DeskId is a foreign key to the Desk table
        public int DeskId { get; set; } // Stores the ID of the desk being booked

        [Required] // Makes UserName mandatory (cannot be null)
        [MaxLength(50)] // Limits the UserName string to 50 characters
        public string UserName { get; set; } = string.Empty; // Stores the name of the user who booked

        public DateTime BookingDate { get; set; } // Stores the date on which the booking is made

        public string DurationType { get; set; } = string.Empty; // Stores the booking duration (e.g., "FullDay", "HalfDay")

        public bool CheckedIn { get; set; } // Indicates whether the user has checked in

        public DateTime? CheckInTime { get; set; } // Stores the time of check-in (nullable since it may not happen immediately)

        [JsonIgnore] // Prevents the Desk object from being serialized in API responses (to avoid circular reference or overload)
        public virtual Desk? Desk { get; set; } // Navigation property to the related Desk entity

        // Optional navigation property to User entity (commented out, can be enabled if User entity is linked)
        // [JsonIgnore] // Prevent serialization if enabled
        // public virtual User? User { get; set; } // Navigation property to the User who made the booking
    }
}





