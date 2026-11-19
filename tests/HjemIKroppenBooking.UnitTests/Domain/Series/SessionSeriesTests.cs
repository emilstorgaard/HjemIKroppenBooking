using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.Series;
using HjemIKroppenBooking.Domain.ServiceTypes;
using static HjemIKroppenBooking.UnitTests.Domain.TestData;

namespace HjemIKroppenBooking.UnitTests.Domain.Series;

public sealed class SessionSeriesTests
{
    private static readonly Guid UserA = Guid.NewGuid();
    private static readonly Guid UserB = Guid.NewGuid();

    [Fact]
    public void Create_IndividualServiceType_ThrowsNotGroup()
    {
        // Arrange
        var serviceType = IndividualServiceType();

        // Act
        var exception = Record.Exception(() => SessionSeries.Create(serviceType, "Series"));

        // Assert
        AssertDomainError(exception, ErrorCodes.ServiceTypeNotGroup, ErrorType.Validation);
    }

    [Fact]
    public void Create_InactiveServiceType_ThrowsServiceTypeInactive()
    {
        // Arrange
        var serviceType = GroupServiceType(isActive: false);

        // Act
        var exception = Record.Exception(() => SessionSeries.Create(serviceType, "Series"));

        // Assert
        AssertDomainError(exception, ErrorCodes.ServiceTypeInactive, ErrorType.Conflict);
    }

    [Fact]
    public void Create_WithoutTitle_UsesServiceTypeName()
    {
        // Arrange
        var serviceType = ServiceType.Create(ServiceCategory.Group, "Saturday group", null, 60, 8, 0, isActive: true);

        // Act
        var series = SessionSeries.Create(serviceType, "   ");

        // Assert
        Assert.Equal("Saturday group", series.Title);
        Assert.Equal(serviceType.Id, series.ServiceTypeId);
    }

    [Fact]
    public void Book_AllSessionsUpcoming_BooksEverySessionInOrder()
    {
        // Arrange
        var (series, sessions) = CreateSeries(sessionCount: 3);

        // Act
        var bookings = series.Book(sessions, UserA, "Note", Now);

        // Assert
        Assert.Equal(sessions.Select(s => s.Id), bookings.Select(b => b.SessionId));
        Assert.All(sessions, s => Assert.Equal(1, s.ActiveBookingCount));
    }

    [Fact]
    public void Book_SomeSessionsStarted_BooksOnlySessionsNotStarted()
    {
        // Arrange
        var (series, sessions) = CreateSeries(sessionCount: 3);
        var afterFirstStart = sessions[0].StartsAtUtc.AddMinutes(1);

        // Act
        var bookings = series.Book(sessions, UserA, null, afterFirstStart);

        // Assert
        Assert.Equal(2, bookings.Count);
        Assert.Equal(0, sessions[0].ActiveBookingCount);
        Assert.Equal(1, sessions[1].ActiveBookingCount);
        Assert.Equal(1, sessions[2].ActiveBookingCount);
    }

    [Fact]
    public void Book_UserAlreadyBookedOneSession_SkipsThatSession()
    {
        // Arrange
        var (series, sessions) = CreateSeries(sessionCount: 3);
        sessions[1].Book(UserA, null, Now);

        // Act
        var bookings = series.Book(sessions, UserA, null, Now);

        // Assert
        Assert.Equal(2, bookings.Count);
        Assert.DoesNotContain(bookings, b => b.SessionId == sessions[1].Id);
        Assert.All(sessions, s => Assert.Equal(1, s.ActiveBookingCount));
    }

    [Fact]
    public void Book_OneSessionFull_BooksNothingAndIdentifiesFullSession()
    {
        // Arrange
        var (series, sessions) = CreateSeries(sessionCount: 3, capacity: 1);
        sessions[2].Book(UserB, null, Now);

        // Act
        var exception = Record.Exception(() => series.Book(sessions, UserA, null, Now));

        // Assert
        var error = AssertDomainError(exception, ErrorCodes.SessionFull, ErrorType.Conflict);
        Assert.Equal(sessions[2].Id, error.Parameters["sessionId"]);
        Assert.Equal(0, sessions[0].ActiveBookingCount);
        Assert.Equal(0, sessions[1].ActiveBookingCount);
        Assert.Empty(sessions[0].Bookings);
        Assert.Empty(sessions[1].Bookings);
    }

    [Fact]
    public void Book_SessionCancelled_SkipsCancelledSession()
    {
        // Arrange
        var (series, sessions) = CreateSeries(sessionCount: 2);
        sessions[0].Cancel(Now);

        // Act
        var bookings = series.Book(sessions, UserA, null, Now);

        // Assert
        var booking = Assert.Single(bookings);
        Assert.Equal(sessions[1].Id, booking.SessionId);
    }

    [Fact]
    public void Book_AllSessionsStarted_ThrowsNoUpcomingSessions()
    {
        // Arrange
        var (series, sessions) = CreateSeries(sessionCount: 2);
        var afterLastStart = sessions[^1].StartsAtUtc.AddMinutes(1);

        // Act
        var exception = Record.Exception(() => series.Book(sessions, UserA, null, afterLastStart));

        // Assert
        AssertDomainError(exception, ErrorCodes.SeriesNoUpcomingSessions, ErrorType.Conflict);
    }

