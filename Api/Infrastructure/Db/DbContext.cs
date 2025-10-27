using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;
namespace MinimalApi.Infrastructure.Db;

public class DBContext : DbContext
{
    private readonly IConfiguration _configurationAppSettings;
    public DBContext(IConfiguration configurationAppSettings)
    {
        _configurationAppSettings = configurationAppSettings;
    }

    public DbSet<Administrator> Administrators { get; set; } = default!;
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
                return;
            }
        }
    }
}