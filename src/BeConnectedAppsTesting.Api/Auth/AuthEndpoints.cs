using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BeConnectedAppsTesting.Api.Auth;

internal static class AuthEndpoints
{
    private static readonly Dictionary<string, (string Password, string Role)> DevUsers =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["manager"]   = ("password123", "Manager"),
            ["developer"] = ("password123", "Developer"),
            ["admin"]     = ("password123", "Admin"),
        };

    internal static WebApplication MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/api/auth/login", (LoginRequest request, IConfiguration config) =>
        {
            if (!DevUsers.TryGetValue(request.Username, out var user) || user.Password != request.Password)
                return Results.Unauthorized();

            var key      = config["Jwt:Key"]      ?? throw new InvalidOperationException("Jwt:Key is not configured.");
            var issuer   = config["Jwt:Issuer"]   ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
            var audience = config["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, request.Username),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var signingKey  = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer:             issuer,
                audience:           audience,
                claims:             claims,
                expires:            DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Results.Ok(new LoginResponse(tokenString, request.Username, user.Role));
        });

        return app;
    }
}
