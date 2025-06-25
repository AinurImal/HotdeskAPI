<div align="center">

# Hotdesk Booking

</div>

## What is Hotdesk Booking

Hotdesk Booking is a web application designed to facilitate the booking of hot desks in an office environment. It allows users to view available desks, make reservations, and manage their bookings efficiently.

## Features of Hotdesk Booking
1. **Desk Booking**: Users can book desks for specific dates and times.

2. **Availability Check**: Users can check the availability of desks in real-time.

3. **User Management**: Users can create accounts, log in, and manage their profiles.


## Components of Hotdesk Booking

### Using Directive: All Directive used in the application

|  | Using Directive       | Function        |    
|--|---------------|-----------------------|
|1.| using System;  | Basic .NET types and base classes    | 
|2.| using System.Collections.Generic;  | Provides generic collection types | 
|3.| using System.LINQ;     | Enables LINQ capabilities for querying collection and databases    | 
|4.| using System.Threading.Tasks; | Provides type for asynchronous programming       |
|5.| using Microsoft.AspNetCore.Mvc; | Controllers and MVC features for ASP.NET          |
|6.| using Microsoft.AspNetCore.Http; | Provides types for handling HTTP context, requests, and responses         |
|7.| using Microsoft.EntityFrameworkCore; | Provides Entity Framework Core to work with database and migration |
|8.| using HotdeskAPI.data; | Import project namespoace for database contexrt |
|9.| using HotdeskAPI.Models; | Import model classes   |
|10.| using HotdeskAPI.component.Models; | Import other model classes, allowing to use the type directly |

## Important Attributes

### Attributes used in the application

| Attribute Name | Description |
|----------------|-------------|
| [Key] | Specifies the property that is the primary key of an entity. |
| [Required] | Indicates that a property must have a value. |
|[MaxLength(x)] |	Optimizes storage and prevents oversized data. |
| [ForeignKey] | Specifies a foreign key relationship between two entities. |
| virtual | Indicates that a property or method can be overridden in a derived class. |
| [JsonIgnore] |	Prevents circular reference issues in API responses. |

## Data Models

### Booking Model: Booking.cs

| Code line        | Function description           | 
|---------------|-----------------------|
| public class Booking | Defines the Booking class    | 
| [DatabaseGenerated(DatabaseGeneratedOption.Identity)] | Entity Framework auto-generate this field | 
| public int BookingId { get; set; }     | Booking ID - the unique ID for each booking.         | 
| public int DeskId { get; set; } |       Desk ID - the ID of the desk being booked.         |
| public int UserName { get; set; } |       Stores the name of the user who made the booking.         |
| public DateTime BookingDate { get; set; } | Stores the date when the booking takes place.         |
| public string DurationType { get; set; } = string.Empty; | Stores how long the booking is for (e.g."daily").         |
| public bool CheckedIn { get; set; } | Boolean flag to indicate if the user checked in.        |
| public DateTime? CheckInTime { get; set; } | Stores the time when the user checked in.         |
| public virtual Desk? Desk { get; set; } = null!; | Navigation property to the Desk entity.         |

### Desk Model: Desk.cs
| Code line        | Function description           |
|---------------|-----------------------|
| public class Desk | Defines the Desk class    |
| [DatabaseGenerated(DatabaseGeneratedOption.None)] | Prevents EF Core from auto-generating DeskId; it must be manually assigned. |
| public int DeskId { get; set; } | Desk ID - the unique ID for each desk.         |
| public string Name { get; set; } = string.Empty; | Stores the name of the desk.         |
| public string Location { get; set; } = string.Empty; | Stores the location of the desk.         |
| public bool HasMonitor { get; set; } | Indicates whether the desk includes a monitor. |
| public bool IsAvailable { get; set; } = true; | Boolean flag to indicate if the desk is available for booking.         |
| public string Description { get; set; } = string.Empty; | Stores a description of the desk.         |
| public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>(); | Navigation property to the collection of bookings associated with the desk.         |

