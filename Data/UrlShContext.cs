using Microsoft.EntityFrameworkCore;
using UrlSh.Data.Models;

namespace UrlSh.Data
{
    public class UrlShContext : DbContext
    {
        public UrlShContext(DbContextOptions<UrlShContext> options) : base(options)
        {
        }

        public DbSet<Redirect> Redirects { get; set; } = null!;
        public DbSet<RedirectLog> RedirectsLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Redirect>()
                .ToTable("Redirects");

            modelBuilder.Entity<RedirectLog>()
                .ToTable("RedirectLogs");

            modelBuilder.Entity<Redirect>()
                .HasIndex(t => t.Code)
                .IsUnique();

            modelBuilder.Entity<Redirect>()
                .Property(t => t.CreatedAt)
                .HasDefaultValueSql("(NOW())");

            modelBuilder.Entity<RedirectLog>()
                .Property(t => t.CreatedAt)
                .HasDefaultValueSql("(NOW())");
        }
    }
}
