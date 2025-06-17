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
                return BadRequest(new { Message = "FullName is required." }); // FullName must not be empty
            if (string.IsNullOrWhiteSpace(user.UserName))
                return BadRequest(new { Message = "UserName is required." }); // UserName must not be empty
            if (string.IsNullOrWhiteSpace(user.PhoneNumber))
                return BadRequest(new { Message = "PhoneNumber is required." }); // PhoneNumber must not be empty
            if (!System.Text.RegularExpressions.Regex.IsMatch(user.PhoneNumber, @"^0\d{9}$"))
                return BadRequest(new { Message = "PhoneNumber must be in the format 0123456789." }); // PhoneNumber must match pattern
            if (string.IsNullOrWhiteSpace(user.Email))
                return BadRequest(new { Message = "Email is required." }); // Email must not be empty
            if (!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(user.Email)) // IsValid is an instance method, so you must instantiate EmailAddressAttribute before using it.
                return BadRequest(new { Message = "Email is not valid." }); // Email must be valid format

            // Check for existing user with same UserName, PhoneNumber, or Email
            var existingUser = await _context.User
                .FirstOrDefaultAsync(u =>
                    u.UserName == user.UserName ||
                    u.PhoneNumber == user.PhoneNumber ||
                    u.Email == user.Email);

            if (existingUser != null)
            {
                // Return specific message if email is duplicate
                if (existingUser.Email == user.Email)
                    return Conflict(new { Message = "Email already exists.", Email = existingUser.Email });
                if (existingUser.UserName == user.UserName)
                    return Conflict(new { Message = "UserName already exists.", UserName = existingUser.UserName });
                if (existingUser.PhoneNumber == user.PhoneNumber)
                    return Conflict(new { Message = "PhoneNumber already exists.", PhoneNumber = existingUser.PhoneNumber });
            }

            // Use a transaction to ensure atomicity
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.User.Add(user); // Add new user to context
                await _context.SaveChangesAsync(); // Save to database
                await transaction.CommitAsync(); // Commit transaction if successful

                return CreatedAtAction("GetUser", new { id = user.UserId }, user); // Return created user
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Rollback transaction on error
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred.", Error = ex.Message }); // Return error to user
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


