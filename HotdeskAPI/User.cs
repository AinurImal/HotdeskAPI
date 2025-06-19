// These namespaces provide required features for entity definitions, validation
using System; // For Guid type, allows you to use all the types in the System namespace directly, making your C# code simpler and more concise.
using System.ComponentModel.DataAnnotations; // For validation attributes like [Required], [MaxLength], etc.
using System.ComponentModel.DataAnnotations.Schema;  // For data schema attributes, e.g., [Index]
using Microsoft.EntityFrameworkCore; // Required for [Index] attribute and other Entity Framework Core features

namespace HotdeskAPI
{
    // Add unique indexes for UserName, PhoneNumber, and Email to enforce uniqueness and improve query performance
    [Index(nameof(UserName), IsUnique = true)] // Ensures UserName is unique in the database
    [Index(nameof(PhoneNumber), IsUnique = true)] // Ensures PhoneNumber is unique in the database
    [Index(nameof(Email), IsUnique = true)] // Ensures Email is unique in the database
    public class User
    {
        [Key] // Marks this property as the primary key
        public Guid UserId { get; set; } = Guid.NewGuid(); // Unique identifier for each user, auto-generated

        [Required] // FullName must be provided, ensure the property is not null and not empty. This also triggers validation in the database
        [MaxLength(100)] // Limits FullName to 100 characters
        //[MaxLength] enforces a maximum size for a property, helping maintain data quality, prevent errors, and optimize storage.
        public string FullName { get; set; } = string.Empty; // Stores the user's full name

        [Required] // UserName must be provided, ensure the property is not null and not empty. This also triggers validation in the database
        [MaxLength(50)] // Limits UserName to 50 characters
        public string UserName { get; set; } = string.Empty; // Stores the user's username (must be unique)

        [Required] // PhoneNumber must be provided
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must be in the format 0123456789.")]
        // Validates phone number format
        //[RegularExpression] ensures that a property’s value matches a defined pattern, helping maintain correct and predictable data input in your application.
        public string PhoneNumber { get; set; } = string.Empty; // Stores the user's phone number (must be unique)

        [Required] // Email must be provided
        [EmailAddress] // Validates email format
        [MaxLength(100)] // Limits Email to 100 characters
        public string Email { get; set; } = string.Empty; // Stores the user's email (must be unique)

        // Add navigation properties here if you want to reference related entities (e.g., bookings)
    }
}






