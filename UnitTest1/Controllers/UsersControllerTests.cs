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
    }
}