    [Fact]
    public void Book_AllUpcomingSessionsAlreadyBooked_ThrowsAlreadyBooked()
    {
        // Arrange
        var (series, sessions) = CreateSeries(sessionCount: 2);
        series.Book(sessions, UserA, null, Now);

        // Act
        var exception = Record.Exception(() => series.Book(sessions, UserA, null, Now));

        // Assert
        AssertDomainError(exception, ErrorCodes.SeriesAlreadyBooked, ErrorType.Conflict);
    }

    [Fact]
    public void Book_CancelledSeries_ThrowsSeriesCancelled()
    {
        // Arrange
        var (series, sessions) = CreateSeries(sessionCount: 2);
        series.Cancel(sessions, Now);

        // Act
        var exception = Record.Exception(() => series.Book(sessions, UserA, null, Now));

        // Assert
        AssertDomainError(exception, ErrorCodes.SeriesCancelled, ErrorType.Conflict);
    }

    [Fact]
    public void Book_SessionFromAnotherSeries_ThrowsArgumentException()
    {
        // Arrange
        var (series, sessions) = CreateSeries(sessionCount: 1);
        var (_, otherSessions) = CreateSeries(sessionCount: 1);
        var mixed = sessions.Concat(otherSessions).ToList();

        // Act
        var exception = Record.Exception(() => series.Book(mixed, UserA, null, Now));

        // Assert
        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void Book_OverlappingSeries_BothSucceed()
    {
        // Arrange
        var (first, firstSessions) = CreateSeries(sessionCount: 2);
        var (second, secondSessions) = CreateSeries(sessionCount: 2);

        // Act
        var firstBookings = first.Book(firstSessions, UserA, null, Now);
        var secondBookings = second.Book(secondSessions, UserA, null, Now);

        // Assert
        Assert.Equal(firstSessions[0].StartsAtUtc, secondSessions[0].StartsAtUtc);
        Assert.Equal(2, firstBookings.Count);
        Assert.Equal(2, secondBookings.Count);
    }

    [Fact]
    public void CancelBookingsFor_SomeSessionsStarted_CancelsOnlySessionsNotStarted()
    {
        // Arrange
        var (series, sessions) = CreateSeries(sessionCount: 3);
        var bookings = series.Book(sessions, UserA, null, Now);
        var afterFirstStart = sessions[0].StartsAtUtc.AddMinutes(1);

        // Act
        var cancelled = series.CancelBookingsFor(sessions, UserA, afterFirstStart);

        // Assert
        Assert.Equal(2, cancelled.Count);
        Assert.True(bookings[0].IsActive);
        Assert.False(bookings[1].IsActive);
        Assert.False(bookings[2].IsActive);
        Assert.Equal(1, sessions[0].ActiveBookingCount);
    }

    [Fact]
    public void CancelBookingsFor_OtherUsersBookings_AreNotTouched()
    {
        // Arrange
        var (series, sessions) = CreateSeries(sessionCount: 2);
        series.Book(sessions, UserA, null, Now);
        var otherBookings = series.Book(sessions, UserB, null, Now);

        // Act
        series.CancelBookingsFor(sessions, UserA, Now);

        // Assert
        Assert.All(otherBookings, b => Assert.True(b.IsActive));
        Assert.All(sessions, s => Assert.Equal(1, s.ActiveBookingCount));
    }

    [Fact]
    public void CancelBookingsFor_NoActiveBookings_ThrowsNoActiveBookings()
    {
        // Arrange
        var (series, sessions) = CreateSeries(sessionCount: 2);

        // Act
        var exception = Record.Exception(() => series.CancelBookingsFor(sessions, UserA, Now));

        // Assert
        AssertDomainError(exception, ErrorCodes.SeriesNoActiveBookings, ErrorType.NotFound);
    }

    [Fact]
    public void Cancel_SomeSessionsStarted_CancelsFutureSessionsAndTheirBookings()
    {
        // Arrange
        var (series, sessions) = CreateSeries(sessionCount: 3);
        var bookings = series.Book(sessions, UserA, null, Now);
        var afterFirstStart = sessions[0].StartsAtUtc.AddMinutes(1);

        // Act
        var cancelledCount = series.Cancel(sessions, afterFirstStart);

        // Assert
        Assert.Equal(2, cancelledCount);
        Assert.True(series.IsCancelled);
        Assert.False(sessions[0].IsCancelled);
        Assert.True(bookings[0].IsActive);
        Assert.True(sessions[1].IsCancelled);
        Assert.True(sessions[2].IsCancelled);
        Assert.False(bookings[1].IsActive);
        Assert.False(bookings[2].IsActive);
    }

    [Fact]
    public void Cancel_AlreadyCancelled_ThrowsSeriesCancelled()
    {
        // Arrange
        var (series, sessions) = CreateSeries(sessionCount: 1);
        series.Cancel(sessions, Now);

        // Act
        var exception = Record.Exception(() => series.Cancel(sessions, Now));

        // Assert
        AssertDomainError(exception, ErrorCodes.SeriesCancelled, ErrorType.Conflict);
    }
}
