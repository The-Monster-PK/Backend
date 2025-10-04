using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MonsterPKAPI.Entities
{
    [Table("StarterRegion")]
    public class StarterRegion
    {
        [Key]
        [Column("region_id")]
        [StringLength(20)]
        public string RegionId { get; set; } = string.Empty;

        [Required]
        [Column("region_name")]
        [StringLength(50)]
        public string RegionName { get; set; } = string.Empty;

        [Required]
        [Column("starting_town")]
        [StringLength(50)]
        public string StartingTown { get; set; } = string.Empty;

        [Required]
        [Column("starting_location_id")]
        [StringLength(20)]
        public string StartingLocationId { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("background_image")]
        [StringLength(255)]
        public string? BackgroundImage { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("sort_order")]
        public int SortOrder { get; set; } = 0;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
