using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using HotdeskAPI.Controllers;
using HotdeskAPI.Data;
using Hotdesk.Models;
using Hotdesk.Components.Models;
using Xunit;

namespace HotdeskAPI.UnitTest.Controllers
{
    public class BookingControllerTests
    {
        /// <summary>
        /// Test the GetBookings method using the 3A pattern (Arrange, Act, Assert)
        /// This test verifies that all bookings are returned correctly
        /// </summary>
        [Fact]
        public async Task GetBookings_ReturnsAllBookings()
        {
            // ARRANGE
            var options = new DbContextOptionsBuilder<HotdeskAPIContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            using var context = new HotdeskAPIContext(options);

            // Seed test data: Desk, Booking
            var desk = new Desk { DeskId = 1, Name = "Desk 1", Location = "Floor 1", HasMonitor = true, IsAvailable = true };
            var booking = new Booking
            {
                BookingId = 1,
                DeskId = 1,
                UserName = "alice",
                BookingDate = new DateTime(2024, 6, 1),
                DurationType = "daily",
                CheckedIn = false
            };
            context.Desk.Add(desk);
            context.Booking.Add(booking);
            await context.SaveChangesAsync();

            var controller = new BookingsController(context);

            // ACT
            var result = await controller.GetBookings();

            // ASSERT
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Booking>>>(result);
            var bookings = Assert.IsAssignableFrom<IEnumerable<Booking>>(actionResult.Value);
            var bookingList = bookings.ToList();
            Assert.Single(bookingList);
            Assert.Equal(1, bookingList[0].BookingId);
            Assert.Equal("alice", bookingList[0].UserName);
            Assert.Equal(1, bookingList[0].DeskId);
            Assert.Equal("daily", bookingList[0].DurationType);
        }

        /// <summary>
        /// Test the PostBooking method with missing UserName using the 3A pattern (Arrange, Act, Assert)
        /// This test verifies that a booking with missing UserName returns BadRequest
        /// </summary>
        [Fact]
        public async Task PostBooking_WithMissingUserName_ReturnsBadRequest()
        {
            // ARRANGE
            var options = new DbContextOptionsBuilder<HotdeskAPIContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            using var context = new HotdeskAPIContext(options);
            var booking = new Booking
            {
                DeskId = 1,
                UserName = "",
                BookingDate = new DateTime(2024, 6, 1),
                DurationType = "Daily",
                CheckedIn = false
            };
            var controller = new BookingsController(context);

            // ACT
            var result = await controller.PostBooking(booking);

            // ASSERT
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequest.StatusCode);
            Assert.NotNull(badRequest.Value); // Ensure Value is not null
            Assert.Contains("UserName is required", badRequest.Value.ToString());
        }

        /// <summary>
        /// Test the GetBooking method returns a booking when it exists
        /// </summary>
        [Fact]
        public async Task GetBooking_ReturnsBooking_WhenExists()
        {
            // ARRANGE
            var options = new DbContextOptionsBuilder<HotdeskAPIContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            using var context = new HotdeskAPIContext(options);
            var desk = new Desk { DeskId = 2, Name = "Desk 2", Location = "Floor 2", HasMonitor = false, IsAvailable = true };
            var booking = new Booking
            {
                BookingId = 2,
                DeskId = 2,
                UserName = "bob",
                BookingDate = new DateTime(2024, 7, 1),
                DurationType = "weekly",
                CheckedIn = true
            };
            context.Desk.Add(desk);
            context.Booking.Add(booking);
            await context.SaveChangesAsync();
            var controller = new BookingsController(context);

            // ACT
            var result = await controller.GetBooking(2);

            // ASSERT
            var actionResult = Assert.IsType<ActionResult<Booking>>(result);
            var returnedBooking = Assert.IsType<Booking>(actionResult.Value);
            Assert.Equal(2, returnedBooking.BookingId);
            Assert.Equal("bob", returnedBooking.UserName);
        }

