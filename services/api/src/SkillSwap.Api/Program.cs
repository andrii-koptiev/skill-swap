using Microsoft.EntityFrameworkCore;
using Serilog;
using SkillSwap.Api.Extensions;
using SkillSwap.Api.Middleware;
using SkillSwap.Infrastructure.Data;
using SkillSwap.Infrastructure.Data.Extensions;

// Configure Serilog early for startup logging
var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.ConfigureSerilog();

try
{
    Log.Information("Starting SkillSwap API application");

    // Add services to the container
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc(
            "v1",
            new()
            {
                Title = "SkillSwap API",
                Version = "v1",
                Description = "API for the SkillSwap peer-to-peer skill exchange platform",
            }
        );
    });

    // Add Entity Framework and PostgreSQL
    builder.Services.AddDbContext<SkillSwapDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    );

    // Add validation services
    builder.Services.AddValidation();

    // Add health checks
    builder.Services.AddHealthChecks().AddDbContextCheck<SkillSwapDbContext>();

    var app = builder.Build();

    // Configure the HTTP request pipeline

    // 1. Exception handling (must be first)
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // 2. Request logging
    app.UseRequestLogging();

    // 3. Development tools
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "SkillSwap API V1");
            c.RoutePrefix = string.Empty; // Set Swagger UI at app's root
        });
    }

    // 4. Security headers and HTTPS
    app.UseHttpsRedirection();

    // 5. Validation middleware
    app.UseValidation();

    // 6. Authentication and Authorization (when implemented)

    // 7. Map controllers and health checks
    app.MapControllers();
    app.MapHealthChecks("/health");

    // Apply migrations and seed data
    await InitializeDatabaseAsync(app);

    Log.Information("SkillSwap API application started successfully");
    await app.RunAsync();
    return 0; // Success exit code
}
catch (Exception ex)
{
    Log.Fatal(ex, "SkillSwap API application terminated unexpectedly");
    return 1; // Exit code indicating failure
}
finally
{
    await Log.CloseAndFlushAsync();
}

/// <summary>
/// Initializes the database with migrations and seed data.
/// </summary>
static async Task InitializeDatabaseAsync(WebApplication app)
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
