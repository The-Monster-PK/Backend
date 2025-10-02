using System.ComponentModel.DataAnnotations;

namespace MonsterPKAPI.DTOs
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Username or Email is required")]
        [StringLength(100, ErrorMessage = "Username or Email cannot exceed 100 characters")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string Password { get; set; } = string.Empty;
    }
}
