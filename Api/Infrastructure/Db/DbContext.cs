using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;
namespace MinimalApi.Infrastructure.Db;
using BCrypt.Net;

public class DBContext : DbContext
{
    public DBContext(DbContextOptions<DBContext> options) : base(options) { }
    
    // Tables of the database
    public DbSet<Administrator> Administrators { get; set; } = default!;
    public DbSet<Vehicle> Vehicles { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed default data for Administrator table
        modelBuilder.Entity<Administrator>().HasData(
            new Administrator {
                Id = 1,
                Email = "admin@test.com",
                Password = BCrypt.HashPassword("123456"),
                Profile = "Admin"
            }
        );
    }
}