using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotdeskAPI.Data;
using Hotdesk.Components.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HotdeskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookFindersController : ControllerBase
    {
        private readonly HotdeskAPIContext _context;

        public BookFindersController(HotdeskAPIContext context)
        {
            _context = context;
        }

        // GET: api/BookFinders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookFinder>>> GetBookFinders()
        {
            var results = await _context.Booking
                .Include(b => b.User)
                .Include(b => b.Desk)
                .Select(b => new BookFinder
                {
                    BookingId = b.BookingId,
                    UserName = b.User != null ? b.User.UserName : "",
                    UserId = b.UserId,
                    PhoneNumber = b.User != null ? b.User.PhoneNumber : "",
                    DeskName = b.Desk != null ? b.Desk.Name : "",
                    Location = b.Desk != null ? b.Desk.Location : "",
                    BookingDate = b.BookingDate,
                    DurationType = b.DurationType
                })
                .ToListAsync();

            return Ok(results);
        }

        // GET: api/BookFinders/{id}?searchBy=desk|user|booking
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<BookFinder>>> GetBookFinder(int id, [FromQuery] string searchBy)
        {
            IQueryable<Hotdesk.Models.Booking> query = _context.Booking
                .Include(b => b.User)
                .Include(b => b.Desk);

            if (searchBy?.ToLower() == "desk")
                query = query.Where(b => b.DeskId == id);
            else if (searchBy?.ToLower() == "user")
                query = query.Where(b => b.UserId == id.ToString());
            else if (searchBy?.ToLower() == "booking")
                query = query.Where(b => b.BookingId == id);
            else
                return BadRequest(new { Message = "searchBy must be 'desk', 'user', or 'booking'" });

            var results = await query
                .Select(b => new BookFinder
                {
                    BookingId = b.BookingId,
                    UserName = b.User != null ? b.User.UserName : "",
                    UserId = b.UserId,
                    PhoneNumber = b.User != null ? b.User.PhoneNumber : "",
                    DeskName = b.Desk != null ? b.Desk.Name : "",
                    Location = b.Desk != null ? b.Desk.Location : "",
                    DurationType = b.DurationType
                })
                .ToListAsync();

            if (!results.Any())
                return NotFound();

            return Ok(results);
        }
    


        // POST api/<BookFInderController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<BookFInderController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/BookFinders/{bookingId}
        [HttpDelete("{bookingId}")]
        public async Task<IActionResult> DeleteBooking(int bookingId)
        {
            var booking = await _context.Booking.FindAsync(bookingId);
            if (booking == null)
            {
                return NotFound(new { Message = "Booking not found." });
            }

            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}



