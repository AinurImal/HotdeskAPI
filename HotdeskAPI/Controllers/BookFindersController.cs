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
            return await _context.BookFinder.ToListAsync();
        }

        // GET: api/BookFinders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookFinder>> GetBookFinder(int id)
        {
            var bookFinder = await _context.BookFinder.FindAsync(id);

            if (bookFinder == null)
            {
                return NotFound();
            }

            return bookFinder;
        }


        // PUT: api/BookFinders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookFinder(int id, BookFinder bookFinder)
        {
            if (id != bookFinder.BookingId)
            {
                return BadRequest();
            }

            _context.Entry(bookFinder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookFinderExists(id))
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

        // POST: api/BookFinders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BookFinder>> PostBookFinder(BookFinder bookFinder)
        {
            _context.BookFinder.Add(bookFinder);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBookFinder", new { id = bookFinder.BookingId }, bookFinder);
        }

        // DELETE: api/BookFinders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookFinder(int id)
        {
            var bookFinder = await _context.BookFinder.FindAsync(id);
            if (bookFinder == null)
            {
                return NotFound();
            }

            _context.BookFinder.Remove(bookFinder);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookFinderExists(int id)
        {
            return _context.BookFinder.Any(e => e.BookingId == id);
        }
    }
}
