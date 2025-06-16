using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hotdesk.Models;
using HotdeskAPI.Data;
using Hotdesk.Components.Models;

namespace HotdeskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly HotdeskAPIContext _context;

        public BookingsController(HotdeskAPIContext context)
        {
            _context = context;
        }

        // GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBooking()
        {
            return await _context.Booking.ToListAsync();
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Booking.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            return booking;
        }

        // PUT: api/Bookings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking(int id, Booking booking)
        {
            if (id != booking.BookingId)
            {
                return BadRequest();
            }

            _context.Entry(booking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Bookings
        [HttpPost]
        public async Task<ActionResult<object>> PostBooking(Booking booking)
        {
            // Step 1: Validate UserId
            if (booking.UserId == Guid.Empty)
                return BadRequest(new { Message = "UserId is required and cannot be empty." });

            // Step 2: Lookup user by UserId
            var user = await _context.User.FirstOrDefaultAsync(u => u.UserId == booking.UserId);
            if (user == null)
                return BadRequest(new { Message = "UserId does not exist. Please register the user first." });

            // Step 3: Only allow "Daily" as DurationType
            var durationType = booking.DurationType?.Trim().ToLower();
            if (durationType != "daily")
                return BadRequest(new { Message = "DurationType must be 'Daily' (case-insensitive)." });

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Step 4: Prevent double booking for the same desk and date
                var overlappingBooking = await _context.Booking
                    .AnyAsync(b => b.DeskId == booking.DeskId && b.BookingDate.Date == booking.BookingDate.Date);

                if (overlappingBooking)
                {
                    return BadRequest(new { Message = "The desk is already booked for the selected date. Please choose another date." });
                }

                // Step 5: Save booking
                _context.Booking.Add(booking);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Step 6: Return booking info
                return Ok(new
                {
                    Message = $"Booking created for this working day.",
                    booking.BookingId,
                    booking.DeskId,
                    booking.UserId,
                    booking.BookingDate,
                    booking.DurationType
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        // DELETE: api/Bookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.BookingId == id);
        }
    }
}


