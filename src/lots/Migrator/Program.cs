using Data.Contexts;
using Microsoft.EntityFrameworkCore;
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("Database migration failed: connection string is not provided");
    Environment.Exit(1);
}
try
{
    var optsBuilder = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(connectionString);
    using var context = new AppDbContext(optsBuilder.Options);
    var migrations = context.Database.GetMigrations();
    foreach (var migration in migrations) Console.WriteLine($"Applying migration: {migration}...");
    context.Database.Migrate();
    context.SaveChanges();
    Console.WriteLine("Database migrations are applied successfully");
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
    Environment.Exit(1);
}