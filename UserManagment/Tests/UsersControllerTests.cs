using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Controllers;
using UserManagement.Models;
using UserManagement.Services.Interfaces;
using Xunit;

namespace UserManagement.Tests
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IStringLocalizer<UsersController>> _localizerMock;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _localizerMock = new Mock<IStringLocalizer<UsersController>>();
            _localizerMock.Setup(l => l["UserNotFound"])
                .Returns(new LocalizedString("UserNotFound", "User not found"));
            _localizerMock.Setup(l => l["UsernameOrEmailExists"])
                .Returns(new LocalizedString("UsernameOrEmailExists", "Username or email already exists"));
            _localizerMock.Setup(l => l["UserIdMismatch"])
                .Returns(new LocalizedString("UserIdMismatch", "User ID mismatch"));
            _controller = new UsersController(_userServiceMock.Object, _localizerMock.Object);
        }

        [Fact]
        public async Task GetUsers_ReturnsOkWithAllUsers()
        {
            // Arrange
            var users = new List<User> { new User { Id = 1, Username = "user1" } };
            _userServiceMock.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _controller.GetUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(users, okResult.Value);
        }

        [Fact]
        public async Task GetUser_ExistingId_ReturnsOkWithUser()
        {
            // Arrange
            var user = new User { Id = 1, Username = "user1" };
            _userServiceMock.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(user);

            // Act
            var result = await _controller.GetUser(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(user, okResult.Value);
        }

        [Fact]
        public async Task GetUser_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _userServiceMock.Setup(s => s.GetUserByIdAsync(999)).ReturnsAsync((User)null);

            // Act
            var result = await _controller.GetUser(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var returnValue = notFoundResult.Value;
            Assert.Equal("User not found", returnValue.GetType().GetProperty("Message").GetValue(returnValue));
        }

        [Fact]
        public async Task CreateUser_ValidUser_ReturnsCreatedAtAction()
        {
            // Arrange
            var user = new User { Id = 1, Username = "user1" };
            _userServiceMock.Setup(s => s.CreateUserAsync(user)).ReturnsAsync(user);

            // Act
            var result = await _controller.CreateUser(user);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetUser", createdResult.ActionName);
            Assert.Equal(1, createdResult.RouteValues["id"]);
            Assert.Equal(user, createdResult.Value);
        }

        [Fact]
        public async Task CreateUser_DuplicateUsernameOrEmail_ReturnsBadRequest()
        {
            // Arrange
            var user = new User { Id = 1, Username = "user1" };
            _userServiceMock.Setup(s => s.CreateUserAsync(user))
                .ThrowsAsync(new DbUpdateException());

            // Act
            var result = await _controller.CreateUser(user);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var returnValue = badRequestResult.Value;
            Assert.Equal("Username or email already exists", returnValue.GetType().GetProperty("Message").GetValue(returnValue));
        }

        [Fact]
        public async Task UpdateUser_ValidUser_ReturnsNoContent()
        {
            // Arrange
            var user = new User { Id = 1, Username = "user1" };
            _userServiceMock.Setup(s => s.UpdateUserAsync(user)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateUser(1, user);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateUser_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var user = new User { Id = 2, Username = "user1" };

            // Act
            var result = await _controller.UpdateUser(1, user);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnValue = badRequestResult.Value;
            Assert.Equal("User ID mismatch", returnValue.GetType().GetProperty("Message").GetValue(returnValue));
        }

        [Fact]
        public async Task UpdateUser_NonExistingUser_ReturnsNotFound()
        {
            // Arrange
            var user = new User { Id = 1, Username = "user1" };
            _userServiceMock.Setup(s => s.UpdateUserAsync(user))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.UpdateUser(1, user);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnValue = notFoundResult.Value;
            Assert.Equal("User not found", returnValue.GetType().GetProperty("Message").GetValue(returnValue));
        }

        [Fact]
        public async Task DeleteUser_ExistingId_ReturnsNoContent()
        {
            // Arrange
            _userServiceMock.Setup(s => s.DeleteUserAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteUser(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteUser_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _userServiceMock.Setup(s => s.DeleteUserAsync(999))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.DeleteUser(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnValue = notFoundResult.Value;
            Assert.Equal("User not found", returnValue.GetType().GetProperty("Message").GetValue(returnValue));
        }
    }
}