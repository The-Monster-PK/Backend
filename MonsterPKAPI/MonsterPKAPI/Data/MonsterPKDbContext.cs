using Microsoft.EntityFrameworkCore;
using MonsterPKAPI.Entities;

namespace MonsterPKAPI.Data
{
    public class MonsterPKDbContext : DbContext
    {
        public MonsterPKDbContext(DbContextOptions<MonsterPKDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<CharacterModel> CharacterModels { get; set; }
        public DbSet<StarterRegion> StarterRegions { get; set; }
        public DbSet<StarterPokemon> StarterPokemons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                
                entity.HasIndex(e => e.Username)
                    .IsUnique()
                    .HasDatabaseName("IX_User_Username");
                
                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_User_Email");
                
                entity.HasIndex(e => e.CurrentLocation)
                    .HasDatabaseName("IX_User_Location");
                
                entity.HasIndex(e => e.LastLogin)
                    .HasDatabaseName("IX_User_LastLogin");
                
                entity.HasIndex(e => e.IsActive)
                    .HasDatabaseName("IX_User_Active");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
