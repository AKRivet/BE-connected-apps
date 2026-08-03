using BeConnectedAppsTesting.Application.Projects;
using BeConnectedAppsTesting.Infrastructure.Persistence;
using BeConnectedAppsTesting.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeConnectedAppsTesting.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (configuration["Database:UseInMemory"] == "true")
        {
            var dbName = configuration["Database:InMemoryName"] ?? "BeConnectedApps";
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(dbName));
        }
        else
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(
                    configuration.GetConnectionString("DefaultConnection") ?? "Data Source=BeConnectedApps.db"));
        }

        services.AddScoped<IProjectRepository, ProjectRepository>();

        return services;
    }
}
