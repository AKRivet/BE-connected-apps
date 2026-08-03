using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BeConnectedAppsTesting.Api.Projects;
using BeConnectedAppsTesting.Api.Tests.Helpers;
using BeConnectedAppsTesting.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace BeConnectedAppsTesting.Api.Tests.Projects;

[Collection("ApiTests")]
public sealed class CreateProjectTests : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public CreateProjectTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CreateProject_AsManager_ReturnsCreated()
    {
        Authorize("Manager");

        var response = await _client.PostAsJsonAsync("/api/projects", new
        {
            name = "Delta Project",
            key = "DT",
            description = "A test project",
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateProject_WithDuplicateKey_ReturnsBadRequest()
    {
        Authorize("Manager");

        await _client.PostAsJsonAsync("/api/projects", new
        {
            name = "Delta Project",
            key = "DT",
            description = "First project",
        });

        var response = await _client.PostAsJsonAsync("/api/projects", new
        {
            name = "Another Project",
            key = "DT",
            description = "Second project",
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateProject_AsDeveloper_ReturnsForbidden()
    {
        Authorize("Developer");

        var response = await _client.PostAsJsonAsync("/api/projects", new
        {
            name = "Delta Project",
            key = "DT",
            description = "A test project",
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateProject_AsAdmin_ReturnsForbidden()
    {
        Authorize("Admin");

        var response = await _client.PostAsJsonAsync("/api/projects", new
        {
            name = "Delta Project",
            key = "DT",
            description = "A test project",
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateProject_WithoutAuth_ReturnsUnauthorized()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.PostAsJsonAsync("/api/projects", new
        {
            name = "Delta Project",
            key = "DT",
            description = "A test project",
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateProject_ThenGetList_ContainsCreatedProject()
    {
        Authorize("Manager");

        await _client.PostAsJsonAsync("/api/projects", new
        {
            name = "Delta Project",
            key = "DT",
            description = "A test project",
        });

        var response = await _client.GetAsync("/api/projects");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var projects = await response.Content.ReadFromJsonAsync<List<ProjectResponse>>();
        Assert.NotNull(projects);
        Assert.Single(projects);
        Assert.Equal("DT", projects[0].Key);
        Assert.Equal("Delta Project", projects[0].Name);
    }

    private void Authorize(string role) =>
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtTokenHelper.GenerateToken(role));
}
