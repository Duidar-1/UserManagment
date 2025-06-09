using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using UserManagement.DTOs;
using UserManagement.Services.Interfaces;

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IStringLocalizer<AuthController> _localizer;

        public AuthController(IAuthService authService, IStringLocalizer<AuthController> localizer)
        {
            _authService = authService;
            _localizer = localizer;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            try
            {
                var (accessToken, refreshToken) = await _authService.LoginAsync(dto.Username, dto.Password);
                return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
            }
            catch (UnauthorizedAccessException)
            {
              
                return Unauthorized(new { Message = _localizer["InvalidCredentials"].Value });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO dto)
        {
            try
            {
                var newAccessToken = await _authService.RefreshTokenAsync(dto.RefreshToken);
                return Ok(new { AccessToken = newAccessToken });
            }
            catch (SecurityTokenException)
            {
                return BadRequest(new { Message = _localizer["InvalidRefreshToken"].Value });
            }
        }
    }

}
