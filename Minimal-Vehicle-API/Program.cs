using Microsoft.EntityFrameworkCore;
using Minimal_Vehicle_API.Domain.Entities;
using Minimal_Vehicle_API.Infrastructure.Db;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.   

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

app.MapPost("/login", (LoginDTO loginDTO) =>
{
    if (loginDTO.Email == "adm@teste.com" && loginDTO.Password == "123456")
        return Results.Ok("Logged");
    else
        return Results.Unauthorized();
});


app.Run();


