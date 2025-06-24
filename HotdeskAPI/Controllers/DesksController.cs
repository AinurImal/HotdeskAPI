// Import required system and framework namespaces
using System; // Provides base functionalities like DateTime, Exception, etc.
using System.Collections.Generic; // Enables use of generic collections like List<>
using System.Linq; // Enables LINQ queries
using System.Threading.Tasks; // Enables asynchronous programming
using Microsoft.AspNetCore.Http; // Provides types for handling HTTP features
using Microsoft.AspNetCore.Mvc; // Contains classes for API controller functionality
using Microsoft.EntityFrameworkCore; // Provides Entity Framework Core ORM capabilities
using Hotdesk.Components.Models; // Imports application-specific models (e.g., Desk)
using HotdeskAPI.Data; // Imports the database context for Hotdesk API


namespace HotdeskAPI.Controllers // Define the namespace for this controller
{
    
    [Route("api/[controller]")] // Set the route for this controller to "api/[controller]" (in this case, api/Desks) 
    [ApiController] // Mark this class as an API controller for automatic model validation and JSON formatting
    public class DesksController : ControllerBase // Define the controller class named DesksController that inherits from ControllerBase
    {
        
        private readonly HotdeskAPIContext _context; // Declare a private read-only field for the database context

        
        public DesksController(HotdeskAPIContext context) // Constructor that receives the database context through dependency injection
        {
            _context = context; // Assign the injected context to the private field
        }

        // HTTP GET endpoint to retrieve all desk records
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Desk>>> GetDesk()
        {
            // Asynchronously retrieve all desks from the database and return them
            return await _context.Desk.ToListAsync();
        }

