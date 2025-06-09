using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hotdesk.Components.Models;
using HotdeskAPI.Data;

namespace HotdeskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesksController : ControllerBase
    {
        private readonly HotdeskAPIContext _context;

        public DesksController(HotdeskAPIContext context)
        {
            _context = context;
        }

        // GET: api/Desks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Desk>>> GetDesk()
        {
            return await _context.Desk.ToListAsync();
        }

        // GET: api/Desks/5
        // GET: api/Desks/{id}/availability?date=2025-06-06
        [HttpGet("{id}/availability")]
        public async Task<ActionResult<object>> CheckDeskAvailability(int id, [FromQuery] string date)
        {
            var desk = await _context.Desk.FindAsync(id);
            if (desk == null)
                return NotFound(new { Message = "Desk not found." });

            // Accept both dd/MM/yyyy and yyyy/MM/dd formats
            string[] formats = { "dd/MM/yyyy", "yyyy/MM/dd", "yyyy-MM-dd" };
            if (!DateTime.TryParseExact(date, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                return BadRequest(new { Message = "Invalid date format. Please use dd/MM/yyyy or yyyy/MM/dd." });
            }

            bool isBooked = await _context.Booking
                .AnyAsync(b => b.DeskId == id && b.BookingDate.Date == parsedDate.Date);

            return Ok(new
            {
                DeskId = id,
                Date = parsedDate.Date,
                IsAvailable = !isBooked,
                Message = isBooked
                    ? "The desk is already booked for the selected date. Please choose another date."
                    : "The desk is available for the selected date."
            });
        }



        // PUT: api/Desks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDesk(int id, Desk desk)
        {
            // Step 1: Validate the input
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }

            // Step 2: Check if the ID in the route matches the DeskId in the body
            if (id != desk.DeskId)
            {
                return BadRequest("The Desk ID in the URL does not match the Desk ID in the request body.");
            }

            // Step 3: Check if the DeskId is within the valid range
            if (desk.DeskId < 1 || desk.DeskId > 9999)
            {
                // Generate a list of available unique Desk IDs within the valid range
                var existingIds = _context.Desk.Select(d => d.DeskId).ToHashSet();
                var availableIds = Enumerable.Range(1, 9999).Except(existingIds).Take(5).ToList();

                return BadRequest(new
                {
                    Message = "The Desk ID must be within the range of 1 to 9999. Please provide a valid Desk ID.",
                    AvailableIds = availableIds
                });
            }

            // Step 4: Check if the DeskId already exists (excluding the current desk being updated)
            if (_context.Desk.Any(d => d.DeskId == desk.DeskId && d.DeskId != id))
            {
                // Generate a list of available unique Desk IDs
                var existingIds = _context.Desk.Select(d => d.DeskId).ToHashSet();
                var availableIds = Enumerable.Range(1, 9999).Except(existingIds).Take(5).ToList();

                return Conflict(new
                {
                    Message = "The Desk ID already exists. Please provide a unique Desk ID.",
                    AvailableIds = availableIds
                });
            }

            // Step 5: Begin a transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Update the desk in the database
                _context.Entry(desk).State = EntityState.Modified;

                // Save changes
                await _context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();

                // Return success response
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Rollback the transaction in case of a concurrency issue
                await transaction.RollbackAsync();

                if (!DeskExists(id))
                {
                    return NotFound("The desk does not exist.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of any other error
                await transaction.RollbackAsync();

                // Log the exception (optional) and return an error response
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }


        // POST: api/Desks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Desk>> PostDesk(Desk desk)
        {
            // Step 1: Validate the input
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }

            // Step 2: Check if the DeskId is within the valid range
            if (desk.DeskId < 1 || desk.DeskId > 9999)
            {
                // Generate a list of available unique Desk IDs within the valid range
                var existingIds = _context.Desk.Select(d => d.DeskId).ToHashSet();
                var availableIds = Enumerable.Range(1, 9999).Except(existingIds).Take(5).ToList();

                return BadRequest(new
                {
                    Message = "The Desk ID must be within the range of 1 to 9999. Please provide a valid Desk ID.",
                    AvailableIds = availableIds
                });
            }

            // Step 3: Check if the DeskId already exists
            if (DeskExists(desk.DeskId))
            {
                // Generate a list of available unique Desk IDs
                var existingIds = _context.Desk.Select(d => d.DeskId).ToHashSet();
                var availableIds = Enumerable.Range(1, 9999).Except(existingIds).Take(5).ToList();

                return Conflict(new
                {
                    Message = "The Desk ID already exists. Please provide a unique Desk ID.",
                    AvailableIds = availableIds
                });
            }

            // Step 4: Begin a transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Add the desk to the database
                _context.Desk.Add(desk);

                // Save changes
                await _context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();

                // Return the created desk
                return CreatedAtAction("GetDesk", new { id = desk.DeskId }, desk);
            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of an error
                await transaction.RollbackAsync();

                // Log the exception (optional) and rethrow or return an error response
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }






        // DELETE: api/Desks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDesk(int id)
        {
            var desk = await _context.Desk.FindAsync(id);
            if (desk == null)
            {
                return NotFound();
            }

            _context.Desk.Remove(desk);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DeskExists(int id)
        {
            return _context.Desk.Any(e => e.DeskId == id);
        }
    }
}
