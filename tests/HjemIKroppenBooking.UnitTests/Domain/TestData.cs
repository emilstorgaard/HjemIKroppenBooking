using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.Series;
using HjemIKroppenBooking.Domain.ServiceCategories;
using HjemIKroppenBooking.Domain.ServiceTypes;
using HjemIKroppenBooking.Domain.Sessions;

namespace HjemIKroppenBooking.UnitTests.Domain;

/// <summary>Builders and assertions shared by the domain tests. All times are UTC and relative to <see cref="Now"/>.</summary>
internal static class TestData
{
    public static readonly DateTime Now = new(2027, 3, 1, 12, 0, 0, DateTimeKind.Utc);

    public static readonly Guid AdminId = Guid.NewGuid();

    public static ServiceCategory Category() => ServiceCategory.Create("Danseterapi", "danseterapi");

    public static ServiceType GroupServiceType(bool isActive = true) =>
        ServiceType.Create(Category(), "Theatre group", null, 90, 10, isActive);

    public static ServiceType IndividualServiceType() =>
        ServiceType.Create(Category(), "Dance therapy", null, 60, 1, isActive: true);

    /// <summary>Schedules a session (created at <see cref="Now"/>) starting one day from now by default.</summary>
    public static Session CreateSession(int capacity = 5, DateTime? startsAt = null, int durationMinutes = 60, ServiceType? serviceType = null, SessionSeries? series = null)
    {
        var start = startsAt ?? Now.AddDays(1);
        return Session.Schedule(serviceType ?? GroupServiceType(), series, start, start.AddMinutes(durationMinutes), capacity, "Studio", null, AdminId, Now);
    }

    /// <summary>Creates a series with weekly sessions, the first one starting one day from now.</summary>
    public static (SessionSeries Series, List<Session> Sessions) CreateSeries(int sessionCount, int capacity = 5)
    {
        var serviceType = GroupServiceType();
        var series = SessionSeries.Create(serviceType, "Spring theatre");
        var sessions = Enumerable.Range(0, sessionCount)
            .Select(i => CreateSession(capacity, Now.AddDays(1 + (7 * i)), serviceType: serviceType, series: series))
            .ToList();
        return (series, sessions);
    }

    /// <summary>Asserts that the exception is a <see cref="DomainException"/> with the expected code and type.</summary>
    public static DomainException AssertDomainError(Exception? exception, string expectedCode, ErrorType expectedType)
    {
        var domainException = Assert.IsType<DomainException>(exception);
        Assert.Equal(expectedCode, domainException.Code);
        Assert.Equal(expectedType, domainException.ErrorType);
        return domainException;
    }
}
