using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.ModelViews;
using MinimalApi.Domain.Services;
using MinimalApi.Infrastructure.Db;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DBContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home()));
#endregion

#region Administrators
app.MapPost("/administrators/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) => {
    if (administratorService.Login(loginDTO) != null)
    {
        return Results.Ok("Login realizado com sucesso!");
    }
    return Results.Unauthorized();
});
#endregion

#region Vehicles
app.MapPost("/vehicles", ([FromBody] VehicleDTO VehicleDTO, IVehicleService vehicleService) => {
    var vehicle = new Vehicle
    {
        Name = VehicleDTO.Name,
        Brand = VehicleDTO.Brand,
        Year = VehicleDTO.Year
    };
    var createdVehicle = vehicleService.Create(vehicle);
    return Results.Created($"/vehicle/{createdVehicle.Id}", createdVehicle);
});
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion