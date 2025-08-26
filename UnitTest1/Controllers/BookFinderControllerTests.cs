using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using HotdeskAPI.Controllers;
using HotdeskAPI.Data;
using Hotdesk.Components.Models;
using Hotdesk.Models;
using Xunit;

namespace HotdeskAPI.UnitTest.Controllers
{
    public class BookFinderControllerTests
    {
        /// <summary>
        /// Test the GetBookFinders method using the 3A pattern (Arrange, Act, Assert)
        /// This test verifies that all booking data is returned correctly
        /// </summary>
        [Fact]
        public async Task GetBookFinders_ReturnsAllBookFinders()
        {
            // ARRANGE - Set up the test environment, data, and dependencies
            var options = new DbContextOptionsBuilder<HotdeskAPIContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            using var context = new HotdeskAPIContext(options);

            // Seed test data: User, Desk, Booking
            var user = new User
            {
                UserId = Guid.NewGuid(),
                FullName = "Alice Smith",
                UserName = "alice",
                PhoneNumber = "0123456789",
                Email = "alice@example.com"
            };
            var desk = new Desk
            {
                DeskId = 1,
                Name = "Desk 1",
                Location = "Floor 1",
                HasMonitor = true,
                IsAvailable = true
            };
            var booking = new Booking
            {
                BookingId = 1,
                DeskId = 1,
                UserName = "alice",
                BookingDate = new DateTime(2024, 6, 1),
                DurationType = "Full Day",
                CheckedIn = false
            };
            context.User.Add(user);
            context.Desk.Add(desk);
            context.Booking.Add(booking);
            await context.SaveChangesAsync();

            var controller = new BookFindersController(context);

            // ACT - Execute the method being tested
            var result = await controller.GetBookFinders();

            // ASSERT - Verify the results meet expectations
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var bookFinders = Assert.IsAssignableFrom<IEnumerable<BookFinder>>(okResult.Value);
            var bookFinderList = bookFinders.ToList();
            Assert.Single(bookFinderList);
            Assert.Equal(1, bookFinderList[0].BookingId);
            Assert.Equal("alice", bookFinderList[0].UserName);
            Assert.Equal("Desk 1", bookFinderList[0].DeskName);
            Assert.Equal("Floor 1", bookFinderList[0].Location);
            Assert.Equal("0123456789", bookFinderList[0].PhoneNumber);
            Assert.Equal("Full Day", bookFinderList[0].DurationType);
        }
    }
}
