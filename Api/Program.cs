using MinimalApi.Infrastructure.Db;
using MinimalApi.Domain.DTOs;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Domain.ModelViews;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministratorService, AdministratorService>();

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

app.MapGet("/", () => Results.Json(new Home()));

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) => {
    if (administratorService.Login(loginDTO) != null)
    {
        return Results.Ok("Login realizado com sucesso!");
    }
    return Results.Unauthorized();
});

app.UseSwagger();
app.UseSwaggerUI();

app.Run();