        /// <summary>
        /// Test the GetBooking method returns NotFound when booking does not exist
        /// </summary>
        [Fact]
        public async Task GetBooking_ReturnsNotFound_WhenNotExists()
        {
            // ARRANGE
            var options = new DbContextOptionsBuilder<HotdeskAPIContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            using var context = new HotdeskAPIContext(options);
            var controller = new BookingsController(context);

            // ACT
            var result = await controller.GetBooking(999);

            // ASSERT
            var actionResult = Assert.IsType<ActionResult<Booking>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        /// <summary>
        /// Test the PostBooking method with valid booking returns CreatedAtAction
        /// </summary>
        [Fact]
        public async Task PostBooking_WithValidBooking_ReturnsCreatedAtAction()
        {
            // ARRANGE
            var options = new DbContextOptionsBuilder<HotdeskAPIContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            using var context = new HotdeskAPIContext(options);
            var desk = new Desk { DeskId = 3, Name = "Desk 3", Location = "Floor 3", HasMonitor = false, IsAvailable = true };
            context.Desk.Add(desk);
            await context.SaveChangesAsync();
            var booking = new Booking
            {
                DeskId = 3,
                UserName = "carol",
                BookingDate = new DateTime(2024, 8, 1),
                DurationType = "monthly",
                CheckedIn = false
            };
            var controller = new BookingsController(context);

            // ACT
            var result = await controller.PostBooking(booking);

            // ASSERT
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdBooking = Assert.IsType<Booking>(createdResult.Value);
            Assert.Equal("carol", createdBooking.UserName);
            Assert.Equal(3, createdBooking.DeskId);
        }

        /// <summary>
        /// Test the PostBooking method with missing DeskId returns BadRequest
        /// </summary>
        [Fact]
        public async Task PostBooking_WithMissingDeskId_ReturnsBadRequest()
        {
            // ARRANGE
            var options = new DbContextOptionsBuilder<HotdeskAPIContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            using var context = new HotdeskAPIContext(options);
            var booking = new Booking
            {
                DeskId = 0,
                UserName = "dave",
                BookingDate = new DateTime(2024, 9, 1),
                DurationType = "daily",
                CheckedIn = false
            };
            var controller = new BookingsController(context);

            // ACT
            var result = await controller.PostBooking(booking);

            // ASSERT
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequest.StatusCode);
            Assert.NotNull(badRequest.Value);
            Assert.Contains("DeskId is required", badRequest.Value.ToString());
        }

        /// <summary>
        /// Test the PostBooking method with missing BookingDate returns BadRequest
        /// </summary>
        [Fact]
        public async Task PostBooking_WithMissingBookingDate_ReturnsBadRequest()
        {
            // ARRANGE
            var options = new DbContextOptionsBuilder<HotdeskAPIContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            using var context = new HotdeskAPIContext(options);
            var booking = new Booking
            {
                DeskId = 1,
                UserName = "eve",
                BookingDate = default,
                DurationType = "daily",
                CheckedIn = false
            };
            var controller = new BookingsController(context);

            // ACT
            var result = await controller.PostBooking(booking);

            // ASSERT
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequest.StatusCode);
            Assert.NotNull(badRequest.Value);
            Assert.Contains("BookingDate is required", badRequest.Value.ToString());
        }

        /// <summary>
        /// Test the DeleteBooking method with valid id removes booking and returns NoContent
        /// </summary>
        [Fact]
        public async Task DeleteBooking_WithValidId_RemovesBookingAndReturnsNoContent()
        {
            // ARRANGE
            var options = new DbContextOptionsBuilder<HotdeskAPIContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            using var context = new HotdeskAPIContext(options);
            var desk = new Desk { DeskId = 4, Name = "Desk 4", Location = "Floor 4", HasMonitor = false, IsAvailable = true };
            var booking = new Booking
            {
                BookingId = 4,
                DeskId = 4,
                UserName = "frank",
                BookingDate = new DateTime(2024, 10, 1),
                DurationType = "daily",
                CheckedIn = false
            };
            context.Desk.Add(desk);
            context.Booking.Add(booking);
            await context.SaveChangesAsync();
            var controller = new BookingsController(context);

            // ACT
            var result = await controller.DeleteBooking(4);

            // ASSERT
            var noContent = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContent.StatusCode);
            Assert.False(context.Booking.Any(b => b.BookingId == 4));
        }

        /// <summary>
        /// Test the DeleteBooking method with invalid id returns NotFound
        /// </summary>
        [Fact]
        public async Task DeleteBooking_WithInvalidId_ReturnsNotFound()
        {
            // ARRANGE
            var options = new DbContextOptionsBuilder<HotdeskAPIContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            using var context = new HotdeskAPIContext(options);
            var controller = new BookingsController(context);

            // ACT
            var result = await controller.DeleteBooking(999);

            // ASSERT
            var notFound = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFound.StatusCode);
        }
    }
}
