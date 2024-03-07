using dotnet_training.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_training.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply global query filter for ISoftDelete entities
            modelBuilder.Entity<User>().HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
