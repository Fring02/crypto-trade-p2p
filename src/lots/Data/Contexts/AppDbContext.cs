using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Lot> Lots { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseValidationCheckConstraints().UseEnumCheckConstraints();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lot>().Property(l => l.Id).UseIdentityAlwaysColumn();
        modelBuilder.Entity<Lot>().Property(l => l.CreatedAt).HasDefaultValueSql("now()");
        base.OnModelCreating(modelBuilder);
    }
}