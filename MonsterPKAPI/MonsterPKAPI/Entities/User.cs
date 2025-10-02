using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MonsterPKAPI.Entities
{
    [Table("User")]
    public class User
    {
        [Key]
        [Column("user_id")]
        [StringLength(20)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Column("username")]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [Column("email")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("password_hash")]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("salt")]
        [StringLength(255)]
        public string? Salt { get; set; }

        [Column("display_name")]
        [StringLength(100)]
        public string? DisplayName { get; set; }

        [Column("current_location")]
        [StringLength(20)]
        public string? CurrentLocation { get; set; }

        [Column("play_time")]
        public int PlayTime { get; set; } = 0;

        [Column("last_ip_address")]
        [StringLength(45)]
        public string? LastIpAddress { get; set; }

        [Column("failed_login_attempts")]
        public int FailedLoginAttempts { get; set; } = 0;

        [Column("account_locked_until")]
        public DateTime? AccountLockedUntil { get; set; }

        [Column("email_verified")]
        public bool EmailVerified { get; set; } = false;

        [Column("two_factor_enabled")]
        public bool TwoFactorEnabled { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("last_login")]
        public DateTime? LastLogin { get; set; }

        [Column("last_save")]
        public DateTime? LastSave { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("is_banned")]
        public bool IsBanned { get; set; } = false;

        [Column("ban_reason")]
        [StringLength(500)]
        public string? BanReason { get; set; }

        [Column("ban_until")]
        public DateTime? BanUntil { get; set; }

        [Column("game_version")]
        [StringLength(10)]
        public string GameVersion { get; set; } = "1.0.0";
    }
}
