using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MonsterPKAPI.Entities
{
    [Table("CharacterModel")]
    public class CharacterModel
    {
        [Key]
        [Column("model_id")]
        [StringLength(20)]
        public string ModelId { get; set; } = string.Empty;

        [Required]
        [Column("model_name")]
        [StringLength(50)]
        public string ModelName { get; set; } = string.Empty;

        [Required]
        [Column("gender")]
        [StringLength(10)]
        public string Gender { get; set; } = string.Empty; // "Male" or "Female"

        [Column("sprite_path")]
        [StringLength(255)]
        public string? SpritePath { get; set; }

        [Column("avatar_path")]
        [StringLength(255)]
        public string? AvatarPath { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("is_default")]
        public bool IsDefault { get; set; } = false;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
