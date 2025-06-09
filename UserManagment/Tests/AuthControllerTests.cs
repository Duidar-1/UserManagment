using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.Globalization;
using UserManagement.Controllers;
using UserManagement.DTOs;
using UserManagement.Services.Interfaces;
using Xunit;

namespace UserManagement.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IStringLocalizer<AuthController>> _localizerMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _localizerMock = new Mock<IStringLocalizer<AuthController>>();
            _controller = new AuthController(_authServiceMock.Object, _localizerMock.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkWithTokens()
        {
            // Arrange
            var dto = new LoginDTO { Username = "testuser", Password = "Test@123" };
            _authServiceMock.Setup(s => s.LoginAsync(dto.Username, dto.Password))
                .ReturnsAsync(("accessToken", "refreshToken"));

            // Act
            var result = await _controller.Login(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = okResult.Value;
            Assert.Equal("accessToken", returnValue.GetType().GetProperty("AccessToken").GetValue(returnValue));
            Assert.Equal("refreshToken", returnValue.GetType().GetProperty("RefreshToken").GetValue(returnValue));
        }

        [Fact]
        public async Task Login_InvalidCredentials_EnUs_ReturnsUnauthorized()
        {
            // Arrange
            var dto = new LoginDTO { Username = "testuser", Password = "WrongPass" };
            _authServiceMock.Setup(s => s.LoginAsync(dto.Username, dto.Password))
                .ThrowsAsync(new UnauthorizedAccessException());
            _localizerMock.Setup(l => l["InvalidCredentials"])
                .Returns(new LocalizedString("InvalidCredentials", "Invalid credentials"));

            // Act
            var result = await _controller.Login(dto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var returnValue = unauthorizedResult.Value;
            Assert.Equal("Invalid credentials", returnValue.GetType().GetProperty("Message").GetValue(returnValue));
        }

        [Fact]
        public async Task Login_InvalidCredentials_HiIn_ReturnsUnauthorizedWithLocalizedMessage()
        {
            // Arrange
            var originalCulture = CultureInfo.CurrentUICulture;
            CultureInfo.CurrentUICulture = new CultureInfo("hi-IN");

            var dto = new LoginDTO { Username = "testuser", Password = "WrongPass" };
            _authServiceMock.Setup(s => s.LoginAsync(dto.Username, dto.Password))
                .ThrowsAsync(new UnauthorizedAccessException());
            _localizerMock.Setup(l => l["InvalidCredentials"])
                .Returns(new LocalizedString("InvalidCredentials", "अमान्य क्रेडेंशियल्स"));

            // Act
            var result = await _controller.Login(dto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var returnValue = unauthorizedResult.Value;
            Assert.Equal("अमान्य क्रेडेंशियल्स", returnValue.GetType().GetProperty("Message").GetValue(returnValue));

            // Cleanup
            CultureInfo.CurrentUICulture = originalCulture;
        }

        [Fact]
        public async Task RefreshToken_ValidToken_ReturnsOkWithNewAccessToken()
        {
            // Arrange
            var dto = new RefreshTokenDTO { RefreshToken = "validtoken" };
            _authServiceMock.Setup(s => s.RefreshTokenAsync(dto.RefreshToken))
                .ReturnsAsync("newAccessToken");

            // Act
            var result = await _controller.RefreshToken(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = okResult.Value;
            Assert.Equal("newAccessToken", returnValue.GetType().GetProperty("AccessToken").GetValue(returnValue));
        }

        [Fact]
        public async Task RefreshToken_InvalidToken_HiIn_ReturnsBadRequestWithLocalizedMessage()
        {
            // Arrange
            var originalCulture = CultureInfo.CurrentUICulture;
            CultureInfo.CurrentUICulture = new CultureInfo("hi-IN");

            var dto = new RefreshTokenDTO { RefreshToken = "invalidtoken" };
            _authServiceMock.Setup(s => s.RefreshTokenAsync(dto.RefreshToken))
                .ThrowsAsync(new SecurityTokenException());
            _localizerMock.Setup(l => l["InvalidRefreshToken"])
                .Returns(new LocalizedString("InvalidRefreshToken", "अमान्य या समाप्त रिफ्रेश टोकन"));

            // Act
            var result = await _controller.RefreshToken(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnValue = badRequestResult.Value;
            Assert.Equal("अमान्य या समाप्त रिफ्रेश टोकन", returnValue.GetType().GetProperty("Message").GetValue(returnValue));

            // Cleanup
            CultureInfo.CurrentUICulture = originalCulture;
        }
    }
}