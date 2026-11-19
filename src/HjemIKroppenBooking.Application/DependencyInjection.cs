using System.Reflection;
using FluentValidation;
using HjemIKroppenBooking.Application.Features.Bookings.Common;
using HjemIKroppenBooking.Application.Features.Series.Common;
using HjemIKroppenBooking.Application.Features.Users.Common;
using HjemIKroppenBooking.Application.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace HjemIKroppenBooking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddScoped<IDispatcher, Dispatcher>();
        services.AddScoped<BookingCreator>();
        services.AddScoped<BookingCanceller>();
        services.AddScoped<SeriesBookingCanceller>();
        services.AddScoped<UserDeactivator>();
        AddHandlers(services, assembly);
        services.AddValidatorsFromAssembly(assembly, ServiceLifetime.Scoped, includeInternalTypes: true);

        return services;
    }

    private static void AddHandlers(IServiceCollection services, Assembly assembly)
    {
        Type[] handlerInterfaces = [typeof(ICommandHandler<,>), typeof(IQueryHandler<,>)];

        var registrations = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && handlerInterfaces.Contains(i.GetGenericTypeDefinition()))
                .Select(i => (Service: i, Implementation: t)));

        foreach (var (service, implementation) in registrations)
        {
            services.AddScoped(service, implementation);
        }
    }
}
