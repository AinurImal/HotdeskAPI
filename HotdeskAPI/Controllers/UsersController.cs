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
    // Using a primary constructor for dependency injection (C# 12 feature)
    public class UsersController(HotdeskAPIContext context) : ControllerBase
    {
        // The context field is initialized automatically from the primary constructor parameter.
        private readonly HotdeskAPIContext _context = context;

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.User.ToListAsync();
        }

        // GET: api/Users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, User user)
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

            // UserId will be auto-generated as Guid if not set
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

        // DELETE: api/Users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
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

        // Checks if a user exists by Guid UserId
        private bool UserExists(Guid id)
        {
            return _context.User.Any(e => e.UserId == id);
        }
    }
}


