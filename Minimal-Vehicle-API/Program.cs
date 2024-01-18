using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_Vehicle_API.Domain.Entities;
using Minimal_Vehicle_API.Domain.Interfaces;
using Minimal_Vehicle_API.Domain.ModelViews;
using Minimal_Vehicle_API.Infrastructure.Db;
using Minimal_Vehicle_API.Services;

#region Builder
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.   

builder.Services.AddScoped<IAdminService, AdminService>();

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
app.MapGet("/", () => Results.Json(new Home()));
#endregion

#region Admins
app.MapPost("/admins/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) =>
{
    if (adminService.Login(loginDTO) != null)
        return Results.Ok("Logged");
    else
        return Results.Unauthorized();
});
#endregion

#region Vehicles

#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();
#endregion

app.Run();


