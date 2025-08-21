using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using HotdeskAPI.Controllers;
using HotdeskAPI.Data;
using Hotdesk.Components.Models; // For Desk model
using Xunit;

namespace HotdeskAPI.UnitTest.Controllers
{
    public class DesksControllerTests
    {
        /// <summary>
        /// Test the PostDesk method with valid desk data using the 3A pattern (Arrange, Act, Assert)
        /// This test verifies that when a valid desk is provided, the API returns CreatedAtAction
        /// </summary>
        [Fact]
        public async Task PostDesk_WithValidDesk_ReturnsCreatedAtAction()
        {
            // ARRANGE - Set up the test environment, data, and dependencies
            
            // Create an in-memory database for testing to avoid affecting real database
            var options = new DbContextOptionsBuilder<HotdeskAPIContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB name for test isolation
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)) // Ignore transaction warnings for in-memory DB
                .Options;

            // Create test desk with valid data (this should pass validation)
            var validDesk = new Desk
            {
                DeskId = 1, // Valid - within range 1-9999
                Name = "Desk A1", // Valid - proper desk name
                Location = "2nd Floor, Room 3", // Valid - proper location
                HasMonitor = true, // Valid - boolean value
                IsAvailable = true, // Valid - boolean value
                Description = "Premium desk with dual monitors" // Valid - optional description
            };

            // Set up the database context with in-memory database
            using var context = new HotdeskAPIContext(options);
            
            // Create the controller instance with the test database context
            var controller = new DesksController(context);

            // ACT - Execute the method being tested
            var result = await controller.PostDesk(validDesk);

            // ASSERT - Verify the results meet expectations
            
            // Verify that the result is of type CreatedAtActionResult (successful creation)
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            
            // Verify that the HTTP status code is 201 (Created)
            Assert.Equal(201, createdResult.StatusCode);
            
            // Verify that the created desk is returned
            var createdDesk = Assert.IsType<Desk>(createdResult.Value);
            
