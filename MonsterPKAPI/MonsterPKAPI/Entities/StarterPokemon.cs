using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MonsterPKAPI.Entities
{
    [Table("StarterPokemon")]
    public class StarterPokemon
    {
        [Key]
        [Column("starter_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StarterId { get; set; }

        [Required]
        [Column("region_id")]
        [StringLength(20)]
        public string RegionId { get; set; } = string.Empty;

        [Required]
        [Column("mon_id")]
        [StringLength(20)]
        public string MonId { get; set; } = string.Empty;

        [Column("starter_level")]
        public int StarterLevel { get; set; } = 5;

        [Column("sort_order")]
        public int SortOrder { get; set; } = 0;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("RegionId")]
        public StarterRegion? Region { get; set; }
    }
}
