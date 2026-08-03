using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace BeConnectedAppsTesting.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    internal const string CorsPolicyName = "AllowFrontend";

    internal static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApi();
        services.AddHealthChecks();

        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        services.AddCors(options =>
            options.AddPolicy(CorsPolicyName, policy =>
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()));

        var jwtKey = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is not configured.");
        var jwtIssuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
        var jwtAudience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateLifetime = true,
                    RoleClaimType = ClaimTypes.Role,
                };
            });

        services.AddAuthorization(options =>
            options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager")));

        return services;
    }
}
