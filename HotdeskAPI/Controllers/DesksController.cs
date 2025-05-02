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
        [HttpGet("{id}")]
        public async Task<ActionResult<Desk>> GetDesk(int id)
        {
            var desk = await _context.Desk.FindAsync(id);

            if (desk == null)
            {
                return NotFound();
            }

            return desk;
        }

        // PUT: api/Desks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDesk(int id, Desk desk)
        {
            if (id != desk.DeskId)
            {
                return BadRequest();
            }

            _context.Entry(desk).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeskExists(id))
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

        // POST: api/Desks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Desk>> PostDesk(Desk desk)
        {
            _context.Desk.Add(desk);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DeskExists(desk.DeskId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetDesk", new { id = desk.DeskId }, desk);
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
