using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Enuns;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.ModelViews;
using MinimalApi.Domain.Services;
using MinimalApi.Infrastructure.Db;
using BCryptNet = BCrypt.Net.BCrypt;

namespace MinimalApi
{
    public class Startup
    {
        private readonly string key;
        public IConfiguration Configuration { get; set; } = default!;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Configuration value 'Jwt:Key' is required. Set 'Jwt:Key' in appsettings or environment variables.");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });

            services.AddAuthorization();

            services.AddScoped<IAdministratorService, AdministratorService>();
            services.AddScoped<IVehicleService, VehicleService>();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Minimal API",
                    Version = "v1",
                    Description = "A Minimal API example with JWT authentication and MySQL database.",
                    Contact = new OpenApiContact
                    {
                        Name = "Giovane Souza",
                        Url = new Uri("https://github.com/giovanesouza"),
                    }
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Insert your token here."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
                });

            });

            services.AddDbContext<DBContext>(options =>
            {
                options.UseMySql(
                    Configuration.GetConnectionString("MySql"),
                    ServerVersion.AutoDetect(Configuration.GetConnectionString("MySql"))
                );
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                #region Home
                endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
                #endregion

                #region Administrators
                ValidationErrors ValidateAdministratorDTO(AdministratorDTO administratorDTO)
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

                string GenerateJwtToken(Administrator administrator)
                {
                    if (string.IsNullOrEmpty(key)) return string.Empty;

                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); // Data algorithm to encrypt the token

                    var claims = new List<Claim>()
                    {
                    new("Email", administrator.Email),
                    new("Profile", administrator.Profile),
                    new(ClaimTypes.Role, administrator.Profile)
                    };

                    var token = new JwtSecurityToken(
                        claims: claims,
                        expires: DateTime.Now.AddHours(2),
                        signingCredentials: credentials
                    );

                    return new JwtSecurityTokenHandler().WriteToken(token);
                }

                endpoints.MapPost("/administrators/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>
                {
                    var admin = administratorService.Login(loginDTO);
                    if (admin != null)
                    {
                        string token = GenerateJwtToken(admin);
                        return Results.Ok(new LoggedAdmininistratorModelView
                        {
                            Email = admin.Email,
                            Profile = admin.Profile,
                            Token = token
                        });
                    }
                    return Results.Unauthorized();
                }).AllowAnonymous().WithTags("Administrators");


                endpoints.MapPost("/administrators", ([FromBody] AdministratorDTO administratorDTO, IAdministratorService administratorService) =>
                {
                    var validation = ValidateAdministratorDTO(administratorDTO);
                    if (validation.Messages.Count > 0) return Results.BadRequest(validation);

                    var administrator = new Administrator
                    {
                        Email = administratorDTO.Email,
                        Password = BCryptNet.HashPassword(administratorDTO.Password),
                        Profile = administratorDTO.Profile.ToString() ?? Profile.Editor.ToString()
                    };

                    administratorService.Create(administrator);
                    return Results.Created($"/administrator/{administrator.Id}", new AdministratorModelView
                    {
                        Id = administrator.Id,
                        Email = administrator.Email,
                        Profile = administrator.Profile
                    });
                })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
                .WithTags("Administrators");


                endpoints.MapGet("/administrators", ([FromQuery] int? page, IAdministratorService administratorService) =>
                {
                    // Defines the list to hold the model views (don't return entities directly: excludes sensitive data like passwords)
                    var admins = new List<AdministratorModelView>();
                    var administrators = administratorService.GetAll(page);
                    foreach (var admin in administrators)
                    {
                        admins.Add(new AdministratorModelView
                        {
                            Id = admin.Id,
                            Email = admin.Email,
                            Profile = admin.Profile
                        });
                    }
                    return Results.Ok(admins);
                })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
                .WithTags("Administrators");


                endpoints.MapGet("/administrators/{id}", ([FromRoute] int id, IAdministratorService administratorService) =>
                {
                    var administrator = administratorService.GetById(id);
                    if (administrator == null) return Results.NotFound();
                    return Results.Ok(new AdministratorModelView
                    {
                        Id = administrator.Id,
                        Email = administrator.Email,
                        Profile = administrator.Profile
                    });
                })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
                .WithTags("Administrators");

                #endregion

                #region Vehicles
                ValidationErrors ValidateVehicleDTO(VehicleDTO vehicleDTO)
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


                endpoints.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
                {

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
                })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin,Editor" })
                .WithTags("Vehicles");


                endpoints.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService) =>
                {
                    var vehicles = vehicleService.GetAll(page);
                    return Results.Ok(vehicles);
                })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
                .WithTags("Vehicles");


                endpoints.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
                {
                    var vehicle = vehicleService.GetById(id);
                    if (vehicle == null) return Results.NotFound();
                    return Results.Ok(vehicle);
                })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin,Editor" })
                .WithTags("Vehicles");


                endpoints.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
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
                })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
                .WithTags("Vehicles");


                endpoints.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
                {
                    var vehicle = vehicleService.GetById(id);
                    if (vehicle == null) return Results.NotFound();
                    vehicleService.Delete(vehicle);
                    return Results.NoContent();
                })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
                .WithTags("Vehicles");
                #endregion
            });
        }
    }
}