using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_Vehicle_API.Domain.DTOs;
using Minimal_Vehicle_API.Domain.Entities;
using Minimal_Vehicle_API.Domain.Enums;
using Minimal_Vehicle_API.Domain.Interfaces;
using Minimal_Vehicle_API.Domain.ModelViews;
using Minimal_Vehicle_API.Infrastructure.Db;
using Minimal_Vehicle_API.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

#region Builder
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.   

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MySQLContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        new MySqlServerVersion(new Version(8, 0, 31))
    );
});
var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Admins
app.MapGet("/admins", ([FromQuery] int? page, IAdminService adminService) =>
{
    var adms = new List<AdminModelView>();
    var admins = adminService.FindAll(page);
    foreach(var adm in admins)
    {
        adms.Add(new AdminModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Profile = adm.Profile
        });
    }

    return Results.Ok(adms);
}).WithTags("Admins");

app.MapGet("/admins/{id}", ([FromRoute] int id, IAdminService adminService) =>
{
    var adm = adminService.FindById(id);
    if (adm == null) return Results.NotFound();
    return Results.Ok(new AdminModelView
    {
        Id = adm.Id,
        Email = adm.Email,
        Profile = adm.Profile
    });
}).WithTags("Admins");

app.MapPost("/admins", ([FromBody] AdminDTO adminDTO, IAdminService adminService) => {
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

}).WithTags("Admins");

app.MapPost("/admins/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) =>
{
    if (adminService.Login(loginDTO) != null)
        return Results.Ok("Logged");
    else
        return Results.Unauthorized();
}).WithTags("Admins");
#endregion

#region Vehicles

ErrorsHandling validateDTO(VehicleDTO vehicleDTO)
{
    var validation = new ErrorsHandling {
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

app.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService) =>
{
    var vehicles = vehicleService.GetVehicles(page);

    return Results.Ok(vehicles);
}).WithTags("Vehicles");


app.MapGet("/vehicles/{id}", ([FromRoute] int? id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.FindById(id);
    if (vehicle == null) return Results.NotFound();
    return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
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

}).WithTags("Vehicles");

app.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
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
}).WithTags("Vehicles");

app.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.FindById(id);

    if (vehicle == null) return Results.NotFound();

    vehicleService.Delete(vehicle);

    return Results.NoContent();
}).WithTags("Vehicles");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();
#endregion

app.Run();


