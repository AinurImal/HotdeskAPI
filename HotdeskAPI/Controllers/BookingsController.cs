using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotdeskAPI.Data;                    // Import database context
using Hotdesk.Components.Models;         // Import Desk and Booking models
using Hotdesk.Models;                    // Import User model

namespace HotdeskAPI.Controllers
{
    [Route("api/[controller]")]          // Route: api/Bookings
    [ApiController]                      // Enables automatic API behavior (model binding, validation, etc.)
    public class BookingsController(HotdeskAPIContext context) : ControllerBase
    {
        private readonly HotdeskAPIContext _context = context; // Dependency injection of the database context

        // GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            // Get all bookings and include related desk data
            return await _context.Booking
                .Include(b => b.Desk)    // Eager load Desk related to each booking
                .ToListAsync();          // Convert result to list asynchronously
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            // Get a specific booking by its BookingId including its desk info
            var booking = await _context.Booking
                .Include(b => b.Desk)    // Include desk details
                .FirstOrDefaultAsync(b => b.BookingId == id); // Search for booking

            if (booking == null)
            {
                return NotFound();      // Return 404 if booking not found
            }

            return booking;             // Return found booking
        }

        // POST: api/Bookings
        [HttpPost]
        public async Task<ActionResult<Booking>> PostBooking(Booking booking)
        {
            // Validate that the UserName is not empty
            if (string.IsNullOrWhiteSpace(booking.UserName))
                return BadRequest(new { Message = "UserName is required." }); // Return 400 if missing

            // Check if user exists in database
            var user = await _context.User.FirstOrDefaultAsync(u => u.UserName == booking.UserName);
            if (user == null)
                return BadRequest(new { Message = "UserName does not exist. Please register the user first." });

            // Check if the specified desk exists
            var desk = await _context.Desk.FirstOrDefaultAsync(d => d.DeskId == booking.DeskId);
            if (desk == null)
                return BadRequest(new { Message = "Desk does not exist." }); // Return 400 if desk not found

            _context.Booking.Add(booking);             // Add new booking
            await _context.SaveChangesAsync();         // Save changes to database

            // Return 201 Created with location header pointing to the new booking
            return CreatedAtAction(nameof(GetBooking), new { id = booking.BookingId }, booking);
        }

        // PUT: api/Bookings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking(int id, Booking booking)
        {
            if (id != booking.BookingId)
                return BadRequest(); // Return 400 if route ID doesn't match booking ID

            _context.Entry(booking).State = EntityState.Modified; // Mark booking entity as modified

            try
            {
                await _context.SaveChangesAsync(); // Try saving changes
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))            // If booking no longer exists, return 404
                    return NotFound();
                else
                    throw;                         // Otherwise, rethrow the error
            }

            return NoContent(); // Return 204 No Content on successful update
        }

        // DELETE: api/Bookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Booking.FindAsync(id); // Find booking by ID
            if (booking == null)
                return NotFound(); // Return 404 if not found

            _context.Booking.Remove(booking);       // Remove the booking
            await _context.SaveChangesAsync();      // Save changes to database

            return NoContent();                     // Return 204 No Content
        }

        private bool BookingExists(int id)
        {
            // Check if booking with given ID exists
            return _context.Booking.Any(e => e.BookingId == id);
        }
    }
}



