// Required namespaces for ASP.NET Core MVC functionality and EF Core database operations
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotdeskAPI.Data; // Namespace for database context
using Hotdesk.Components.Models; // Namespace for data models
using System.Collections.Generic; // For using List<T> and IEnumerable<T>
using System.Linq; // For LINQ queries
using System.Threading.Tasks; // For async/await support

// Namespace for API controllers
namespace HotdeskAPI.Controllers
{
    // Route attribute defines the base route for this controller as "api/BookFinders"
    // ApiController attribute enables API-specific behaviors such as model validation
    [Route("api/[controller]")]
    [ApiController]
    public class BookFindersController(HotdeskAPIContext context) : ControllerBase // Inherits from ControllerBase for Web API support
    {
        // Dependency injection: Database context injected and assigned to a private field
        private readonly HotdeskAPIContext _context = context;

        // GET: api/BookFinders
        // Retrieves all booking data by joining Booking, User, and Desk tables
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookFinder>>> GetBookFinders()
        {
            // LINQ query joins Booking, User, and Desk entities to shape the result into a BookFinder model
            var results = await (from b in _context.Booking
                                 join u in _context.User on b.UserName equals u.UserName
                                 join d in _context.Desk on b.DeskId equals d.DeskId
                                 select new BookFinder
                                 {
                                     BookingId = b.BookingId, // Booking ID from Booking table
                                     UserName = b.UserName, // Username from Booking table
                                     UserId = u.UserId.ToString(), // User ID from User table (converted to string)
                                     PhoneNumber = u.PhoneNumber, // User's phone number from User table
                                     DeskName = d.Name, // Desk name from Desk table
                                     Location = d.Location, // Desk location from Desk table
                                     BookingDate = b.BookingDate, // Booking date from Booking table
                                     DurationType = b.DurationType // Duration type from Booking table
                                 }).ToListAsync(); // Asynchronously executes the query and returns results as a list

            // Returns the result with HTTP 200 OK
            return Ok(results);
        }

        // GET: api/BookFinders/{id}?searchBy=desk|user|booking
        // Retrieves filtered booking data based on the specified search criteria (desk id, user name, or booking id)
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<BookFinder>>> GetBookFinder(string id, [FromQuery] string searchBy)
        {
            // Start with a base query for bookings
            IQueryable<Hotdesk.Models.Booking> query = _context.Booking;

            // Check the value of 'searchBy' query parameter and filter the query accordingly
            if (searchBy?.ToLower() == "desk" && int.TryParse(id, out int deskId))
                query = query.Where(b => b.DeskId == deskId); // Filter by desk ID if searchBy is "desk"
            else if (searchBy?.ToLower() == "user")
                query = query.Where(b => b.UserName == id); // Filter by user name if searchBy is "user"
            else if (searchBy?.ToLower() == "booking" && int.TryParse(id, out int bookingId))
                query = query.Where(b => b.BookingId == bookingId); // Filter by booking ID if searchBy is "booking"
            else
                // Return HTTP 400 Bad Request if searchBy value is invalid or id is not parsable
                return BadRequest(new { Message = "searchBy must be 'desk', 'user', or 'booking', and id must be a valid value." });

            // Join the filtered booking data with user and desk data to build the BookFinder result
            var results = await (from b in query
                                 join u in _context.User on b.UserName equals u.UserName
                                 join d in _context.Desk on b.DeskId equals d.DeskId
                                 select new BookFinder
                                 {
                                     BookingId = b.BookingId, // Booking ID
                                     UserName = b.UserName, // Username
                                     UserId = u.UserId.ToString(), // User ID as string
                                     PhoneNumber = u.PhoneNumber, // User's phone number
                                     DeskName = d.Name, // Desk name
                                     Location = d.Location, // Desk location
                                     BookingDate = b.BookingDate, // Booking date
                                     DurationType = b.DurationType // Duration type
                                 }).ToListAsync(); // Execute and convert result to list

            // Return HTTP 404 Not Found if no data is returned
            if (results.Count == 0)
                return NotFound();

            // Return HTTP 200 OK with the filtered booking data
            return Ok(results);
        }

        // DELETE: api/BookFinders/{bookingId}
        // Deletes a booking by its booking ID
        [HttpDelete("{bookingId}")]
        public async Task<IActionResult> DeleteBooking(int bookingId)
        {
            // Attempt to find the booking by ID
            var booking = await _context.Booking.FindAsync(bookingId);

            // Return HTTP 404 Not Found if booking doesn't exist
            if (booking == null)
            {
                return NotFound(new { Message = "Booking not found." });
            }

            // Remove the booking from the database
            _context.Booking.Remove(booking);

            // Save changes to the database asynchronously
            await _context.SaveChangesAsync();

            // Return HTTP 204 No Content to indicate successful deletion
            return NoContent();
        }
    }
}




