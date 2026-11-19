using System.Net;
using System.Text.Json.Serialization;
using Asp.Versioning;
using HjemIKroppenBooking.Api.ErrorHandling;
using HjemIKroppenBooking.Api.Middleware;
using HjemIKroppenBooking.Api.OpenApi;
using HjemIKroppenBooking.Api.Options;
using HjemIKroppenBooking.Api.Security;
using HjemIKroppenBooking.Application.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;

namespace HjemIKroppenBooking.Api.Extensions;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddApiControllers();
        services.AddApiErrorHandling();
        services.AddApiVersioningAndOpenApi(configuration);
        services.AddApiAuthentication();
        services.AddApiRateLimiting(configuration);
        services.AddApiCors(configuration);
        services.AddApiForwardedHeaders(configuration);
        return services;
    }

    private static void AddApiControllers(this IServiceCollection services)
    {
        services
            .AddControllers(options =>
            {
                // Secure by default: every controller action requires a signed-in user unless it opts out with [AllowAnonymous].
                options.Filters.Add(new AuthorizeFilter());
                // Missing values reach the validators, so they are reported as validation.required instead of a binding error.
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            })
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
            .ConfigureApiBehaviorOptions(options => options.InvalidModelStateResponseFactory = InvalidModelStateResponse.Create);

        // Problem responses and the OpenAPI schema use the minimal-API JSON options.
        services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    }

    private static void AddApiErrorHandling(this IServiceCollection services)
    {
        services.AddProblemDetails(options => options.CustomizeProblemDetails = context => Problems.Enrich(context.ProblemDetails, context.HttpContext));
        services.AddExceptionHandler<ApiExceptionHandler>();
    }

    private static void AddApiVersioningAndOpenApi(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        services.Configure<OpenApiSettings>(configuration.GetSection(OpenApiSettings.SectionName));
        services.AddOpenApi("v1", options =>
        {
            options.AddDocumentTransformer<DocumentInfoTransformer>();
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            options.AddOperationTransformer<BearerSecuritySchemeTransformer>();
        });
    }

    private static void AddApiAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();
        services.AddAuthorizationBuilder().AddPolicy(AuthorizationPolicies.Admin, policy => policy.RequireAuthenticatedUser().RequireRole(AuthorizationPolicies.Admin));
    }

    private static void AddApiCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CorsSettings>()
            .Bind(configuration)
            .Validate(s => s.AllowedOrigins.Count > 0, "AllowedOrigins must contain at least one origin.")
            .Validate(s => s.AllowedOrigins.All(CorsSettings.IsValidOrigin), "AllowedOrigins must be absolute http(s) origins without path or trailing slash, e.g. https://booking.example.dk.")
            .ValidateOnStart();

        services.AddCors();
        services.AddOptions<CorsOptions>().Configure<IOptions<CorsSettings>>((options, settings) => options.AddDefaultPolicy(policy => policy
            .WithOrigins([.. settings.Value.AllowedOrigins])
            .WithMethods(HttpMethods.Get, HttpMethods.Post, HttpMethods.Put, HttpMethods.Delete)
            .WithHeaders("Content-Type", "Authorization", "Accept", CorrelationIdMiddleware.HeaderName)
            .WithExposedHeaders("Location", "Retry-After", "api-supported-versions", CorrelationIdMiddleware.HeaderName)
            .SetPreflightMaxAge(TimeSpan.FromHours(1))));
    }

    private static void AddApiForwardedHeaders(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ForwardedHeadersSettings>()
            .Bind(configuration.GetSection(ForwardedHeadersSettings.SectionName))
            .Validate(s => s.IsValid(), "ForwardedHeaders:KnownProxies must be IP addresses and ForwardedHeaders:KnownNetworks CIDR ranges, e.g. 172.18.0.0/16.")
            .ValidateOnStart();

        services.AddOptions<ForwardedHeadersOptions>().Configure<IOptions<ForwardedHeadersSettings>>((options, settings) =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.ForwardLimit = 1;
            if (!settings.Value.IsConfigured)
            {
                return;
            }

            options.KnownProxies.Clear();
            options.KnownIPNetworks.Clear();
            foreach (var proxy in settings.Value.KnownProxies)
            {
                options.KnownProxies.Add(IPAddress.Parse(proxy));
            }

            foreach (var network in settings.Value.KnownNetworks)
            {
                options.KnownIPNetworks.Add(System.Net.IPNetwork.Parse(network));
            }
        });
    }
}
