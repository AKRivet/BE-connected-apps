using BeConnectedAppsTesting.Application.Projects.CreateProject;
using Microsoft.Extensions.DependencyInjection;

namespace BeConnectedAppsTesting.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<CreateProjectCommandHandler>();
        return services;
    }
}
