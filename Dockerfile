# syntax=docker/dockerfile:1

# ---- Build -------------------------------------------------------------------------------------------------------
# The SDK image must contain an SDK in the feature band pinned by global.json (rollForward: latestPatch).
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore in its own layer, so it is only repeated when the solution, build props or project files change.
COPY HjemIKroppenBooking.slnx Directory.Build.props Directory.Packages.props global.json nuget.config ./
COPY src/HjemIKroppenBooking.Domain/HjemIKroppenBooking.Domain.csproj src/HjemIKroppenBooking.Domain/
COPY src/HjemIKroppenBooking.Application/HjemIKroppenBooking.Application.csproj src/HjemIKroppenBooking.Application/
COPY src/HjemIKroppenBooking.Infrastructure/HjemIKroppenBooking.Infrastructure.csproj src/HjemIKroppenBooking.Infrastructure/
COPY src/HjemIKroppenBooking.Api/HjemIKroppenBooking.Api.csproj src/HjemIKroppenBooking.Api/
RUN dotnet restore src/HjemIKroppenBooking.Api/HjemIKroppenBooking.Api.csproj

# .editorconfig is part of the build: code style and analyzer rules are enforced with warnings as errors.
COPY .editorconfig ./
COPY src/ src/
RUN dotnet publish src/HjemIKroppenBooking.Api/HjemIKroppenBooking.Api.csproj -c Release --no-restore -o /app/publish /p:UseAppHost=false

# ---- Runtime -----------------------------------------------------------------------------------------------------
# Not chiseled: the health check needs a shell and curl.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Built-in non-root user of the official .NET images.
USER $APP_UID

HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 \
    CMD curl --fail --silent --show-error http://localhost:8080/health/live || exit 1

ENTRYPOINT ["dotnet", "HjemIKroppenBooking.Api.dll"]
