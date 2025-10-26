var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (MinimalApi.Domain.DTOs.LoginDTO loginDTO) => {
    if (loginDTO.Email == "adm@test.com" && loginDTO.Password == "123456")
    {
        return Results.Ok("Login realizado com sucesso!");
    }
    return Results.Unauthorized();
});


app.Run();