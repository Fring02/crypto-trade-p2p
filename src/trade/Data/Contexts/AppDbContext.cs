using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
    public DbSet<Session> Sessions { get; set; } = null!;
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseEnumCheckConstraints();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Session>().OwnsOne(s => s.Details);
        base.OnModelCreating(modelBuilder);
    }
}