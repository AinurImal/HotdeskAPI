using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotdeskAPI.Controllers;
using HotdeskAPI.Data;
using HotdeskAPI; // For User model

namespace UnitTest1
{
    public class UnitTest1
    {
        /// <summary>
        /// Test the EASIEST validation: FullName is required for User creation
        /// This is the simplest validation test using the 3A pattern (Arrange, Act, Assert)
        /// </summary>
        [Fact]
        public async Task PostUser_WithEmptyFullName_ReturnsBadRequest()
        {
            // ARRANGE - Set up the test environment and data

            // Create an in-memory database for testing
            var options = new DbContextOptionsBuilder<HotdeskAPIContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Create test user with empty FullName (this should fail validation)
            var invalidUser = new User
            {
                FullName = "", // Invalid - empty FullName should trigger validation error
                UserName = "testuser",
                PhoneNumber = "0123456789",
                Email = "test@example.com"
            };

            // Set up the database context
            using var context = new HotdeskAPIContext(options);

            // Create the controller instance with the test context
            var controller = new UsersController(context);

            // ACT - Execute the method being tested
            var result = await controller.PostUser(invalidUser);

            // ASSERT - Verify the results meet expectations

            // Verify the result type is BadRequestObjectResult
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);

            // Verify the status code is 400 (Bad Request)
            Assert.Equal(400, badRequestResult.StatusCode);

            // Verify the error message contains the expected validation message
            var errorResponse = badRequestResult.Value;
            Assert.NotNull(errorResponse);

            // Convert the anonymous object to a string to check the message
            var errorMessage = errorResponse.ToString();
            Assert.Contains("FullName is required", errorMessage);
        }
    }
}