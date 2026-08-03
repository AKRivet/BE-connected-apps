using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BeConnectedAppsTesting.Api.Tests.Helpers;

internal static class JwtTokenHelper
{
    internal const string TestKey = "test-super-secret-key-for-integration-tests-only-not-for-production";
    internal const string TestIssuer = "BeConnectedAppsTesting.Test";
    internal const string TestAudience = "BeConnectedAppsTesting.Test";

    internal static string GenerateToken(string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user"),
            new Claim(ClaimTypes.Role, role),
        };

        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
