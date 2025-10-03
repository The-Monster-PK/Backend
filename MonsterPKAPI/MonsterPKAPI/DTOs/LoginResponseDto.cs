namespace MonsterPKAPI.DTOs
{
    /// <summary>
    /// Simple login response with just the token
    /// </summary>
    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
    }
}
