using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Enuns;
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
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administrators
static ValidationErrors ValidateAdministratorDTO(AdministratorDTO administratorDTO)
{
    var validation = new ValidationErrors
    {
        Messages = []
    };

    if (string.IsNullOrEmpty(administratorDTO.Email)) 
        validation.Messages.Add("The 'Email' field is required.");
    if (string.IsNullOrEmpty(administratorDTO.Password)) 
        validation.Messages.Add("The 'Password' field is required.");
    if (administratorDTO.Profile == null) 
        validation.Messages.Add("The 'Profile' field is required.");

    return validation;
}


app.MapPost("/administrators/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>
{
    if (administratorService.Login(loginDTO) != null)
    {
        return Results.Ok("Login successful");
    }
    return Results.Unauthorized();
}).WithTags("Administrators");

app.MapPost("/administrators", ([FromBody] AdministratorDTO administratorDTO, IAdministratorService administratorService) =>
{
    var validation = ValidateAdministratorDTO(administratorDTO);
    if (validation.Messages.Count > 0) return Results.BadRequest(validation);
    
    var administrator = new Administrator
    {
        Email = administratorDTO.Email,
        Password = administratorDTO.Password,
        Profile = administratorDTO.Profile.ToString() ?? Profile.Editor.ToString()
    };

    administratorService.Create(administrator);
    return Results.Created($"/administrator/{administrator.Id}", new AdministratorModelView{
        Id = administrator.Id,
        Email = administrator.Email,
        Profile = administrator.Profile
    });
}).WithTags("Administrators");

app.MapGet("/administrators", ([FromQuery] int? page, IAdministratorService administratorService) =>
{
    // Defines the list to hold the model views (don't return entities directly: excludes sensitive data like passwords)
    var admins = new List<AdministratorModelView>();
    var administrators = administratorService.GetAll(page);
    foreach(var admin in administrators)
    {
        admins.Add(new AdministratorModelView{
            Id = admin.Id,
            Email = admin.Email,
            Profile = admin.Profile
        });
    }
    return Results.Ok(admins);
}).WithTags("Administrators");


app.MapGet("/administrators/{id}", ([FromRoute] int id, IAdministratorService administratorService) =>
{
    var administrator = administratorService.GetById(id);
    if (administrator == null) return Results.NotFound();
    return Results.Ok(new AdministratorModelView{
        Id = administrator.Id,
        Email = administrator.Email,
        Profile = administrator.Profile
    });
}).WithTags("Administrators");

#endregion

#region Vehicles
static ValidationErrors ValidateVehicleDTO(VehicleDTO vehicleDTO)
{
    var validation = new ValidationErrors
    {
        Messages = []
    };

    if (string.IsNullOrEmpty(vehicleDTO.Name)) validation.Messages.Add("The 'Name' field is required.");
    if (string.IsNullOrEmpty(vehicleDTO.Brand)) validation.Messages.Add("The 'Brand' field is required.");
    if (vehicleDTO.Year <= 1950) validation.Messages.Add("The 'Year' field must be greater than 1950.");

    return validation;
}

app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) => {

    var validation = ValidateVehicleDTO(vehicleDTO);
    if (validation.Messages.Count > 0) return Results.BadRequest(validation);

    var vehicle = new Vehicle
    {
        Name = vehicleDTO.Name,
        Brand = vehicleDTO.Brand,
        Year = vehicleDTO.Year
    };

    var createdVehicle = vehicleService.Create(vehicle);
    return Results.Created($"/vehicle/{createdVehicle.Id}", createdVehicle);
}).WithTags("Vehicles");

app.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService) =>
{
    var vehicles = vehicleService.GetAll(page);
    return Results.Ok(vehicles);
}).WithTags("Vehicles");

app.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetById(id);
    if (vehicle == null) return Results.NotFound();
    return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetById(id);
    if (vehicle == null) return Results.NotFound();

    var validation = ValidateVehicleDTO(vehicleDTO);
    if (validation.Messages.Count > 0) return Results.BadRequest(validation);

    vehicle.Name = vehicleDTO.Name;
    vehicle.Brand = vehicleDTO.Brand;
    vehicle.Year = vehicleDTO.Year;

    vehicleService.Update(vehicle);
    return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) => {
    var vehicle = vehicleService.GetById(id);
    if (vehicle == null) return Results.NotFound();
    vehicleService.Delete(vehicle);
    return Results.NoContent();  
}).WithTags("Vehicles");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion