using BeConnectedAppsTesting.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeConnectedAppsTesting.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Project> Projects => Set<Project>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Key).IsRequired().HasMaxLength(20);
            entity.Property(p => p.Description).IsRequired();
            entity.HasIndex(p => p.Key).IsUnique();
        });
    }
}
