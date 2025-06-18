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
    public class BookFindersController(HotdeskAPIContext context) : ControllerBase
    {
        private readonly HotdeskAPIContext _context = context;

        // GET: api/BookFinders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookFinder>>> GetBookFinders()
        {
            var results = await (from b in _context.Booking
                                 join u in _context.User on b.UserName equals u.UserName
                                 join d in _context.Desk on b.DeskId equals d.DeskId
                                 select new BookFinder
                                 {
                                     BookingId = b.BookingId,
                                     UserName = b.UserName,
                                     UserId = u.UserId.ToString(),
                                     PhoneNumber = u.PhoneNumber,
                                     DeskName = d.Name,
                                     Location = d.Location,
                                     BookingDate = b.BookingDate,
                                     DurationType = b.DurationType
                                 }).ToListAsync();

            return Ok(results);
        }

        // GET: api/BookFinders/{id}?searchBy=desk|user|booking
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<BookFinder>>> GetBookFinder(string id, [FromQuery] string searchBy)
        {
            IQueryable<Hotdesk.Models.Booking> query = _context.Booking;

            if (searchBy?.ToLower() == "desk" && int.TryParse(id, out int deskId))
                query = query.Where(b => b.DeskId == deskId);
            else if (searchBy?.ToLower() == "user")
                query = query.Where(b => b.UserName == id);
            else if (searchBy?.ToLower() == "booking" && int.TryParse(id, out int bookingId))
                query = query.Where(b => b.BookingId == bookingId);
            else
                return BadRequest(new { Message = "searchBy must be 'desk', 'user', or 'booking', and id must be a valid value." });

            var results = await (from b in query
                                 join u in _context.User on b.UserName equals u.UserName
                                 join d in _context.Desk on b.DeskId equals d.DeskId
                                 select new BookFinder
                                 {
                                     BookingId = b.BookingId,
                                     UserName = b.UserName,
                                     UserId = u.UserId.ToString(),
                                     PhoneNumber = u.PhoneNumber,
                                     DeskName = d.Name,
                                     Location = d.Location,
                                     BookingDate = b.BookingDate,
                                     DurationType = b.DurationType
                                 }).ToListAsync();

            if (results.Count == 0)
                return NotFound();

            return Ok(results);
        }
    }
}



