var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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


public class LoginDTO
{
    public string Email { get; set; } = default!;

    public string Password { get; set; } = default!;
}
