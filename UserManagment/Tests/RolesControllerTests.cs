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
    public class RolesControllerTests
    {
        private readonly Mock<IRoleService> _roleServiceMock;
        private readonly Mock<IStringLocalizer<RolesController>> _localizerMock;
        private readonly RolesController _controller;

        public RolesControllerTests()
        {
            _roleServiceMock = new Mock<IRoleService>();
            _localizerMock = new Mock<IStringLocalizer<RolesController>>();
            _localizerMock.Setup(l => l["RoleNotFound"])
                .Returns(new LocalizedString("RoleNotFound", "Role not found"));
            _localizerMock.Setup(l => l["RoleNameExists"])
                .Returns(new LocalizedString("RoleNameExists", "Role name already exists"));
            _localizerMock.Setup(l => l["RoleIdMismatch"])
                .Returns(new LocalizedString("RoleIdMismatch", "Role ID mismatch"));
            _controller = new RolesController(_roleServiceMock.Object, _localizerMock.Object);
        }

        [Fact]
        public async Task GetRoles_ReturnsOkWithAllRoles()
        {
            // Arrange
            var roles = new List<Role> { new Role { Id = 1, Name = "Admin" } };
            _roleServiceMock.Setup(s => s.GetAllRolesAsync()).ReturnsAsync(roles);

            // Act
            var result = await _controller.GetRoles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(roles, okResult.Value);
        }

        [Fact]
        public async Task GetRole_ExistingId_ReturnsOkWithRole()
        {
            // Arrange
            var role = new Role { Id = 1, Name = "Admin" };
            _roleServiceMock.Setup(s => s.GetRoleByIdAsync(1)).ReturnsAsync(role);

            // Act
            var result = await _controller.GetRole(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(role, okResult.Value);
        }

        [Fact]
        public async Task GetRole_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _roleServiceMock.Setup(s => s.GetRoleByIdAsync(999)).ReturnsAsync((Role)null);

            // Act
            var result = await _controller.GetRole(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var returnValue = notFoundResult.Value;
            Assert.Equal("Role not found", returnValue.GetType().GetProperty("Message").GetValue(returnValue));
        }

        [Fact]
        public async Task CreateRole_ValidRole_ReturnsCreatedAtAction()
        {
            // Arrange
            var role = new Role { Id = 1, Name = "Admin" };
            _roleServiceMock.Setup(s => s.CreateRoleAsync(role)).ReturnsAsync(role);

            // Act
            var result = await _controller.CreateRole(role);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetRole", createdResult.ActionName);
            Assert.Equal(1, createdResult.RouteValues["id"]);
            Assert.Equal(role, createdResult.Value);
        }

        [Fact]
        public async Task CreateRole_DuplicateName_ReturnsBadRequest()
        {
            // Arrange
            var role = new Role { Id = 1, Name = "Admin" };
            _roleServiceMock.Setup(s => s.CreateRoleAsync(role))
                .ThrowsAsync(new DbUpdateException());

            // Act
            var result = await _controller.CreateRole(role);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var returnValue = badRequestResult.Value;
            Assert.Equal("Role name already exists", returnValue.GetType().GetProperty("Message").GetValue(returnValue));
        }

        [Fact]
        public async Task UpdateRole_ValidRole_ReturnsNoContent()
        {
            // Arrange
            var role = new Role { Id = 1, Name = "Admin" };
            _roleServiceMock.Setup(s => s.UpdateRoleAsync(role)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateRole(1, role);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateRole_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var role = new Role { Id = 2, Name = "Admin" };

            // Act
            var result = await _controller.UpdateRole(1, role);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnValue = badRequestResult.Value;
            Assert.Equal("Role ID mismatch", returnValue.GetType().GetProperty("Message").GetValue(returnValue));
        }

        [Fact]
        public async Task UpdateRole_NonExistingRole_ReturnsNotFound()
        {
            // Arrange
            var role = new Role { Id = 1, Name = "Admin" };
            _roleServiceMock.Setup(s => s.UpdateRoleAsync(role))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.UpdateRole(1, role);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnValue = notFoundResult.Value;
            Assert.Equal("Role not found", returnValue.GetType().GetProperty("Message").GetValue(returnValue));
        }

        [Fact]
        public async Task DeleteRole_ExistingId_ReturnsNoContent()
        {
            // Arrange
            _roleServiceMock.Setup(s => s.DeleteRoleAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteRole(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteRole_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _roleServiceMock.Setup(s => s.DeleteRoleAsync(999))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.DeleteRole(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnValue = notFoundResult.Value;
            Assert.Equal("Role not found", returnValue.GetType().GetProperty("Message").GetValue(returnValue));
        }
    }
}