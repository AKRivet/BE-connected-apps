using BeConnectedAppsTesting.Api.Extensions;
using BeConnectedAppsTesting.Application;
using BeConnectedAppsTesting.Infrastructure;
using BeConnectedAppsTesting.Infrastructure.Persistence;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());

    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddApiServices(builder.Configuration);

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
    }

    app.UseApiMiddleware();
    app.MapApiEndpoints();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Exposes Program as a public type so integration tests can reference it.
public partial class Program { }
