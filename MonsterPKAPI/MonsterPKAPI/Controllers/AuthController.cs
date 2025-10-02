using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonsterPKAPI.DTOs;
using MonsterPKAPI.Services;

namespace MonsterPKAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user account
        /// </summary>
        /// <param name="request">Registration details</param>
        /// <returns>Registration response with success message</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<RegisterResponseDto>> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new RegisterResponseDto
                {
                    Success = false,
                    Message = "Invalid input data",
                });
            }

            var result = await _authService.RegisterAsync(request);

            if (!result.Success)
            {
                return BadRequest(new RegisterResponseDto
                {
                    Success = result.Success,
                    Message = result.Message
                });
            }

            return Ok(new RegisterResponseDto
            {
                Success = true,
                Message = "Registration successful. Please login to get your token."
            });
        }

        /// <summary>
        /// Login with username and password
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>Authentication response with JWT token</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new LoginResponseDto
                {
                    Success = false,
                    Message = "Invalid input data",
                });
            }

            var result = await _authService.LoginAsync(request);

            if (!result.Success)
            {
                return Unauthorized(new LoginResponseDto
                {
                    Success = result.Success,
                    Message = result.Message,
                    Token = null
                });
            }

            return Ok(new LoginResponseDto
            {
                Success = result.Success,
                Message = result.Message,
                Token = result.Token
            });
        }

        /// <summary>
        /// Test endpoint to verify JWT authentication
        /// </summary>
        /// <returns>User info from token</returns>
        [HttpGet("me")]
        [Authorize]
        public ActionResult<object> GetCurrentUser()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            return Ok(new
            {
                userId,
                username,
                email,
                message = "Authenticated successfully"
            });
        }
    }
}
