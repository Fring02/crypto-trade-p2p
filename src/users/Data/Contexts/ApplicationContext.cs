using System.Security.Cryptography;
using System.Text;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) {}
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RequisiteDetails> RequisiteDetails { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var admin = new User { Id = Guid.NewGuid(), Email = "admin@gmail.com", Firstname = "admin", Lastname = "admin", Role = "admin" };
        var adminRequisite = new RequisiteDetails { PhoneNumber = "+77761667060", CreditCardNumber = "1234567890123456",
            BankName = "Kaspi", UserId = admin.Id, Id = 1 };
        using var hmac = new HMACSHA256();
        admin.Salt = hmac.Key;
        admin.Hash = hmac.ComputeHash(Encoding.UTF8.GetBytes("admin"));
        modelBuilder.Entity<User>().HasData(admin);
        modelBuilder.Entity<User>().HasMany(u => u.Requisites).WithOne(r => r.User);
        modelBuilder.Entity<RequisiteDetails>().Property(u => u.CreditCardNumber).IsFixedLength().HasMaxLength(16);
        modelBuilder.Entity<RequisiteDetails>().Property(u => u.PhoneNumber).IsFixedLength().HasMaxLength(12);
        modelBuilder.Entity<RequisiteDetails>().HasData(adminRequisite);
        base.OnModelCreating(modelBuilder);
    }

    public async Task<bool> SaveAsync(CancellationToken token = default) => await SaveChangesAsync(token) > 0;
}