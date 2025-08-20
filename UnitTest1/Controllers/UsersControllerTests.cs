using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using HotdeskAPI.Controllers;
using HotdeskAPI.Data;
using HotdeskAPI; // For User model

namespace HotdeskAPI.UnitTest.Controllers
{
    public class UsersControllerTests
    {
        // Test method to verify that posting a user with an empty FullName returns BadRequest
        // This test checks the validation logic in the UsersController for the PostUser method
        // It ensures that the API correctly handles invalid input by returning a 400 Bad Request status code
        // and includes an appropriate error message in the response.

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

        /// <summary>
        /// Test phone number format validation in PostUser using the 3A pattern (Arrange, Act, Assert)
        /// This test verifies that when an invalid phone number format is provided, the API returns BadRequest
        /// This adds coverage for the RegularExpression validation pattern ^0\d{9}$ in the User model
        /// </summary>

    

    [Fact]
        public async Task PostUser_WithInvalidPhoneNumberFormat_ReturnsBadRequest()
        {
            // ARRANGE - Set up the test environment, data, and dependencies

            // Create an in-memory database for testing to avoid affecting real database
            var options = new DbContextOptionsBuilder<HotdeskAPIContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB name for test isolation
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)) // Ignore transaction warnings for in-memory DB
                .Options;

            // Create test user with invalid phone number format (this should trigger validation failure)
            var invalidUser = new User
            {
                FullName = "Mike Johnson", // Valid - proper FullName
                UserName = "mikejohnson",  // Valid - proper UserName
                PhoneNumber = "123456789", // Invalid - missing leading 0, doesn't match pattern ^0\d{9}$
                Email = "mike.johnson@example.com" // Valid - proper email format
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

            // Convert the error response to string and verify it contains the expected phone number validation message
            var errorMessage = errorResponse.ToString();
            Assert.Contains("PhoneNumber must be in the format 0123456789", errorMessage);
        }

        /// <summary>
        /// Test positive case: Valid phone number format validation in PostUser using the 3A pattern (Arrange, Act, Assert)
        /// This test verifies that when a valid phone number format is provided, the user is created successfully
        /// This tests the RegularExpression validation pattern ^0\d{9}$ accepts correct phone numbers
        /// </summary>
        [Fact]
        public async Task PostUser_WithValidPhoneNumberFormat_ReturnsCreatedAtAction()
        {
            // ARRANGE - Set up the test environment, data, and dependencies

            // Create an in-memory database for testing to avoid affecting real database
            var options = new DbContextOptionsBuilder<HotdeskAPIContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB name for test isolation
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)) // Ignore transaction warnings for in-memory DB
                .Options;

            // Create test user with valid phone number format (this should pass validation)
            var validUser = new User
            {
                FullName = "Sarah Wilson", // Valid - proper FullName
                UserName = "sarahwilson",  // Valid - proper UserName
                PhoneNumber = "0987654321", // Valid - matches pattern ^0\d{9}$ (starts with 0, followed by 9 digits)
                Email = "sarah.wilson@example.com" // Valid - proper email format
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

            // Verify that the returned user has the correct phone number
            Assert.Equal("0987654321", createdUser.PhoneNumber);

            // Verify that the user was assigned a valid UserId
            Assert.NotEqual(Guid.Empty, createdUser.UserId);

            // Verify that all other properties are correctly set
            Assert.Equal("Sarah Wilson", createdUser.FullName);
            Assert.Equal("sarahwilson", createdUser.UserName);
            Assert.Equal("sarah.wilson@example.com", createdUser.Email);
        }

        /// <summary>
        /// Test positive case: Valid email format validation in PostUser using the 3A pattern (Arrange, Act, Assert)
        /// This test verifies that when a valid email format is provided, the user is created successfully
        /// This tests the EmailAddress validation attribute accepts correct email formats
        /// </summary>
        [Fact]
        public async Task PostUser_WithValidEmailFormat_ReturnsCreatedAtAction()
        {
            // ARRANGE - Set up the test environment, data, and dependencies

            // Create an in-memory database for testing to avoid affecting real database
            var options = new DbContextOptionsBuilder<HotdeskAPIContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB name for test isolation
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)) // Ignore transaction warnings for in-memory DB
                .Options;

            // Create test user with valid email format (this should pass validation)
            var validUser = new User
            {
                FullName = "David Miller", // Valid - proper FullName
                UserName = "davidmiller",  // Valid - proper UserName
                PhoneNumber = "0123456789", // Valid - proper phone number format
                Email = "david.miller@company.com" // Valid - proper email format with subdomain
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

            // Verify that the returned user has the correct email
            Assert.Equal("david.miller@company.com", createdUser.Email);

            // Verify that the user was assigned a valid UserId
            Assert.NotEqual(Guid.Empty, createdUser.UserId);

            // Verify that all other properties are correctly set
            Assert.Equal("David Miller", createdUser.FullName);
            Assert.Equal("davidmiller", createdUser.UserName);
            Assert.Equal("0123456789", createdUser.PhoneNumber);
        }
    }
}
