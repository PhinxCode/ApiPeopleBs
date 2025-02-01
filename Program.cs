using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using webApiPeople.Context;

var builder = WebApplication.CreateBuilder(args);

// ✅ Configurar Swagger correctamente
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registrar el DbContext antes de construir la app
builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// ✅ Registrar controladores
builder.Services.AddControllers(); // 🚀 Esto permite que se detecten los controladores

var app = builder.Build();

// ✅ Habilitar Swagger en desarrollo o producción
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Personas v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

// Validar conexión a la base de datos
app.MapGet(
    "pingdb",
    () =>
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return Results.Ok("✅ Successfully connected to DB");
        }
        catch (Exception ex)
        {
            return Results.Problem($"❌ Error: No hay conexión con la DB - {ex.Message}");
        }
    }
);

// Endpoint de ejemplo
var summaries = new[]
{
    "Freezing",
    "Bracing",
    "Chilly",
    "Cool",
    "Mild",
    "Warm",
    "Balmy",
    "Hot",
    "Sweltering",
    "Scorching",
};

app.MapGet(
        "/weatherforecast",
        () =>
        {
            var forecast = Enumerable
                .Range(1, 5)
                .Select(index => new WeatherForecast(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        }
    )
    .WithName("GetWeatherForecast");

// ✅ Importante para detectar los controladores
app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
