using System.Net;
using BeConnectedAppsTesting.Api.Tests.Helpers;

namespace BeConnectedAppsTesting.Api.Tests;

[Collection("ApiTests")]
public sealed class HealthCheckTests
{
    private readonly CustomWebApplicationFactory _factory;

    public HealthCheckTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsOk()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
