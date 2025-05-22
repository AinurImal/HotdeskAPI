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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // POST: api/Bookings
        [HttpPost]
        public async Task<ActionResult<object>> PostBooking(Booking booking)
        {
            // Step 1: Validate UserId
            if (string.IsNullOrWhiteSpace(booking.UserId))
            {
                return BadRequest(new { Message = "UserId is required and cannot be empty." });
            }
            // UserId must be numeric and between 1001 and 9999
            if (!int.TryParse(booking.UserId, out int userIdNumber) || userIdNumber < 1001 || userIdNumber > 9999)
            {
                return BadRequest(new { Message = "UserId must be a number between 1001 and 9999." });
            }
            // UserId must be unique for the same booking date
            if (_context.Booking.Any(b => b.UserId == booking.UserId && b.BookingDate.Date == booking.BookingDate.Date))
            {
                return Conflict(new { Message = "UserId already has a booking for this date." });
            }

            // Step 2: Validate UserName
            if (string.IsNullOrWhiteSpace(booking.UserName))
            {
                return BadRequest(new { Message = "UserName is required and cannot be empty." });
            }
            if (booking.UserName.Length < 2 || booking.UserName.Length > 50)
            {
                return BadRequest(new { Message = "UserName must be between 2 and 50 characters." });
            }

            // Step 3: Validate DeskId
            if (booking.DeskId < 1)
            {
                return BadRequest(new { Message = "DeskId must be a positive integer." });
            }
            var desk = await _context.Desk.FindAsync(booking.DeskId);
            if (desk == null)
            {
                return BadRequest(new { Message = "Desk not found." });
            }
            if (!desk.IsAvailable)
            {
                return BadRequest(new { Message = "Desk is not available." });
            }

            // Step 4: Validate BookingDate (cannot be in the past)
            if (booking.BookingDate.Date < DateTime.Today)
            {
                return BadRequest(new { Message = "BookingDate cannot be in the past." });
            }

            // Step 5: Only allow "Daily" as DurationType
            var durationType = booking.DurationType?.Trim().ToLower();
            if (durationType != "daily")
            {
                return BadRequest(new { Message = "DurationType must be 'Daily' (case-insensitive)." });
            }

            // Step 6: Check for overlapping bookings for the same desk and date
            var overlappingBooking = await _context.Booking
                .Where(b => b.DeskId == booking.DeskId && b.BookingDate.Date == booking.BookingDate.Date)
                .FirstOrDefaultAsync();

            if (overlappingBooking != null)
            {
                return BadRequest(new { Message = "The desk is already booked for the selected date." });
            }

            // Step 7: Set Desk availability to false
            desk.IsAvailable = false;
            _context.Entry(desk).State = EntityState.Modified;

            // Step 8: Save booking
            _context.Booking.Add(booking);
            await _context.SaveChangesAsync();

            // Step 9: Add to BookFinder
            var bookFinder = new BookFinder
            {
                BookingId = booking.BookingId,
                DeskId = booking.DeskId,
                UserId = booking.UserId,
                UserName = booking.UserName,
                BookingDate = booking.BookingDate,
                CheckedIn = booking.CheckedIn,
                CheckInTime = booking.CheckInTime,
                IsAvailable = false
            };
            _context.BookFinder.Add(bookFinder);
            await _context.SaveChangesAsync();

            // Step 10: Return booking info
            return Ok(new
            {
                Message = $"Booking created for this working day.",
                booking.BookingId,
                booking.DeskId,
                booking.UserId,
                booking.UserName,
                booking.BookingDate,
                booking.DurationType
            });
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
