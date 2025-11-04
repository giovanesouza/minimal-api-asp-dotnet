using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;
namespace MinimalApi.Infrastructure.Db;
using BCrypt.Net;

public class DBContext : DbContext
{
    private readonly IConfiguration? _configurationAppSettings;

    // Constructor for real application (uses dependency injection)
    public DBContext(IConfiguration configurationAppSettings)
    {
        _configurationAppSettings = configurationAppSettings;
    }

    // Constructor for testing (receives DbContextOptions directly)
    public DBContext(DbContextOptions<DBContext> options) : base(options)
    {   
    }

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // If options have already been configured externally (e.g. in tests), skip configuration
        if (optionsBuilder.IsConfigured) return;

        // Configure the database connection using settings from appsettings.json
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _configurationAppSettings!.GetConnectionString("MySql")?.ToString();
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