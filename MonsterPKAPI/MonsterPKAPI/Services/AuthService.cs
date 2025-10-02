using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MonsterPKAPI.Data;
using MonsterPKAPI.DTOs;
using MonsterPKAPI.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BCryptLib = BCrypt.Net.BCrypt;

namespace MonsterPKAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly MonsterPKDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(MonsterPKDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            try
            {
                // Check if username already exists
                var existingUsername = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username.ToLower() == request.Username.ToLower());

                if (existingUsername != null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Username already exists"
                    };
                }

                // Check if email already exists
                var existingEmail = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

                if (existingEmail != null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Email already exists"
                    };
                }

                // Generate unique user ID
                var userId = await GenerateUniqueUserIdAsync();

                // Generate salt and hash password
                var salt = GenerateSalt();
                var passwordHash = HashPasswordWithSalt(request.Password, salt);

                // Create new user
                var newUser = new User
                {
                    UserId = userId,
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    Salt = salt,
                    DisplayName = null, // User will set this later
                    IsActive = true,
                    IsBanned = false,
                    CreatedAt = DateTime.Now,
                    GameVersion = "1.0.0",
                    EmailVerified = false,
                    TwoFactorEnabled = false
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // Generate JWT token
                var token = GenerateJwtToken(newUser.UserId, newUser.Username, newUser.Email);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Registration successful",
                    Token = token,
                    User = new UserInfoDto
                    {
                        UserId = newUser.UserId,
                        Username = newUser.Username,
                        Email = newUser.Email,
                        DisplayName = newUser.DisplayName,
                        CreatedAt = newUser.CreatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration: {ErrorMessage}", ex.Message);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"An error occurred during registration: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            try
            {
                // Find user by username or email
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => 
                        u.Username.ToLower() == request.UsernameOrEmail.ToLower() || 
                        u.Email.ToLower() == request.UsernameOrEmail.ToLower());

                if (user == null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid username/email or password"
                    };
                }

                // Check if user is banned
                if (user.IsBanned)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = $"Account is banned. Reason: {user.BanReason ?? "No reason provided"}"
                    };
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Account is not active"
                    };
                }

                // Verify password
                if (string.IsNullOrEmpty(user.Salt) || !VerifyPassword(request.Password, user.PasswordHash, user.Salt))
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid username/email or password"
                    };
                }

                // Update last login
                user.LastLogin = DateTime.Now;
                user.LastSave = DateTime.Now;
                await _context.SaveChangesAsync();

                // Generate JWT token
                var token = GenerateJwtToken(user.UserId, user.Username, user.Email);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Login successful",
                    Token = token,
                    User = new UserInfoDto
                    {
                        UserId = user.UserId,
                        Username = user.Username,
                        Email = user.Email,
                        DisplayName = user.DisplayName,
                        CreatedAt = user.CreatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login: {ErrorMessage}", ex.Message);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"An error occurred during login: {ex.Message}"
                };
            }
        }

        public string GenerateJwtToken(string userId, string username, string email)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];

            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Key is not configured");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, email),
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddDays(7), // Token valid for 7 days
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateSalt()
        {
            // BCrypt generates and includes salt automatically
            return global::BCrypt.Net.BCrypt.GenerateSalt();
        }

        private string HashPasswordWithSalt(string password, string salt)
        {
            // BCrypt handles salt internally, just hash with the salt
            return global::BCrypt.Net.BCrypt.HashPassword(password, salt);
        }

        private bool VerifyPassword(string password, string passwordHash, string salt)
        {
            // BCrypt.Verify handles salt comparison automatically
            try
            {
                return global::BCrypt.Net.BCrypt.Verify(password, passwordHash);
            }
            catch
            {
                return false;
            }
        }

        private async Task<string> GenerateUniqueUserIdAsync()
        {
            string userId;
            bool exists;
            int counter = 1;

            do
            {
                userId = $"user_{counter:D6}"; // Format: user_000001, user_000002, etc.
                exists = await _context.Users.AnyAsync(u => u.UserId == userId);
                counter++;
            } while (exists);

            return userId;
        }
    }
}
