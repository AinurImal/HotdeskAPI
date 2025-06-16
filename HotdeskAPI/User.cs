using System; // For Guid type
using System.ComponentModel.DataAnnotations; // For validation attributes
using System.ComponentModel.DataAnnotations.Schema; // For [Index] attribute
using Microsoft.EntityFrameworkCore; // Required for [Index] attribute and other Entity Framework Core features

namespace HotdeskAPI // Add your project's namespace here
{
    // Add unique indexes for UserName and PhoneNumber to enforce uniqueness and improve query performance
    [Index(nameof(UserName), IsUnique = true)] // Ensures UserName is unique in the database
    [Index(nameof(PhoneNumber), IsUnique = true)] // Ensures PhoneNumber is unique in the database
    public class User
    {
        [Key] // Marks this property as the primary key
        public Guid UserId { get; set; } = Guid.NewGuid(); // Unique identifier for each user, auto-generated

        [Required] // FullName must be provided
        [MaxLength(100)] // Limits FullName to 100 characters
        public string FullName { get; set; } = string.Empty; // Stores the user's full name

        [Required] // UserName must be provided
        [MaxLength(50)] // Limits UserName to 50 characters
        public string UserName { get; set; } = string.Empty; // Stores the user's username (must be unique)

        [Required] // PhoneNumber must be provided
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must be in the format 0123456789.")] // Validates phone number format
        public string PhoneNumber { get; set; } = string.Empty; // Stores the user's phone number (must be unique)

        // Add navigation properties here if you want to reference related entities (e.g., bookings)
    }
}






