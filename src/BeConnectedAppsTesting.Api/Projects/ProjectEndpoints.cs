using BeConnectedAppsTesting.Application.Projects;
using BeConnectedAppsTesting.Application.Projects.CreateProject;

namespace BeConnectedAppsTesting.Api.Projects;

internal static class ProjectEndpoints
{
    internal static WebApplication MapProjectEndpoints(this WebApplication app)
    {
        app.MapPost("/api/projects", async (
            CreateProjectRequest request,
            CreateProjectCommandHandler handler,
            CancellationToken ct) =>
        {
            var command = new CreateProjectCommand(request.Name, request.Key, request.Description);
            var result = await handler.Handle(command, ct);

            return result.IsSuccess
                ? Results.Created($"/api/projects/{result.Value}", new { id = result.Value })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequireAuthorization("ManagerOnly");

        app.MapGet("/api/projects", async (
            IProjectRepository repository,
            CancellationToken ct) =>
        {
            var projects = await repository.GetAllAsync(ct);
            var response = projects.Select(p => new ProjectResponse(p.Id, p.Name, p.Key, p.Description));
            return Results.Ok(response);
        })
        .RequireAuthorization();

        return app;
    }
}
