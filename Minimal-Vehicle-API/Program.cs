using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_Vehicle_API.Domain.Entities;
using Minimal_Vehicle_API.Domain.Interfaces;
using Minimal_Vehicle_API.Infrastructure.Db;
using Minimal_Vehicle_API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.   

builder.Services.AddScoped<IAdminService, AdminService>();

builder.Services.AddDbContext<MySQLContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        new MySqlServerVersion(new Version(8, 0, 31))
    );
});
var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapGet("/", () => "Hello World");

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) =>
{
    if (adminService.Login(loginDTO) != null)
        return Results.Ok("Logged");
    else
        return Results.Unauthorized();
});


app.Run();


