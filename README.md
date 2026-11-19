# Hjem i Kroppen Booking API

ASP.NET Core Web API (.NET 10) for booking dance, conversation and group sessions. Users book single sessions or whole series; admins manage service types, sessions, series, bookings and users. The API is consumed by a separate (Danish) frontend.

- [Architecture](#architecture)
- [Business rules](#business-rules)
- [API conventions](#api-conventions)
- [Local setup](#local-setup)
- [Configuration](#configuration)
- [Migrations](#migrations)
- [Promoting a user to Admin](#promoting-a-user-to-admin)
- [Tests](#tests)
- [Docker deployment](#docker-deployment)
- [CI](#ci)

## Architecture

```
src/
  HjemIKroppenBooking.Domain           Aggregates, invariants, error codes. No dependencies.
  HjemIKroppenBooking.Application      Use cases (commands/queries), validators, response contracts, abstractions.
  HjemIKroppenBooking.Infrastructure   EF Core (SQL Server), repositories, read queries, migrations, JWT, BCrypt.
  HjemIKroppenBooking.Api              Controllers, request contracts, error handling, security, hosting.
tests/
  HjemIKroppenBooking.UnitTests        Domain, handlers, validators, error catalog.
  HjemIKroppenBooking.IntegrationTests WebApplicationFactory + Testcontainers SQL Server.
```

Dependencies point inwards: `Domain ← Application ← Infrastructure / Api`.

- **Domain:** `Session` is the aggregate root for capacity. It owns its bookings, keeps a denormalized `ActiveBookingCount` and has a SQL Server `rowversion` concurrency token. `SessionSeries` books and cancels across its sessions all-or-nothing. Rule violations throw `DomainException(code, ErrorType)`.
- **Application:** a small hand-rolled dispatcher (no MediatR). Each use case lives in `Features/<Area>/<UseCase>/` with its command or query, handler and FluentValidation validator. The pipeline validates, runs the handler, and for commands retries once after a concurrency conflict before returning `409` with a stable code. Reads go through `I*Queries` projections; writes load aggregates through repositories and save once per command.
- **Infrastructure:** `AppDbContext` stores every `DateTime` as UTC and maps unique index violations to error codes (for example `booking.already_exists`). A filtered unique index guarantees at most one active booking per user and session.
- **Api:** controllers only translate HTTP to commands/queries. All user-facing text lives in `Api/ErrorHandling/ErrorCatalog.cs` (Danish); everything else is English.

Time comes from `TimeProvider` (`FakeTimeProvider` in tests). All times are UTC; time zone conversion is the frontend's job.

## Business rules

- A session never has more active bookings than its capacity, and a user has at most one active booking per session. These are the only booking invariants.
- Overlapping sessions and overlapping bookings are allowed, for admins and users alike.
- A session can be booked until it starts (`session.already_started`). A booking can be cancelled until the session starts (`booking.cancellation_closed`).
- A series booking books every session of the series that has not started, in one transaction. Sessions the user has already booked are skipped. If one session is full nothing is booked, and the error names the full session in `parameters.sessionId`.
- Cancelling a series booking cancels only bookings on sessions that have not started. Cancelling a series cancels the series, its sessions that have not started and their bookings.
- Capacity cannot be set below the number of active bookings (`session.capacity_below_active_bookings`), and a session with active bookings cannot be deleted (`session.has_active_bookings`).

## API conventions

- Base path `/api/v1` (URL versioning). Admin endpoints are under `/api/v1/admin/...` and require the `Admin` role; the signed-in user's own resources are under `/api/v1/me/...`.
- JSON is camelCase, enums are strings, times are ISO 8601 UTC, collections are never `null`.
- Create returns `201` with `Location`; delete and state changes without a body return `204`.
- Lists take `page` (1-based) and `pageSize` (default 20, max 100) and return `{ items, page, pageSize, totalCount, totalPages }`.
- Authentication: `POST /api/v1/auth/login` returns a JWT valid for 12 hours. Send it as `Authorization: Bearer <token>`. Deactivated users are rejected on every request, even with a valid token.
- Every error is `application/problem+json` with a stable `code`, a Danish `detail` that can be shown to the user, `traceId` and `correlationId`:

  ```json
  {
    "status": 409,
    "detail": "Tiden er desværre fuldt booket.",
    "code": "session.full",
    "parameters": { "sessionId": "…", "startsAt": "2027-03-01T17:00:00Z" },
    "traceId": "36c79c0826c2b427601043e4e7197d21",
    "correlationId": "36c79c0826c2b427601043e4e7197d21"
  }
  ```

  Validation errors return `422` with `errors: { "<field>": [ { "code": "validation.required", "message": "Feltet skal udfyldes." } ] }`. Malformed requests return `400 request.invalid`.

| Status | Meaning |
|---|---|
| 400 | Request could not be read (`request.invalid`) |
| 401 | Not signed in, or wrong credentials (`auth.*`) |
| 403 | Signed in but not allowed (`auth.forbidden`) |
| 404 | Resource not found (`*.not_found`) |
| 409 | Business conflict (`session.full`, `booking.already_exists`, …) |
| 422 | Validation failed (`validation.*`) |
| 429 | Rate limited (`request.rate_limited`, with `Retry-After`) |
| 500 | Unexpected error (`server.unexpected_error`, details only in the logs) |

Clients may send `X-Correlation-Id`; it is echoed in the response and included in every log line of the request.

The OpenAPI document (`/openapi/v1.json`) and the Scalar UI (`/scalar`) are available in Development, or elsewhere when `OpenApi__Enabled=true`.

## Local setup

Requirements: .NET SDK 10.0.404 (see `global.json`) and a SQL Server you can reach.

```powershell
dotnet tool restore

# Secrets for Development (stored outside the repository)
$key = [Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(48))
dotnet user-secrets set "Jwt:Key" $key --project src/HjemIKroppenBooking.Api
dotnet user-secrets set "ConnectionStrings:Default" "Server=localhost;Database=HjemIKroppenBookingDb;Trusted_Connection=True;TrustServerCertificate=True" --project src/HjemIKroppenBooking.Api

# Create the database (see Migrations)
$env:ConnectionStrings__Default = "Server=localhost;Database=HjemIKroppenBookingDb;Trusted_Connection=True;TrustServerCertificate=True"
dotnet ef database update --project src/HjemIKroppenBooking.Infrastructure --startup-project src/HjemIKroppenBooking.Infrastructure

dotnet run --project src/HjemIKroppenBooking.Api
```

Then open http://localhost:5014/scalar. `appsettings.Development.json` allows the CORS origins `http://localhost:5173`, `:4173` and `:3000`.

## Configuration

No secrets are stored in `appsettings*.json`. Settings use the usual ASP.NET Core sources; as environment variables, `:` becomes `__`. Every setting is validated at startup, and the app refuses to start with a clear message when one is missing or invalid.

| Setting | Environment variable | Required | Default | Notes |
|---|---|---|---|---|
| `ConnectionStrings:Default` | `ConnectionStrings__Default` | Yes | | SQL Server connection string |
| `Jwt:Key` | `Jwt__Key` | Yes | | At least 32 bytes; values containing `CHANGE_ME` are rejected |
| `Jwt:Issuer` / `Jwt:Audience` | `Jwt__Issuer` / `Jwt__Audience` | | `HjemIKroppenBooking` | |
| `Jwt:ExpiryHours` | `Jwt__ExpiryHours` | | `12` | 1–24 |
| `AllowedOrigins` | `AllowedOrigins__0`, `AllowedOrigins__1`, … | Yes | Localhost in Development | CORS origins, e.g. `https://booking.example.dk` |
| `PasswordHashing:WorkFactor` | `PasswordHashing__WorkFactor` | | `12` | BCrypt work factor, 10–16 |
| `RateLimiting:GlobalPermitLimitPerMinute` | `RateLimiting__GlobalPermitLimitPerMinute` | | `300` | Per client IP |
| `RateLimiting:AuthPermitLimitPerMinute` | `RateLimiting__AuthPermitLimitPerMinute` | | `10` | Login and register, per client IP |
| `ForwardedHeaders:KnownNetworks` | `ForwardedHeaders__KnownNetworks__0`, … | Behind a proxy | | CIDR ranges of trusted reverse proxies |
| `ForwardedHeaders:KnownProxies` | `ForwardedHeaders__KnownProxies__0`, … | | | IP addresses of trusted reverse proxies |
| `OpenApi:Enabled` | `OpenApi__Enabled` | | `false` | OpenAPI and Scalar outside Development |

HTTPS is terminated by the reverse proxy. The API itself listens on HTTP and does not redirect to HTTPS or send HSTS.

## Migrations

Migrations live in `src/HjemIKroppenBooking.Infrastructure/Persistence/Migrations`. The API never migrates on startup; migrations are applied manually. `dotnet-ef` is pinned in `.config/dotnet-tools.json`, so run `dotnet tool restore` once.

`Infrastructure` is both the migrations project and the design-time startup project. Its design-time factory reads the connection string from the `ConnectionStrings__Default` environment variable (user-secrets are not used here). Generating a migration or a script does not connect to a database.

**Add a migration** after changing the model:

```powershell
dotnet ef migrations add <Name> --project src/HjemIKroppenBooking.Infrastructure --startup-project src/HjemIKroppenBooking.Infrastructure --output-dir Persistence/Migrations
```

**Apply to a development database:**

```powershell
$env:ConnectionStrings__Default = "<connection string>"
dotnet ef database update --project src/HjemIKroppenBooking.Infrastructure --startup-project src/HjemIKroppenBooking.Infrastructure
```

**Apply to production** with the idempotent script. It is safe to run more than once, because it only applies migrations that are missing. CI publishes it as the `migration-script` artifact; to create it locally:

```powershell
dotnet ef migrations script --idempotent --project src/HjemIKroppenBooking.Infrastructure --startup-project src/HjemIKroppenBooking.Infrastructure --output artifacts/migrations/migrate-idempotent.sql
```

Run the script against the database (SSMS, Azure Data Studio or `sqlcmd -i migrate-idempotent.sql`) **before** starting the new API version.

## Promoting a user to Admin

There is no admin seeding. Register a normal user through `POST /api/v1/auth/register`, then promote it in the database:

```sql
UPDATE Users SET Role = 'Admin' WHERE Email = 'admin@example.dk';
```

The role is part of the access token, so sign in again afterwards. From then on, admins can change other users' roles through `PUT /api/v1/admin/users/{id}/role` (admins cannot change their own role or deactivate themselves).

## Tests

Tests use xUnit v3 on Microsoft.Testing.Platform (configured in `global.json`), so each test project is run with `--project`:

```powershell
dotnet test --project tests/HjemIKroppenBooking.UnitTests/HjemIKroppenBooking.UnitTests.csproj
dotnet test --project tests/HjemIKroppenBooking.IntegrationTests/HjemIKroppenBooking.IntegrationTests.csproj
```

The integration tests start SQL Server with Testcontainers, so Docker must be running. They build the schema from the migrations and control time with `FakeTimeProvider`.

## Docker deployment

The image is a multi-stage build on the official .NET 10 images. It runs as the non-root `app` user, listens on port `8080` and has a `HEALTHCHECK` on `/health/live`. `docker-compose.yml` runs only the API. SQL Server runs in its own container, and both are attached to the external `proxy-net` network, together with the reverse proxy that terminates HTTPS and forwards to `hjem-i-kroppen-booking-api:8080`.

```bash
cp .env.example .env            # fill in every value; compose refuses to start while one is missing
docker network inspect proxy-net --format '{{(index .IPAM.Config 0).Subnet}}'   # value for TRUSTED_PROXY_NETWORK
# apply the idempotent migration script to the database first (see Migrations)
docker compose up -d --build
docker compose logs -f api
```

| `.env` variable | Maps to |
|---|---|
| `DB_CONNECTION_STRING` | `ConnectionStrings__Default` |
| `ALLOWED_ORIGIN` | `AllowedOrigins__0` |
| `JWT_KEY` | `Jwt__Key` |
| `TRUSTED_PROXY_NETWORK` | `ForwardedHeaders__KnownNetworks__0` |

Health endpoints, both anonymous and not rate limited:

- `GET /health/live`: the process is running (used by the Docker health check).
- `GET /health/ready`: the database answers.

Logs are JSON on stdout, one object per line, with `TraceId` and `CorrelationId` in the scopes.

The API does not use ASP.NET Core Data Protection for anything it stores, so the startup warning about keys not being persisted outside the container is harmless.

## CI

`.github/workflows/ci.yml` runs on pushes to `main` and on pull requests:

1. Set up the SDK from `global.json`, restore with a NuGet cache and build in Release (warnings are errors).
2. Run the unit tests, then the integration tests (Docker is available on `ubuntu-latest`).
3. Create the idempotent migration script.
4. Upload `test-results` (TRX) and `migration-script` as artifacts.
5. In a parallel job, check that the Docker image builds (it is not pushed).
