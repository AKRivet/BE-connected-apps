# BE-connected-apps-testing

A production-ready ASP.NET Core 10 Web API scaffold using Clean Architecture.

## Tech Stack

| Concern | Choice |
|---------|--------|
| Runtime | .NET 10 (`net10.0`) |
| Framework | ASP.NET Core 10 Web API (minimal APIs) |
| Logging | Serilog — Console + rolling file |
| API Docs | `Microsoft.AspNetCore.OpenApi` (`/openapi/v1.json` in Development) |
| Testing | xUnit 2 + `Microsoft.AspNetCore.Mvc.Testing` |
| Packages | Central Package Management (`Directory.Packages.props`) |

## Project Structure

```
src/
  BeConnectedAppsTesting.Api/            # HTTP layer: endpoints, middleware, DI wiring
  BeConnectedAppsTesting.Application/    # Use cases and business logic
  BeConnectedAppsTesting.Domain/         # Entities, value objects, domain primitives
  BeConnectedAppsTesting.Infrastructure/ # Data access, external services

tests/
  BeConnectedAppsTesting.Api.Tests/          # Integration tests (WebApplicationFactory)
  BeConnectedAppsTesting.Application.Tests/  # Unit tests for application services

scripts/
  build.sh   # Restore → build → test in one command
```

## Getting Started

### Prerequisites

.NET 10 SDK. Install if not present:

```bash
curl -sSL https://dot.net/v1/dotnet-install.sh | bash -s -- --channel LTS
```

Subsequent commands use `$HOME/.dotnet/dotnet` or the `dotnet` in PATH.

### Build and test

```bash
chmod +x scripts/build.sh
./scripts/build.sh
```

Or step by step:

```bash
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
```

### Run

```bash
dotnet run --project src/BeConnectedAppsTesting.Api
```

Endpoints available in Development:

| Path | Description |
|------|-------------|
| `GET /health` | Health check |
| `GET /openapi/v1.json` | OpenAPI spec (Development only) |

## Configuration

Serilog is configured via `appsettings.json` / `appsettings.{Environment}.json`.
Any key can be overridden with environment variables using `__` as the separator
(e.g. `Serilog__MinimumLevel__Default=Debug`).

| Key | Default | Notes |
|-----|---------|-------|
| `Serilog:MinimumLevel:Default` | `Information` | `Debug` in Development |
| `Serilog:WriteTo[1]:Args:path` | `logs/app-.log` | Rolling daily, 7-day retention |

## Security Audit

Run after every dependency update:

```bash
dotnet list package --vulnerable --include-transitive
```

> **Initial scaffold audit** — see `Security Audit` section at the bottom of this file for results.

## Architecture Notes

- **`Program.cs`** wires up Serilog bootstrap logging, then calls three DI extension methods
  (`AddApplicationServices`, `AddInfrastructureServices`, `AddApiServices`) before building the app.
- **`Domain`** has no external dependencies. `Result<T>` provides a discriminated-union pattern
  for operations that can fail without exceptions.
- **`Application`** and **`Infrastructure`** expose a single `DependencyInjection.AddXxxServices`
  entry-point; add service registrations there.
- HTTPS redirection is enabled in non-Development environments. Development uses the
  `https` launch profile in `launchSettings.json`.
- The `ExceptionHandlingMiddleware` returns structured JSON for unhandled exceptions and
  maps common exception types to HTTP status codes.
- `public partial class Program { }` at the bottom of `Program.cs` exposes the entry point
  to `WebApplicationFactory<Program>` in integration tests.

---

### Security Audit (initial scaffold — 2026-08-03)

```
dotnet list package --vulnerable --include-transitive
```

Result: **no vulnerable packages** across all six projects.

**Note**: `Microsoft.AspNetCore.OpenApi 10.0.10` transitively pulled `Microsoft.OpenApi 2.0.0`
which is affected by GHSA-v5pm-xwqc-g5wc (high severity). The scaffold pins
`Microsoft.OpenApi 2.11.0` globally via `Directory.Build.props` + `Directory.Packages.props`
to remediate this. Remove the override once `Microsoft.AspNetCore.OpenApi` ships a version
that depends on `>= 2.11.0` itself.
