using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Infrastructure.Persistence;
using HjemIKroppenBooking.Infrastructure.Persistence.Queries;
using HjemIKroppenBooking.Infrastructure.Persistence.Repositories;
using HjemIKroppenBooking.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace HjemIKroppenBooking.Infrastructure;

public static class DependencyInjection
{
    /// <summary>Health checks with this tag must pass before the instance is ready for traffic.</summary>
    public const string ReadyHealthCheckTag = "ready";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services.TryAddSingleton(TimeProvider.System);

        services.AddOptions<DatabaseOptions>()
            .Configure(options => options.ConnectionString = configuration.GetConnectionString(DatabaseOptions.ConnectionStringName) ?? string.Empty)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddOptions<JwtOptions>().Bind(configuration.GetSection(JwtOptions.SectionName)).ValidateDataAnnotations().ValidateOnStart();
        services.AddSingleton<IValidateOptions<JwtOptions>, JwtOptionsValidator>();
        services.AddOptions<PasswordHashingOptions>().Bind(configuration.GetSection(PasswordHashingOptions.SectionName)).ValidateDataAnnotations().ValidateOnStart();

        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var connectionString = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConnectionString;
            options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure());
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IServiceTypeRepository, ServiceTypeRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<ISessionSeriesRepository, SessionSeriesRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IUserQueries, UserQueries>();
        services.AddScoped<IServiceTypeQueries, ServiceTypeQueries>();
        services.AddScoped<ISessionQueries, SessionQueries>();
        services.AddScoped<ISeriesQueries, SeriesQueries>();
        services.AddScoped<IBookingQueries, BookingQueries>();

        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        services.AddHealthChecks().AddDbContextCheck<AppDbContext>("database", tags: [ReadyHealthCheckTag]);

        return services;
    }
}