        // HTTP GET endpoint to check desk availability by desk ID and date
        [HttpGet("{id}/availability")]
        public async Task<ActionResult<object>> CheckDeskAvailability(int id, [FromQuery] string date)
        {
            // Find a desk by its ID asynchronously
            var desk = await _context.Desk.FindAsync(id);
            // If desk not found, return 404 Not Found with custom message
            if (desk == null)
                return NotFound(new { Message = "Desk not found." });

            // Define acceptable date formats
            string[] formats = { "dd/MM/yyyy" };
            // Try to parse the input date using one of the formats
            if (!DateTime.TryParseExact(date, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                // Return 400 Bad Request if date format is invalid
                return BadRequest(new { Message = "Invalid date format. Please use dd/MM/yyyy." });
            }

            // Check if a booking exists for the given desk and date
            bool isBooked = await _context.Booking
                .AnyAsync(b => b.DeskId == id && b.BookingDate.Date == parsedDate.Date);

            // Return availability result as a JSON object
            return Ok(new
            {
                DeskId = id, // Return desk ID
                Date = parsedDate.Date, // Return parsed date
                IsAvailable = !isBooked, // True if not booked
                Message = isBooked
                    ? "The desk is already booked for the selected date. Please choose another date." // If booked
                    : "The desk is available for the selected date." // If available
            });
        }

        // HTTP PUT endpoint to update a desk by ID
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDesk(int id, Desk desk)
        {
            // Check if model is valid according to data annotations
            if (!ModelState.IsValid)
            {
                // Return 400 Bad Request if model is invalid
                return BadRequest(ModelState);
            }

            // Check if URL ID matches Desk ID in the request body
            if (id != desk.DeskId)
            {
                // Return 400 if IDs do not match
                return BadRequest("The Desk ID in the URL does not match the Desk ID in the request body.");
            }

            // Check if Desk ID is within the allowed range (1 to 9999)
            if (desk.DeskId < 1 || desk.DeskId > 9999)
            {
                // Get all existing desk IDs
                var existingIds = _context.Desk.Select(d => d.DeskId).ToHashSet();
                // Generate 5 available IDs not in use
                var availableIds = Enumerable.Range(1, 9999).Except(existingIds).Take(5).ToList();

                // Return 400 with suggestion of available IDs
                return BadRequest(new
                {
                    Message = "The Desk ID must be within the range of 1 to 9999. Please provide a valid Desk ID.",
                    AvailableIds = availableIds
                });
            }

            // Check if the Desk ID already exists and is used by a different desk
            if (_context.Desk.Any(d => d.DeskId == desk.DeskId && d.DeskId != id))
            {
                // Get current desk IDs
                var existingIds = _context.Desk.Select(d => d.DeskId).ToHashSet();
                // Generate list of alternative available IDs
                var availableIds = Enumerable.Range(1, 9999).Except(existingIds).Take(5).ToList();

                // Return 409 Conflict with message and alternatives
                return Conflict(new
                {
                    Message = "The Desk ID already exists. Please provide a unique Desk ID.",
                    AvailableIds = availableIds
                });
            }

            // Begin a new database transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Mark the desk as modified for EF Core
                _context.Entry(desk).State = EntityState.Modified;

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Commit the transaction if everything is successful
                await transaction.CommitAsync();

                // Return 204 No Content on success
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Rollback transaction if concurrency exception occurs
                await transaction.RollbackAsync();

                // Check if the desk exists after the failure
                if (!DeskExists(id))
                {
                    // Return 404 if desk no longer exists
                    return NotFound("The desk does not exist.");
                }
                else
                {
                    // Rethrow the exception to be handled by ASP.NET Core
                    throw;
                }
            }
            catch (Exception ex)
            {
                // Rollback transaction on any other error
                await transaction.RollbackAsync();

                // Return 500 Internal Server Error with exception message
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        // HTTP POST endpoint to create a new desk
        [HttpPost]
        public async Task<ActionResult<Desk>> PostDesk(Desk desk)
        {
            // Check model validity
            if (!ModelState.IsValid)
            {
                // Return validation errors
                return BadRequest(ModelState);
            }

            // Check if Desk ID is in the valid range
            if (desk.DeskId < 1 || desk.DeskId > 9999)
            {
                var existingIds = _context.Desk.Select(d => d.DeskId).ToHashSet();
                var availableIds = Enumerable.Range(1, 9999).Except(existingIds).Take(5).ToList();

                // Return 400 with suggestions
                return BadRequest(new
                {
                    Message = "The Desk ID must be within the range of 1 to 9999. Please provide a valid Desk ID.",
                    AvailableIds = availableIds
                });
            }

            // Check if Desk ID is already taken
            if (DeskExists(desk.DeskId))
            {
                var existingIds = _context.Desk.Select(d => d.DeskId).ToHashSet();
                var availableIds = Enumerable.Range(1, 9999).Except(existingIds).Take(5).ToList();

                // Return 409 Conflict
                return Conflict(new
                {
                    Message = "The Desk ID already exists. Please provide a unique Desk ID.",
                    AvailableIds = availableIds
                });
            }

            // Begin a database transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Add the new desk to the database context
                _context.Desk.Add(desk);

                // Save changes
                await _context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();

                // Return 201 Created with link to GET method
                return CreatedAtAction("GetDesk", new { id = desk.DeskId }, desk);
            }
            catch (Exception ex)
            {
                // Rollback transaction if exception occurs
                await transaction.RollbackAsync();

                // Return 500 error with exception details
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        // HTTP DELETE endpoint to remove a desk by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDesk(int id)
        {
            // Find the desk by ID
            var desk = await _context.Desk.FindAsync(id);
            // Return 404 if desk not found
            if (desk == null)
            {
                return NotFound();
            }

            // Remove the desk from the context
            _context.Desk.Remove(desk);
            // Save changes to apply delete
            await _context.SaveChangesAsync();

            // Return 204 No Content
            return NoContent();
        }

        // Helper method to check if a desk exists by ID
        private bool DeskExists(int id)
        {
            // Return true if a desk with given ID exists
            return _context.Desk.Any(e => e.DeskId == id);
        }
    }
}
