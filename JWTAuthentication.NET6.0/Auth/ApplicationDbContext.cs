using JWTAuthentication.NET6._0.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthentication.NET6._0.Auth
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<PosterEntity> Posters { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CategoryEntity>()
                .HasMany(c => c.ProductEntities)
                .WithOne(p => p.CategoryEntity)
                .HasForeignKey(p => p.CategoryId)
                .IsRequired();
            base.OnModelCreating(builder);
        }
    }
}
