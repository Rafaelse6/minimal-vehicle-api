using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_Vehicle_API.Domain.DTOs;
using Minimal_Vehicle_API.Domain.Entities;
using Minimal_Vehicle_API.Domain.Interfaces;
using Minimal_Vehicle_API.Domain.ModelViews;
using Minimal_Vehicle_API.Infrastructure.Db;
using Minimal_Vehicle_API.Services;

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
app.MapPost("/admins/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) =>
{
    if (adminService.Login(loginDTO) != null)
        return Results.Ok("Logged");
    else
        return Results.Unauthorized();
}).WithTags("Admins");
#endregion

#region Vehicles
app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var vehicle = new Vehicle
    {
        Name = vehicleDTO.Name,
        Brand = vehicleDTO.Brand,
        Year = vehicleDTO.Year,
    };
    vehicleService.Add(vehicle);
    return Results.Created($"/vehicle/{vehicle.Id}", vehicle);

}).WithTags("Vehicles");

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

app.MapPut("/vehicles/{id}", ([FromRoute] int? id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.FindById(id);

    if (vehicle == null) return Results.NotFound();

    vehicle.Name = vehicleDTO.Name;
    vehicle.Brand = vehicleDTO.Brand;
    vehicle.Year = vehicleDTO.Year;
    vehicleService.Update(vehicle);

    return Results.Ok(vehicle);
}).WithTags("Vehicles");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();
#endregion

app.Run();


