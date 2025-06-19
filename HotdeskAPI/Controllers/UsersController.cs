using System; // Provides basic types like Guid
using System.Collections.Generic; // For using collections like List<>
using System.Linq; // Enables LINQ queries
using System.Threading.Tasks; // Supports async/await
using Microsoft.AspNetCore.Http; // For HTTP response status codes
using Microsoft.AspNetCore.Mvc; // For controller and route attributes
using Microsoft.EntityFrameworkCore; // For EF Core database operations
using HotdeskAPI.Data; // Importing the application's data context

namespace HotdeskAPI.Controllers
{
    // Defines this class as an API controller with the route "api/Users"
    [Route("api/[controller]")]
    [ApiController]
    // Primary constructor (C# 12 feature) injecting the HotdeskAPIContext
    public class UsersController(HotdeskAPIContext context) : ControllerBase
    {
        
        private readonly HotdeskAPIContext _context = context; // Initializes the private readonly _context field with the injected context

        // GET: api/Users
        [HttpGet] // Retrieves a list of all users in the system
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.User.ToListAsync(); // Asynchronously returns all users
        }

        // GET: api/Users/{id}
        // Retrieves a specific user by their unique ID
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            var user = await _context.User.FindAsync(id); // Finds user by primary key

            if (user == null)
            {
                return NotFound(); // Returns 404 if the user is not found
            }

            return user; // Returns the found user
        }

        // PUT: api/Users/{id}
        // Updates the information of an existing user
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest(); // Returns 400 if the URL ID doesn't match the User object's ID
            }

            _context.Entry(user).State = EntityState.Modified; // Marks entity as modified

            try
            {
                await _context.SaveChangesAsync(); // Saves changes to the database
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id)) // Checks if the user still exists
                {
                    return NotFound(); // Returns 404 if user doesn't exist
                }
                else
                {
                    throw; // Rethrows the exception if it's not a not-found issue
                }
            }

            return NoContent(); // Returns 204 No Content on successful update
        }

        // POST: api/Users
        // Creates a new user with validation and uniqueness checks
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            // Validates that FullName is provided
            if (string.IsNullOrWhiteSpace(user.FullName))
                return BadRequest(new { Message = "FullName is required." });

            // Validates that UserName is provided
            if (string.IsNullOrWhiteSpace(user.UserName))
                return BadRequest(new { Message = "UserName is required." });

            // Validates that PhoneNumber is provided
            if (string.IsNullOrWhiteSpace(user.PhoneNumber))
                return BadRequest(new { Message = "PhoneNumber is required." });

            // Validates phone number format (e.g. 0123456789)
            if (!System.Text.RegularExpressions.Regex.IsMatch(user.PhoneNumber, @"^0\d{9}$"))
                return BadRequest(new { Message = "PhoneNumber must be in the format 0123456789." });

            // Validates that Email is provided
            if (string.IsNullOrWhiteSpace(user.Email))
                return BadRequest(new { Message = "Email is required." });

            // Validates email format using built-in attribute
            if (!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(user.Email))
                return BadRequest(new { Message = "Email is not valid." });

            // Checks for existing user by UserName, PhoneNumber, or Email (case-insensitive)
            var existingUser = await _context.User
                .FirstOrDefaultAsync(u =>
                    u.UserName.ToLower() == user.UserName.ToLower() ||
                    u.PhoneNumber == user.PhoneNumber ||
                    u.Email.ToLower() == user.Email.ToLower());

            // Returns specific conflict messages if a duplicate is found
            if (existingUser != null)
            {
                if (existingUser.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase))
                    return Conflict(new { Message = "Email already exists.", Email = existingUser.Email });
                if (existingUser.UserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase))
                    return Conflict(new { Message = "UserName already exists.", UserName = existingUser.UserName });
                if (existingUser.PhoneNumber == user.PhoneNumber)
                    return Conflict(new { Message = "PhoneNumber already exists.", PhoneNumber = existingUser.PhoneNumber });
            }

            // Ensures UserId is unique and auto-generated if empty or duplicated
            if (user.UserId == Guid.Empty || await _context.User.AnyAsync(u => u.UserId == user.UserId))
            {
                user.UserId = Guid.NewGuid(); // Generates a new GUID for UserId
            }

            // Starts a database transaction to ensure atomic insert
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.User.Add(user); // Adds the user to the context
                await _context.SaveChangesAsync(); // Saves the new user to the database
                await transaction.CommitAsync(); // Commits the transaction

                // Returns 201 Created with location of new resource
                return CreatedAtAction("GetUser", new { id = user.UserId }, user);
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync(); // Rolls back transaction if error occurs

                // Rechecks for uniqueness violations and returns specific conflict messages
                if (await _context.User.AnyAsync(u => u.UserName.ToLower() == user.UserName.ToLower()))
                    return Conflict(new { Message = "UserName already exists.", UserName = user.UserName });

                if (await _context.User.AnyAsync(u => u.Email.ToLower() == user.Email.ToLower()))
                    return Conflict(new { Message = "Email already exists.", Email = user.Email });

                if (await _context.User.AnyAsync(u => u.PhoneNumber == user.PhoneNumber))
                    return Conflict(new { Message = "PhoneNumber already exists.", PhoneNumber = user.PhoneNumber });

                // Returns generic 500 error if cause is unknown
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred.", Error = dbEx.Message });
            }
        }

        // DELETE: api/Users/{id}
        // Deletes a user by their unique ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.User.FindAsync(id); // Finds user by ID
            if (user == null)
            {
                return NotFound(); // Returns 404 if user doesn't exist
            }

            _context.User.Remove(user); // Marks user for deletion
            await _context.SaveChangesAsync(); // Saves the change to the database

            return NoContent(); // Returns 204 on successful deletion
        }

        // Helper method to check if a user exists by their ID
        private bool UserExists(Guid id)
        {
            return _context.User.Any(e => e.UserId == id); // Returns true if user exists
        }
    }
}



