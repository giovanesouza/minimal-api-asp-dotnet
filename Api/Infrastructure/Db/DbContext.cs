using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;
namespace MinimalApi.Infrastructure.Db;
using BCrypt.Net;

public class DBContext : DbContext
{
    private readonly IConfiguration _configurationAppSettings;
    public DBContext(IConfiguration configurationAppSettings)
    {
        _configurationAppSettings = configurationAppSettings;
    }

    public DbSet<Administrator> Administrators { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrator>().HasData(
            new Administrator {
                Id = 1,
                Email = "admin@test.com",
                Password = BCrypt.HashPassword("123456"),
                Profile = "Admin"
            }
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _configurationAppSettings.GetConnectionString("mysql")?.ToString();
            if (connectionString != null)
            {
                optionsBuilder.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString)
                );
            }
        }
    }
}