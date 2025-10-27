using MinimalApi.Infrastructure.Db;
using MinimalApi.Domain.DTOs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DBContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (LoginDTO loginDTO) => {
    if (loginDTO.Email == "adm@test.com" && loginDTO.Password == "123456")
    {
        return Results.Ok("Login realizado com sucesso!");
    }
    return Results.Unauthorized();
});


app.Run();