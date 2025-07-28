using System; // For base types like DateTime, Exception, etc.
using System.Collections.Generic; // For List<T>, IEnumerable<T>
using System.Linq; // For LINQ queries
using System.Threading.Tasks; // For async/await
using Microsoft.AspNetCore.Http; // For StatusCodes
using Microsoft.AspNetCore.Mvc; // For ControllerBase, ActionResult, etc.
using Microsoft.EntityFrameworkCore; // For EF Core features
using HotdeskAPI.Data; // For HotdeskAPIContext
using Hotdesk.Models; // For Booking model

namespace HotdeskAPI.Controllers // Namespace for API controllers
{
    [Route("api/[controller]")] // Route: api/Bookings
    [ApiController] // Enables API-specific behaviors (model validation, etc.)
    public class BookingsController : ControllerBase // Controller for booking endpoints
    {
        private readonly HotdeskAPIContext _context; // Database context

        public BookingsController(HotdeskAPIContext context) // Constructor with DI for context
        {
            _context = context; // Assign context to private field
        }

        // GET: api/Bookings
        [HttpGet] // Handles GET requests to api/Bookings
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            // Return all bookings, including related desk info
            return await _context.Booking
                .Include(b => b.Desk) // Eager load Desk navigation property
                .ToListAsync(); // Execute query asynchronously
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")] // Handles GET requests to api/Bookings/{id}
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            // Find booking by ID, including related desk info
            var booking = await _context.Booking
                .Include(b => b.Desk) // Eager load Desk navigation property
                .FirstOrDefaultAsync(b => b.BookingId == id); // Find booking by ID

            if (booking == null) // If not found
            {
                return NotFound(); // Return 404 Not Found
            }

            return booking; // Return booking
        }

        // POST: api/Bookings
        [HttpPost] // Handles POST requests to api/Bookings
        public async Task<ActionResult<Booking>> PostBooking(Booking booking)
        {
            // Validate DeskId
            if (booking.DeskId < 1)
                return BadRequest(new { Message = "DeskId is required." }); // Return 400 if missing

            // Validate UserName
            if (string.IsNullOrWhiteSpace(booking.UserName))
                return BadRequest(new { Message = "UserName is required." }); // Return 400 if missing

            // Validate BookingDate
            if (booking.BookingDate == default)
                return BadRequest(new { Message = "BookingDate is required." }); // Return 400 if missing

            // Begin a transaction for consistency
            using var transaction = await _context.Database.BeginTransactionAsync(); // Start transaction
            try
            {
                // Check if the desk is already booked for the given date
                bool isBooked = await _context.Booking
                    .AnyAsync(b => b.DeskId == booking.DeskId && b.BookingDate.Date == booking.BookingDate.Date); // Check for existing booking

                if (isBooked) // If already booked
                {
                    await transaction.RollbackAsync(); // Rollback transaction
                    return Conflict(new { Message = "The desk is already booked, please book another desk." }); // Return 409 Conflict
                }

                _context.Booking.Add(booking); // Add new booking to context
                await _context.SaveChangesAsync(); // Save changes to database
                await transaction.CommitAsync(); // Commit transaction

                // Return 201 Created with the new booking
                return CreatedAtAction(nameof(GetBooking), new { id = booking.BookingId }, booking); // Return created booking
            }
            catch (Exception ex) // Catch any exception
            {
                await transaction.RollbackAsync(); // Rollback transaction
                // Return 500 Internal Server Error with error message
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred.", Error = ex.Message });
            }
        }

        // PUT: api/Bookings/5
        [HttpPut("{id}")] // Handles PUT requests to api/Bookings/{id}
        public async Task<IActionResult> PutBooking(int id, Booking booking)
        {
            if (id != booking.BookingId) // Check if ID matches
                return BadRequest(); // Return 400 if not

            _context.Entry(booking).State = EntityState.Modified; // Mark as modified

            try
            {
                await _context.SaveChangesAsync(); // Save changes
            }
            catch (DbUpdateConcurrencyException) // Handle concurrency issues
            {
                if (!BookingExists(id)) // If booking no longer exists
                    return NotFound(); // Return 404
                else
                    throw; // Rethrow exception
            }

            return NoContent(); // Return 204 No Content
        }

        // DELETE: api/Bookings/5
        [HttpDelete("{id}")] // Handles DELETE requests to api/Bookings/{id}
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Booking.FindAsync(id); // Find booking by ID
            if (booking == null) // If not found
                return NotFound(); // Return 404

            _context.Booking.Remove(booking); // Remove booking
            await _context.SaveChangesAsync(); // Save changes

            return NoContent(); // Return 204 No Content
        }

        // Helper method to check if a booking exists by ID
        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.BookingId == id); // Return true if booking exists
        }
    }
}




