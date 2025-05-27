using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hotdesk.Models;
using HotdeskAPI.Data;

namespace HotdeskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly HotdeskAPIContext _context;

        public UsersController(HotdeskAPIContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.User.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(user.FullName))
                return BadRequest(new { Message = "FullName is required." });
            if (string.IsNullOrWhiteSpace(user.UserName))
                return BadRequest(new { Message = "UserName is required." });
            if (string.IsNullOrWhiteSpace(user.PhoneNumber))
                return BadRequest(new { Message = "PhoneNumber is required." });
            if (!System.Text.RegularExpressions.Regex.IsMatch(user.PhoneNumber, @"^0\d{9}$"))
                return BadRequest(new { Message = "PhoneNumber must be in the format 0123456789." });

            // Check for existing user with same UserName and PhoneNumber
            var existingUser = await _context.User
                .FirstOrDefaultAsync(u => u.UserName == user.UserName && u.PhoneNumber == user.PhoneNumber);
            if (existingUser != null)
                return Conflict(new { Message = "User already exists.", UserId = existingUser.UserId });

            // Generate unique UserId (e.g., next available 4-digit number as string)
            string newUserId;
            var usedIds = _context.User.Select(u => u.UserId).ToHashSet();
            int candidate = 1001;
            do
            {
                newUserId = candidate.ToString();
                candidate++;
            } while (usedIds.Contains(newUserId));
            user.UserId = newUserId;

            _context.User.Add(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserExists(user.UserId))
                    return Conflict();
                else
                    throw;
            }

            return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        }


        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(string id)
        {
            return _context.User.Any(e => e.UserId == id);
        }
    }
}
