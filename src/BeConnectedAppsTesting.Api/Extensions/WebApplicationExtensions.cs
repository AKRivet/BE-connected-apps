using BeConnectedAppsTesting.Api.Middleware;
using BeConnectedAppsTesting.Api.Projects;

namespace BeConnectedAppsTesting.Api.Extensions;

internal static class WebApplicationExtensions
{
    internal static WebApplication UseApiMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        // Redirect to HTTPS in non-Development environments; dev uses the https launch profile.
        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }

        app.UseCors(ServiceCollectionExtensions.CorsPolicyName);
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        return app;
    }

    internal static WebApplication MapApiEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health");
        app.MapProjectEndpoints();

        return app;
    }
}