### User Model: User.cs
| Code line        | Function description           |
|---------------|-----------------------|
| namespace HotdeskAPI | Defines the namespace for the application    |
| [Index(nameof(UserName), IsUnique = true)] | Adds a unique index on UserName column in DB. Prevents duplicates and improves query performance. |
| [Index(nameof(PhoneNumber), IsUnique = true)] | Adds a unique index on PhoneNumber. |
| [Index(nameof(Email), IsUnique = true)] | Adds a unique index on Email. |
| public class User | Defines the User class    |
| public Guid UserId { get; set; } = Guid.NewGuid(); | Declares a unique identifier for the user. Auto-generates a new GUID when a user is created. |
| public string FullName { get; set; } = string.Empty; | Stores the user's full name. Initializes with empty string. |
| public string UserName { get; set; } = string.Empty; | Stores the user's username (must be unique). |
| [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must be in the format 0123456789.")] | Validates phone format: must start with 0 and contain exactly 10 digits. |
| public string PhoneNumber { get; set; } = string.Empty; | Stores the user's phone number (must be unique). |
| public string Email { get; set; } = string.Empty; | Stores the user's email address (must be unique). |

### BookFinder Model: BookFinder.cs
| Code line        | Function description           |
|---------------|-----------------------|
| public class BookFinder | Defines the BookFinder class    |
| public int BookingId { get; set; } | Unique ID for the booking. Helps identify individual booking records. |
| public string UserName { get; set; } = string.Empty; | Stores the username of the person who made the booking. |
| public string UserId { get; set; } = string.Empty; | Stores the unique identifier of the user (can be a string version of a Guid). |
| public string PhoneNumber { get; set; } = string.Empty; | Stores the user’s phone number for contact purposes. |
| public string DeskName { get; set; } = string.Empty; | Stores the name of the desk that was booked. |
| public string Location { get; set; } = string.Empty; | Stores the physical location of the desk (e.g., "Level 2, Room A"). |
| public DateTime BookingDate { get; set; } | Stores the date and time when the booking is scheduled to occur. |
| public string DurationType { get; set; } = string.Empty; | Stores the duration type of the booking (e.g., "daily"). |

## Controllers

### BookingController.cs
This controller handles booking-related operations such as creating, retrieving, updating, and deleting bookings.
Here are some key methods and attributes used in the BookingController:

| Code line        | Function description           |
|---------------|-----------------------|
| public class BookingController : ControllerBase | Defines the BookingController class, inheriting from ControllerBase. |
| private readonly HotdeskContext _context; | Declares a private field for the database context. |

[HttpGet]

| Code line        | Function description           |
|---------------|-----------------------|
| [HttpGet] | Attribute that indicates this method handles GET requests. |
| public async Task<ActionResult<IEnumerable<Booking>>> GetBookings() | Retrieves all bookings from the database. Returns a list of Booking objects. |\
| _context.Booking | Refers to the Bookings table in the database through Entity Framework. |
| .Include(b => b.Desk) | Eager loads the related Desk entity for each booking |
| .ToListAsync() | Asynchronously retrieves the list of bookings from the database. |

[HttpGet("{id}")] – Get Booking by ID
| Code line        | Function description           |
|---------------|-----------------------|
| [HttpGet("{id}")] | Attribute that indicates this method handles GET requests with a specific booking ID. |
| public async Task<ActionResult<Booking>> GetBooking(int id) | Retrieves a specific booking by its ID. Returns a Booking object if found, or NotFound if not. |
| var booking = await _context.Booking.Include(b => b.Desk).FirstOrDefaultAsync(b => b.BookingId == id); | Queries the booking by ID and includes its related Desk data |
| if (booking == null) | Checks if the booking exists; if not, returns NotFound. |
| return booking; | Returns the found booking. |

[HttpPost] – Create Booking
| Code line        | Function description           |
|---------------|-----------------------|
| [HttpPost] | Attribute that indicates this method handles POST requests for creating a new booking. |
| public async Task<ActionResult<Booking>> PostBooking(Booking booking) | Creates a new booking in the database. |
| if (string.IsNullOrWhiteSpace(booking.UserName)) | Validates that the UserName field is not empty or just whitespace |
| return BadRequest(new { Message = "UserName is required." }); | Returns 400 Bad Request with a message if UserName is invalid |
| var user = await _context.User.FirstOrDefaultAsync(u => u.UserName == booking.UserName); | Searches the database for a user with the given UserName |
| if (user == null) | Checks if the user exists; if not, returns NotFound. |
| return BadRequest(new { Message = "UserName does not exist. Please register the user first." }); | Returns 400 Bad Request with a message if the user does not exist |
| var desk = await _context.Desk.FirstOrDefaultAsync(d => d.DeskId == booking.DeskId); | Searches for the desk by DeskId |
| _context.Booking.Add(booking); | Adds the new booking to the database context |
| await _context.SaveChangesAsync(); | Asynchronously saves changes to the database |
| return CreatedAtAction(nameof(GetBooking), new { id = booking.BookingId }, booking); | Returns 201 Created with the location of the new booking and the booking data |

[HttpPut("{id}")] – Update Booking
| Code line        | Function description           |
|---------------|-----------------------|
| [HttpPut("{id}")] | Attribute that indicates this method handles PUT requests for updating an existing booking by ID. |
| public async Task<IActionResult> PutBooking(int id, Booking booking) | Updates an existing booking in the database. |
| if (id != booking.BookingId) | Checks if the provided ID matches the booking ID; if not, returns BadRequest. |
| context.Entry(booking).State = EntityState.Modified; | Marks the booking entity as modified in the context |
| try { await _context.SaveChangesAsync(); } | Attempts to save changes to the database asynchronously. |
| catch (DbUpdateConcurrencyException) | Catches concurrency exceptions if the booking was modified by another user. |
| if (!BookingExists(id)) | Checks if the booking exists; if not, returns NotFound. |
| return NoContent(); | Returns 204 No Content if the update was successful. |

[HttpDelete("{id}")] – Delete Booking
| Code line        | Function description           |
|---------------|-----------------------|
| [HttpDelete("{id}")] | Attribute that indicates this method handles DELETE requests for deleting a booking by ID. |
| public async Task<IActionResult> DeleteBooking(int id) | Deletes a booking from the database by its ID. |
| var booking = await _context.Booking.FindAsync(id); | Searches for the booking by ID |
| if (booking == null) | Checks if the booking exists; if not, returns NotFound. |	
| _context.Booking.Remove(booking); | Removes the booking from the database context |
| await _context.SaveChangesAsync(); | Asynchronously saves changes to the database |
| return NoContent(); | Returns 204 No Content if the deletion was successful. |

### DeskController.cs
This controller handles desk-related operations such as retrieving available desks, creating new desks, and updating desk information.
| Code line        | Function description           |
|---------------|-----------------------|
| public class DeskController : ControllerBase | Defines the DeskController class, inheriting from ControllerBase. |
| private readonly HotdeskContext _context; | Declares a private field for the database context. |

| [HttpGet] | Attribute that indicates this method handles GET requests. |






