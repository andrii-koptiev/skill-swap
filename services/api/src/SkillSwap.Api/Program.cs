using Microsoft.EntityFrameworkCore;
using SkillSwap.Infrastructure.Data;
using SkillSwap.Infrastructure.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework and PostgreSQL
builder.Services.AddDbContext<SkillSwapDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

// Apply migrations and seed data in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<SkillSwapDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Ensure database is created and up to date
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied successfully");

        // Seed essential data (all environments)
        await app.Services.SeedEssentialDataAsync(logger);
        logger.LogInformation("Essential data seeding completed");

        // Seed development data (dev only)
        if (app.Environment.IsDevelopment())
        {
            await app.Services.SeedDevelopmentDataAsync(logger);
            logger.LogInformation("Development data seeding completed");
        }
        else
        {
            logger.LogInformation(
                "Skipping development data seeding - not in development environment"
            );
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while applying migrations or seeding data");
        throw new InvalidOperationException(
            "Failed to initialize database during application startup",
            ex
        );
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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
    .WithName("GetWeatherForecast")
    .WithOpenApi();

await app.RunAsync();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
