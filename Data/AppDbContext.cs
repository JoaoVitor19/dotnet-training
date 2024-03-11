using dotnet_training.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace dotnet_training.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<ShortenerUrl> ShortenerUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply global query filter for ISoftDelete entities
            modelBuilder.ApplySoftDeleteQueryFilter();

            //Config shortenerUrl
            modelBuilder.Entity<ShortenerUrl>(builder =>
            {
                builder
                    .Property(shortenedUrl => shortenedUrl.Code)
                    .HasMaxLength(Helpers.Configs.ShortLinkSettings.CodeLength);
                builder
                    .HasIndex(shortenedUrl => shortenedUrl.Code)
                    .IsUnique();
            });
        }
    }

    //SoftDeleteExtension
    internal static class SoftDeleteModelBuilderExtensions
    {
        public static ModelBuilder ApplySoftDeleteQueryFilter(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (!typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    continue;
                }

                var param = Expression.Parameter(entityType.ClrType, "entity");
                var prop = Expression.PropertyOrField(param, nameof(ISoftDelete.IsDeleted));
                var entityNotDeleted = Expression.Lambda(Expression.Equal(prop, Expression.Constant(false)), param);

                entityType.SetQueryFilter(entityNotDeleted);
            }

            return modelBuilder;
        }
    }
}
