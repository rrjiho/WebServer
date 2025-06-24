using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ServerAPI.Models;
using ServerAPI.Services;
using System.Security.Claims;

namespace ServerAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IDistributedCache _cache;
        private readonly ISessionService _sessionService;
        private readonly IGoogleAuthService _googleAuthService;

        public AuthController(IUserService userService, IDistributedCache cache, ISessionService sessionService, IGoogleAuthService googleAuthService)
        {
            _userService = userService;
            _cache = cache;
            _sessionService = sessionService;
            _googleAuthService = googleAuthService;
        }

        [HttpPost("test-login")]
        [AllowAnonymous]
        public async Task<IActionResult> TestLogin([FromBody] TestLoginDto dto)
        {
            // 운영 차단
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
                return Unauthorized("Test login not allowed in production");

            var user = await _userService.FindOrCreateUserByGoogleIdAsync(dto.GoogleId, dto.Nickname);

            var sessionId = await _sessionService.CreateSessionAsync(user.Id);

            Response.Cookies.Append("SessionId", sessionId, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok(new { userId = user.Id, user.Username });
        }

        [HttpPost("google-login")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            var payload = await _googleAuthService.VerifyIdTokenAsync(dto.IdToken);
            var user = await _userService.FindOrCreateUserAsync(payload);
            var sessionId = await _sessionService.CreateSessionAsync(user.Id);

            Response.Cookies.Append("SessionId", sessionId, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok(new { userId = user.Id, user.Username, user.Email });
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = HttpContext.Items["UserId"]?.ToString();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Log in is required.");
            }

            try
            {
                var user = await _userService.GetProfileAsync(userId);
                var resources = await _userService.GetResourcesAsync(userId);
                return Ok(new { user.Username, user.Level, user.Experience, user.Strength, 
                    user.Agility, user.Intelligence, resources.Gold, resources.Gems });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return StatusCode(500, e.Message);
            }
            
        }

        [HttpPost("addexp")]
        public async Task<IActionResult> AddExperience([FromBody] AddExpDto dto)
        {
            var userId = HttpContext.Items["UserId"]?.ToString();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Log in is required.");
            }

            try
            {
                await _userService.AddExperienceAsync(userId, dto.Experience);
                var user = await _userService.GetProfileAsync(userId);
                return Ok(new { LevelUp = user.Experience >= 100, user.Level, user.Experience });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return StatusCode(500, e.Message);
            }
            
        }

        [HttpGet("resources")]
        public async Task<IActionResult> GetResources()
        {
            var userId = HttpContext.Items["UserId"]?.ToString();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Log in is required.");
            }

            try
            {
                var resources = await _userService.GetResourcesAsync(userId);
                return Ok(new { resources.Gold, resources.Gems });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return NotFound(e.Message);
            }
        }

        [HttpPost("addresources")]
        public async Task<IActionResult> AddResources([FromBody] ResourcesDto dto)
        {
            var userId = HttpContext.Items["UserId"]?.ToString();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Log in is required.");
            }

            try
            {
                await _userService.AddResourcesAsync(userId, dto.Gold, dto.Gems);
                var resources = await _userService.GetResourcesAsync(userId);
                return Ok(new { resources.Gold, resources.Gems });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return NotFound(e.Message);
            }

        }

        [HttpPost("useresource")]
        public async Task<IActionResult> UseResources([FromBody] ResourcesDto dto)
        {
            var userId = HttpContext.Items["UserId"]?.ToString();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Log in is required.");
            }

            try
            {
                await _userService.UseResourcesAsync(userId, dto.Gold, dto.Gems);
                var resources = await _userService.GetResourcesAsync(userId);
                return Ok(new { resources.Gold, resources.Gems });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return BadRequest(e.Message);
            }
        }
    }
}