            // Verify that the returned desk has the correct properties
            Assert.Equal(1, createdDesk.DeskId);
            Assert.Equal("Desk A1", createdDesk.Name);
            Assert.Equal("2nd Floor, Room 3", createdDesk.Location);
            Assert.True(createdDesk.HasMonitor);
            Assert.True(createdDesk.IsAvailable);
            Assert.Equal("Premium desk with dual monitors", createdDesk.Description);
        }

        /// <summary>
        /// Test the PostDesk method with invalid DeskId using the 3A pattern (Arrange, Act, Assert)
        /// This test verifies that when DeskId is out of range (1-9999), the API returns BadRequest
        /// </summary>
        [Fact]
        public async Task PostDesk_WithInvalidDeskId_ReturnsBadRequest()
        {
            // ARRANGE - Set up the test environment, data, and dependencies
            
            // Create an in-memory database for testing to avoid affecting real database
            var options = new DbContextOptionsBuilder<HotdeskAPIContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB name for test isolation
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)) // Ignore transaction warnings for in-memory DB
                .Options;

            // Create test desk with invalid DeskId (this should trigger validation failure)
            var invalidDesk = new Desk
            {
                DeskId = 10000, // Invalid - exceeds maximum range of 9999
                Name = "Invalid Desk", // Valid - proper desk name
                Location = "Test Location", // Valid - proper location
                HasMonitor = false, // Valid - boolean value
                IsAvailable = true // Valid - boolean value
            };

            // Set up the database context with in-memory database
            using var context = new HotdeskAPIContext(options);
            
            // Create the controller instance with the test database context
            var controller = new DesksController(context);

            // ACT - Execute the method being tested
            var result = await controller.PostDesk(invalidDesk);

            // ASSERT - Verify the results meet expectations
            
            // Verify that the result is of type BadRequestObjectResult
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            
            // Verify that the HTTP status code is 400 (Bad Request)
            Assert.Equal(400, badRequestResult.StatusCode);
            
            // Verify that the error response is not null
            var errorResponse = badRequestResult.Value;
            Assert.NotNull(errorResponse);
            
            // Convert the error response to string and verify it contains the expected message
            var errorMessage = errorResponse.ToString();
            Assert.Contains("The Desk ID must be within the range of 1 to 9999", errorMessage);
        }

        /// <summary>
        /// Test the PostDesk method with duplicate DeskId using the 3A pattern (Arrange, Act, Assert)
        /// This test verifies that when DeskId already exists, the API returns Conflict
        /// </summary>
        [Fact]
        public async Task PostDesk_WithDuplicateDeskId_ReturnsConflict()
        {
            // ARRANGE - Set up the test environment, data, and dependencies
            
            // Create an in-memory database for testing to avoid affecting real database
            var options = new DbContextOptionsBuilder<HotdeskAPIContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB name for test isolation
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)) // Ignore transaction warnings for in-memory DB
                .Options;

            // Set up the database context with in-memory database
            using var context = new HotdeskAPIContext(options);

            // First, add an existing desk to the database
            var existingDesk = new Desk
            {
                DeskId = 1,
                Name = "Existing Desk",
                Location = "1st Floor",
                HasMonitor = true,
                IsAvailable = true
            };
            context.Desk.Add(existingDesk);
            await context.SaveChangesAsync();

            // Create test desk with duplicate DeskId (this should trigger conflict)
            var duplicateDesk = new Desk
            {
                DeskId = 1, // Duplicate - same as existing desk
                Name = "New Desk", // Different name but same ID
                Location = "2nd Floor", // Different location but same ID
                HasMonitor = false,
                IsAvailable = true
            };
            
            // Create the controller instance with the test database context
            var controller = new DesksController(context);

            // ACT - Execute the method being tested
            var result = await controller.PostDesk(duplicateDesk);

            // ASSERT - Verify the results meet expectations
            
            // Verify that the result is of type ConflictObjectResult
            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
            
            // Verify that the HTTP status code is 409 (Conflict)
            Assert.Equal(409, conflictResult.StatusCode);
            
            // Verify that the error response is not null
            var errorResponse = conflictResult.Value;
            Assert.NotNull(errorResponse);
            
            // Convert the error response to string and verify it contains the expected message
            var errorMessage = errorResponse.ToString();
            Assert.Contains("The Desk ID already exists", errorMessage);
        }

        /// <summary>
        /// Test the GetDesk method using the 3A pattern (Arrange, Act, Assert)
        /// This test verifies that GetDesk returns all desks from the database
        /// </summary>
        [Fact]
        public async Task GetDesk_ReturnsAllDesks()
        {
            // ARRANGE - Set up the test environment, data, and dependencies
            
            // Create an in-memory database for testing to avoid affecting real database
            var options = new DbContextOptionsBuilder<HotdeskAPIContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB name for test isolation
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)) // Ignore transaction warnings for in-memory DB
                .Options;

            // Set up the database context with in-memory database
            using var context = new HotdeskAPIContext(options);

            // Add sample desks to the database
            var testDesks = new List<Desk>
            {
                new Desk { DeskId = 1, Name = "Desk A1", Location = "1st Floor", HasMonitor = true, IsAvailable = true },
                new Desk { DeskId = 2, Name = "Desk A2", Location = "1st Floor", HasMonitor = false, IsAvailable = true },
                new Desk { DeskId = 3, Name = "Desk B1", Location = "2nd Floor", HasMonitor = true, IsAvailable = false }
            };

            context.Desk.AddRange(testDesks);
            await context.SaveChangesAsync();
            
            // Create the controller instance with the test database context
            var controller = new DesksController(context);

            // ACT - Execute the method being tested
            var result = await controller.GetDesk();

            // ASSERT - Verify the results meet expectations
            
            // Verify that the result is of type ActionResult<IEnumerable<Desk>>
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Desk>>>(result);
            
            // Extract the list of desks from the result
            var desks = Assert.IsAssignableFrom<IEnumerable<Desk>>(actionResult.Value);
            var deskList = desks.ToList();
            
            // Verify that all 3 test desks are returned
            Assert.Equal(3, deskList.Count);
            
            // Verify that the desks have the correct properties
            Assert.Contains(deskList, d => d.DeskId == 1 && d.Name == "Desk A1");
            Assert.Contains(deskList, d => d.DeskId == 2 && d.Name == "Desk A2");
            Assert.Contains(deskList, d => d.DeskId == 3 && d.Name == "Desk B1");
        }

        /// <summary>
        /// Test the DeleteDesk method with valid DeskId using the 3A pattern (Arrange, Act, Assert)
        /// This test verifies that when a valid DeskId is provided, the desk is deleted successfully
        /// </summary>
        [Fact]
        public async Task DeleteDesk_WithValidDeskId_ReturnsNoContent()
        {
            // ARRANGE - Set up the test environment, data, and dependencies
            
            // Create an in-memory database for testing to avoid affecting real database
            var options = new DbContextOptionsBuilder<HotdeskAPIContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB name for test isolation
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)) // Ignore transaction warnings for in-memory DB
                .Options;

            // Set up the database context with in-memory database
            using var context = new HotdeskAPIContext(options);

            // Add a test desk to delete
            var testDesk = new Desk
            {
                DeskId = 1,
                Name = "Test Desk",
                Location = "Test Location",
                HasMonitor = true,
                IsAvailable = true
            };
            context.Desk.Add(testDesk);
            await context.SaveChangesAsync();

            // Verify the desk exists before deletion
            Assert.True(await context.Desk.AnyAsync(d => d.DeskId == 1));
            
            // Create the controller instance with the test database context
            var controller = new DesksController(context);

            // ACT - Execute the method being tested
            var result = await controller.DeleteDesk(1);

            // ASSERT - Verify the results meet expectations
            
            // Verify that the result is of type NoContentResult
            var noContentResult = Assert.IsType<NoContentResult>(result);
            
            // Verify that the HTTP status code is 204 (No Content)
            Assert.Equal(204, noContentResult.StatusCode);
            
            // Verify that the desk was actually deleted from the database
            Assert.False(await context.Desk.AnyAsync(d => d.DeskId == 1));
        }

        /// <summary>
        /// Test the DeleteDesk method with non-existent DeskId using the 3A pattern (Arrange, Act, Assert)
        /// This test verifies that when a non-existent DeskId is provided, the API returns NotFound
        /// </summary>
        [Fact]
        public async Task DeleteDesk_WithNonExistentDeskId_ReturnsNotFound()
        {
            // ARRANGE - Set up the test environment, data, and dependencies
            
            // Create an in-memory database for testing to avoid affecting real database
            var options = new DbContextOptionsBuilder<HotdeskAPIContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB name for test isolation
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)) // Ignore transaction warnings for in-memory DB
                .Options;

            // Set up the database context with in-memory database (empty database)
            using var context = new HotdeskAPIContext(options);
            
            // Create the controller instance with the test database context
            var controller = new DesksController(context);

            // ACT - Execute the method being tested with non-existent DeskId
            var result = await controller.DeleteDesk(999); // Non-existent DeskId

            // ASSERT - Verify the results meet expectations
            
            // Verify that the result is of type NotFoundResult
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            
            // Verify that the HTTP status code is 404 (Not Found)
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
