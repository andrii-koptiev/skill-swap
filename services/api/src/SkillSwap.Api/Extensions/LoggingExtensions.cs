using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;

namespace SkillSwap.Api.Extensions;

/// <summary>
/// Extension methods for configuring Serilog logging.
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Configures Serilog with console, file, and PostgreSQL sinks based on environment.
    /// </summary>
    public static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog(
            (context, loggerConfig) =>
            {
                var environment = context.HostingEnvironment.EnvironmentName;
                var configuration = context.Configuration;

                loggerConfig
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Environment", environment)
                    .Enrich.WithProperty("Application", "SkillSwap.Api")
                    .WriteTo.Console(
                        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj} "
                            + "{Properties:j}{NewLine}{Exception}"
                    );

                // File logging for all environments
                var logPath = configuration["Logging:FilePath"] ?? "logs/skillswap-.log";
                loggerConfig.WriteTo.File(
                    path: logPath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    fileSizeLimitBytes: 10_000_000,
                    rollOnFileSizeLimit: true,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] "
                        + "{SourceContext} {Message:lj} {Properties:j}{NewLine}{Exception}"
                );

                // PostgreSQL logging for production and staging
                if (
                    environment.Equals("Production", StringComparison.OrdinalIgnoreCase)
                    || environment.Equals("Staging", StringComparison.OrdinalIgnoreCase)
                )
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnection");
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        var columnWriters = new Dictionary<string, ColumnWriterBase>
                        {
                            { "message", new RenderedMessageColumnWriter() },
                            { "message_template", new MessageTemplateColumnWriter() },
                            { "level", new LevelColumnWriter() },
                            { "raise_date", new TimestampColumnWriter() },
                            { "exception", new ExceptionColumnWriter() },
                            { "properties", new LogEventSerializedColumnWriter() },
                            { "machine_name", new SinglePropertyColumnWriter("MachineName") },
                        };

                        loggerConfig.WriteTo.PostgreSQL(
                            connectionString: connectionString,
                            tableName: "logs",
                            columnOptions: columnWriters,
                            restrictedToMinimumLevel: LogEventLevel.Information
                        );
                    }
                }

                // Development-specific configuration
                if (environment.Equals("Development", StringComparison.OrdinalIgnoreCase))
                {
                    loggerConfig.MinimumLevel.Debug();
                }
            }
        );

        return builder;
    }

    /// <summary>
    /// Adds request logging middleware with structured logging for HTTP requests.
    /// </summary>
    public static WebApplication UseRequestLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate =
                "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            options.GetLevel = GetLogLevel;
            options.EnrichDiagnosticContext = EnrichFromRequest;
        });

        return app;
    }

    // ReSharper disable once UnusedParameter.Local
#pragma warning disable IDE0060 // Remove unused parameter
    private static LogEventLevel GetLogLevel(HttpContext ctx, double _, Exception? ex)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        if (ex != null)
            return LogEventLevel.Error;

        if (ctx.Response.StatusCode > 499)
            return LogEventLevel.Error;

        if (ctx.Response.StatusCode > 399)
            return LogEventLevel.Warning;

        return LogEventLevel.Information;
    }

    private static void EnrichFromRequest(
        IDiagnosticContext diagnosticContext,
        HttpContext httpContext
    )
    {
        var request = httpContext.Request;

        // Standard enrichments
        diagnosticContext.Set("RequestHost", request.Host.Value);
        diagnosticContext.Set("RequestScheme", request.Scheme);
        diagnosticContext.Set("UserAgent", request.Headers.UserAgent);
        diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString());

        // User context if authenticated
        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            diagnosticContext.Set(
                "UserId",
                httpContext.User.FindFirst("sub")?.Value ?? httpContext.User.FindFirst("id")?.Value
            );
            diagnosticContext.Set("UserName", httpContext.User.Identity.Name);
        }

        // Request size
        if (request.ContentLength.HasValue)
        {
            diagnosticContext.Set("RequestLength", request.ContentLength.Value);
        }

        // Response size
        if (httpContext.Response.ContentLength.HasValue)
        {
            diagnosticContext.Set("ResponseLength", httpContext.Response.ContentLength.Value);
        }
    }
}
