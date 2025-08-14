using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using HotdeskAPI.Controllers;
using HotdeskAPI.Data;
using HotdeskAPI; // For User model

namespace UnitTest1
{
    public class UserTest2
    {
        /// <summary>
        /// Test the FullName validation in PostUser using the 3A pattern (Arrange, Act, Assert)
        /// This test verifies that when FullName is empty, the API returns BadRequest
        /// </summary>
        [Fact]
        public async Task PostUser_WithEmptyFullName_ReturnsBadRequest()
        {
            // ARRANGE - Set up the test environment, data, and dependencies
            
            // Create an in-memory database for testing to avoid affecting real database
            var options = new DbContextOptionsBuilder<HotdeskAPIContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB name for test isolation
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)) // Ignore transaction warnings for in-memory DB
                .Options;

            // Create test user with empty FullName (this should trigger validation failure)
            var invalidUser = new User
            {
                FullName = "", // Invalid - empty FullName should cause validation error
                UserName = "johndoe",
                PhoneNumber = "0987654321",
                Email = "john.doe@example.com"
            };

            // Set up the database context with in-memory database
            using var context = new HotdeskAPIContext(options);
            
            // Create the controller instance with the test database context
            var controller = new UsersController(context);

            // ACT - Execute the method being tested
            var result = await controller.PostUser(invalidUser);

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
            Assert.Contains("FullName is required", errorMessage);
        }

        /// <summary>
        /// Test a positive case: Valid user with proper FullName should be created successfully
        /// This test verifies that when FullName is provided, the user creation works
        /// </summary>
        [Fact]
        public async Task PostUser_WithValidFullName_ReturnsCreatedAtAction()
        {
            // ARRANGE - Set up the test environment, data, and dependencies
            
            // Create an in-memory database for testing
            var options = new DbContextOptionsBuilder<HotdeskAPIContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB name for test isolation
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)) // Ignore transaction warnings for in-memory DB
                .Options;

            // Create test user with valid FullName (this should pass validation)
            var validUser = new User
            {
                FullName = "John Doe", // Valid - non-empty FullName should pass validation
                UserName = "johndoe123",
                PhoneNumber = "0123456789",
                Email = "john.doe123@example.com"
            };

            // Set up the database context with in-memory database
            using var context = new HotdeskAPIContext(options);
            
            // Create the controller instance with the test database context
            var controller = new UsersController(context);

            // ACT - Execute the method being tested
            var result = await controller.PostUser(validUser);

            // ASSERT - Verify the results meet expectations
            
            // Verify that the result is of type CreatedAtActionResult (successful creation)
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            
            // Verify that the HTTP status code is 201 (Created)
            Assert.Equal(201, createdResult.StatusCode);
            
            // Verify that the created user is returned
            var createdUser = Assert.IsType<User>(createdResult.Value);
            
            // Verify that the returned user has the correct FullName
            Assert.Equal("John Doe", createdUser.FullName);
            
            // Verify that the user was assigned a valid UserId
            Assert.NotEqual(Guid.Empty, createdUser.UserId);
        }

        /// <summary>
        /// Test email format validation in PostUser using the 3A pattern (Arrange, Act, Assert)
        /// This test verifies that when an invalid email format is provided, the API returns BadRequest
        /// This adds coverage for the EmailAddress validation in the User model and controller
        /// </summary>
        [Fact]
        public async Task PostUser_WithInvalidEmailFormat_ReturnsBadRequest()
        {
            // ARRANGE - Set up the test environment, data, and dependencies
            
            // Create an in-memory database for testing to avoid affecting real database
            var options = new DbContextOptionsBuilder<HotdeskAPIContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB name for test isolation
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)) // Ignore transaction warnings for in-memory DB
                .Options;

            // Create test user with invalid email format (this should trigger validation failure)
            var invalidUser = new User
            {
                FullName = "Jane Smith", // Valid - proper FullName
                UserName = "janesmith",  // Valid - proper UserName
                PhoneNumber = "0987654321", // Valid - proper phone number format
                Email = "invalid-email-format" // Invalid - missing @ symbol and domain, should cause validation error
            };

            // Set up the database context with in-memory database
            using var context = new HotdeskAPIContext(options);
            
            // Create the controller instance with the test database context
            var controller = new UsersController(context);

            // ACT - Execute the method being tested
            var result = await controller.PostUser(invalidUser);

            // ASSERT - Verify the results meet expectations
            
            // Verify that the result is of type BadRequestObjectResult
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            
            // Verify that the HTTP status code is 400 (Bad Request)
            Assert.Equal(400, badRequestResult.StatusCode);
            
            // Verify that the error response is not null
            var errorResponse = badRequestResult.Value;
            Assert.NotNull(errorResponse);
            
            // Convert the error response to string and verify it contains the expected email validation message
            var errorMessage = errorResponse.ToString();
            Assert.Contains("Email is not valid", errorMessage);
        }
    }
}
