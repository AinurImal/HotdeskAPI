using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hotdesk.Components.Models;
using HotdeskAPI.Data;

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
        public async Task<ActionResult<IEnumerable<BookFinder>>> GetBookFinder()
        {
            // Query the Booking table and map to BookFinder
            var bookings = await _context.Booking
                .Select(b => new BookFinder
                {
                    BookingId = b.BookingId,
                    DeskId = b.DeskId,
                    UserId = b.UserId,
                    UserName = b.UserName,
                    BookingDate = b.BookingDate,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    CheckedIn = b.CheckedIn,
                    CheckInTime = b.CheckInTime
                })
                .ToListAsync();

            return Ok(bookings);
        }

        // GET: api/BookFinders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookFinder>> GetBookFinder(int id)
        {
            // Query the Booking table by BookingId and map to BookFinder
            var booking = await _context.Booking
                .Where(b => b.BookingId == id)
                .Select(b => new BookFinder
                {
                    BookingId = b.BookingId,
                    DeskId = b.DeskId,
                    UserId = b.UserId,
                    UserName = b.UserName,
                    BookingDate = b.BookingDate,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    CheckedIn = b.CheckedIn,
                    CheckInTime = b.CheckInTime
                })
                .FirstOrDefaultAsync();

            if (booking == null)
            {
                return NotFound($"Booking with ID {id} not found.");
            }

            return Ok(booking);
        }
    }
}

