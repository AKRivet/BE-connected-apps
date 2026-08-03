using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BeConnectedAppsTesting.Api.Tests.Helpers;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("Jwt:Key", JwtTokenHelper.TestKey);
        builder.UseSetting("Jwt:Issuer", JwtTokenHelper.TestIssuer);
        builder.UseSetting("Jwt:Audience", JwtTokenHelper.TestAudience);
        builder.UseSetting("Database:UseInMemory", "true");
        builder.UseSetting("Database:InMemoryName", _dbName);
    }
}
