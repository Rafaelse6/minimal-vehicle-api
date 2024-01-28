using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minimal_Vehicle_API.Domain.DTOs;
using Minimal_Vehicle_API.Domain.Entities;
using Minimal_Vehicle_API.Domain.Enums;
using Minimal_Vehicle_API.Domain.Interfaces;
using Minimal_Vehicle_API.Domain.ModelViews;
using Minimal_Vehicle_API.Infrastructure.Db;
using Minimal_Vehicle_API.Services;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        key = Configuration?.GetSection("Jwt")?.ToString() ?? "";
    }

    private string key = "";
    public IConfiguration Configuration { get; set; } = default!;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option =>
        {
            option.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false,
            };
        });

        services.AddAuthorization();

        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IVehicleService, VehicleService>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insert your JWT Token Here"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme{
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });

        services.AddDbContext<MySQLContext>(options =>
        {
            options.UseMySql(
                Configuration.GetConnectionString("mysql"),
                ServerVersion.AutoDetect(Configuration.GetConnectionString("mysql"))
            );
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });
    }

    public void Configure(IApplicationBuilder endpoints, IWebHostEnvironment env)
    {
        endpoints.UseSwagger();
        endpoints.UseSwaggerUI();

        endpoints.UseRouting();

        endpoints.UseAuthentication();
        endpoints.UseAuthorization();

        endpoints.UseCors();

        endpoints.UseEndpoints(endpoints =>
        {

            #region Home
            endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
            #endregion

            #region Admins
            string GenerateJwtToken(Admin admin)
            {
                if (string.IsNullOrEmpty(key)) return string.Empty;

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>()
                 {
                     new Claim("Email", admin.Email),
                     new Claim("Profile", admin.Profile),
                     new Claim(ClaimTypes.Role, admin.Profile)
                 };

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            endpoints.MapGet("/admins", ([FromQuery] int? page, IAdminService adminService) =>
            {
                var adms = new List<AdminModelView>();
                var admins = adminService.FindAll(page);
                foreach (var adm in admins)
                {
                    adms.Add(new AdminModelView
                    {
                        Id = adm.Id,
                        Email = adm.Email,
                        Profile = adm.Profile
                    });
                }

                return Results.Ok(adms);
            })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                .WithTags("Admins");

            endpoints.MapGet("/admins/{id}", ([FromRoute] int id, IAdminService adminService) =>
            {
                var adm = adminService.FindById(id);
                if (adm == null) return Results.NotFound();
                return Results.Ok(new AdminModelView
                {
                    Id = adm.Id,
                    Email = adm.Email,
                    Profile = adm.Profile
                });
            })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                .WithTags("Admins");

            endpoints.MapPost("/admins", ([FromBody] AdminDTO adminDTO, IAdminService adminService) =>
            {
                var validation = new ErrorsHandling
                {
                    Messages = new List<string>()
                };
                if (string.IsNullOrEmpty(adminDTO.Email))
                    validation.Messages.Add("Email can not be empty");
                if (string.IsNullOrEmpty(adminDTO.Password))
                    validation.Messages.Add("Password can not be empty");
                if (adminDTO.Profile == null)
                    validation.Messages.Add("Profile can not be empty");
                if (validation.Messages.Count > 0)
                    return Results.BadRequest(validation);

                var admin = new Admin
                {
                    Email = adminDTO.Email,
                    Password = adminDTO.Password,
                    Profile = adminDTO.Profile.ToString() ?? Profile.Editor.ToString()
                };
                adminService.Add(admin);
                return Results.Created($"/administrador/{admin.Id}", new AdminModelView
                {
                    Id = admin.Id,
                    Email = admin.Email,
                    Profile = admin.Profile
                });

            })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                .WithTags("Admins");

            endpoints.MapPost("/admins/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) =>
            {
                var adm = adminService.Login(loginDTO);

                if (adminService.Login(loginDTO) != null)
                {
                    string token = GenerateJwtToken(adm);
                    return Results.Ok(new AdmLogged
                    {
                        Email = adm.Email,
                        Profile = adm.Profile,
                        Token = token
                    });
                }
                else
                    return Results.Unauthorized();
            }).AllowAnonymous().WithTags("Admins");
            #endregion

            #region Vehicles

            ErrorsHandling validateDTO(VehicleDTO vehicleDTO)
            {
                var validation = new ErrorsHandling
                {
                    Messages = new List<String>()
                };

                if (string.IsNullOrEmpty(vehicleDTO.Name))
                    validation.Messages.Add("Name can not be empty");

                if (string.IsNullOrEmpty(vehicleDTO.Brand))
                    validation.Messages.Add("Brand can not be empty");

                if (vehicleDTO.Year < 1950)
                    validation.Messages.Add("The year must be from 1950 to actual date");

                return validation;
            }

            endpoints.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService) =>
            {
                var vehicles = vehicleService.GetVehicles(page);

                return Results.Ok(vehicles);
            })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
                .WithTags("Vehicles");


            endpoints.MapGet("/vehicles/{id}", ([FromRoute] int? id, IVehicleService vehicleService) =>
            {
                var vehicle = vehicleService.FindById(id);
                if (vehicle == null) return Results.NotFound();
                return Results.Ok(vehicle);
            })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
                .WithTags("Vehicles");

            endpoints.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
            {
                var validation = validateDTO(vehicleDTO);

                if (validation.Messages.Count > 0)
                    return Results.BadRequest(validation);

                var vehicle = new Vehicle
                {
                    Name = vehicleDTO.Name,
                    Brand = vehicleDTO.Brand,
                    Year = vehicleDTO.Year,
                };
                vehicleService.Add(vehicle);
                return Results.Created($"/vehicle/{vehicle.Id}", vehicle);

            })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
                .WithTags("Vehicles");

            endpoints.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
            {
                var validation = validateDTO(vehicleDTO);

                if (validation.Messages.Count > 0)
                    return Results.BadRequest(validation);

                var vehicle = vehicleService.FindById(id);

                if (vehicle == null) return Results.NotFound();

                vehicle.Name = vehicleDTO.Name;
                vehicle.Brand = vehicleDTO.Brand;
                vehicle.Year = vehicleDTO.Year;
                vehicleService.Update(vehicle);

                return Results.Ok(vehicle);
            })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                .WithTags("Vehicles");

            endpoints.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
            {
                var vehicle = vehicleService.FindById(id);

                if (vehicle == null) return Results.NotFound();

                vehicleService.Delete(vehicle);

                return Results.NoContent();
            })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                .WithTags("Vehicles");
            #endregion
        });
    }
}