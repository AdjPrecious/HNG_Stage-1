using HNG_Stage_1.Model;
using Microsoft.EntityFrameworkCore;

namespace HNG_Stage_1.Repository
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Profile> Profiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the Profile entity
            modelBuilder.Entity<Profile>().HasIndex(p => p.Name).IsUnique();
        } 
    }
}
