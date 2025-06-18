using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            if (string.IsNullOrWhiteSpace(user.Email))
                return BadRequest(new { Message = "Email is required." });
            if (!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(user.Email))
                return BadRequest(new { Message = "Email is not valid." });

            // Case-insensitive uniqueness check
            var existingUser = await _context.User
                .FirstOrDefaultAsync(u =>
                    u.UserName.ToLower() == user.UserName.ToLower() ||
                    u.PhoneNumber == user.PhoneNumber ||
                    u.Email.ToLower() == user.Email.ToLower());

            if (existingUser != null)
            {
                if (existingUser.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase))
                    return Conflict(new { Message = "Email already exists.", Email = existingUser.Email });
                if (existingUser.UserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase))
                    return Conflict(new { Message = "UserName already exists.", UserName = existingUser.UserName });
                if (existingUser.PhoneNumber == user.PhoneNumber)
                    return Conflict(new { Message = "PhoneNumber already exists.", PhoneNumber = existingUser.PhoneNumber });
            }

            // Ensure UserId is unique and not set by client
            if (user.UserId == Guid.Empty || await _context.User.AnyAsync(u => u.UserId == user.UserId))
            {
                user.UserId = Guid.NewGuid();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.User.Add(user);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction("GetUser", new { id = user.UserId }, user);
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();

                // Double-check for unique constraint violation
                if (await _context.User.AnyAsync(u => u.UserName.ToLower() == user.UserName.ToLower()))
                    return Conflict(new { Message = "UserName already exists.", UserName = user.UserName });
                if (await _context.User.AnyAsync(u => u.Email.ToLower() == user.Email.ToLower()))
                    return Conflict(new { Message = "Email already exists.", Email = user.Email });
                if (await _context.User.AnyAsync(u => u.PhoneNumber == user.PhoneNumber))
                    return Conflict(new { Message = "PhoneNumber already exists.", PhoneNumber = user.PhoneNumber });

                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred.", Error = dbEx.Message });
            }
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


