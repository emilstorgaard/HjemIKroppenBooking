using HjemIKroppenBooking.Application.Features.Bookings.Common;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.Series;
using HjemIKroppenBooking.Domain.Sessions;
using HjemIKroppenBooking.UnitTests.Application.Fakes;
using Microsoft.Extensions.Time.Testing;
using static HjemIKroppenBooking.UnitTests.Domain.TestData;

namespace HjemIKroppenBooking.UnitTests.Application.Bookings;

public sealed class BookingServiceTests
{
    private static readonly Guid UserA = Guid.NewGuid();
    private static readonly Guid UserB = Guid.NewGuid();

    private readonly InMemorySessionRepository _sessions = new();
    private readonly InMemorySeriesRepository _series = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly FakeTimeProvider _time = new(new DateTimeOffset(Now));

    [Fact]
    public async Task BookSession_SpotAvailable_BooksThroughAggregateAndSaves()
    {
        // Arrange
        var session = AddSession();

        // Act
        var bookingId = await CreateCreator().BookSessionAsync(UserA, session.Id, "Note", includeInactiveServiceTypes: false, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(bookingId, Assert.Single(session.Bookings).Id);
        Assert.Equal(1, session.ActiveBookingCount);
        Assert.Equal(1, _unitOfWork.SaveCount);
    }

    [Fact]
    public async Task BookSession_UnknownSession_ThrowsSessionNotFound()
    {
        // Arrange
        var creator = CreateCreator();

        // Act
        var exception = await Record.ExceptionAsync(() => creator.BookSessionAsync(UserA, Guid.NewGuid(), null, includeInactiveServiceTypes: false, TestContext.Current.CancellationToken));

        // Assert
        AssertDomainError(exception, ErrorCodes.SessionNotFound, ErrorType.NotFound);
    }

    [Fact]
    public async Task BookSession_InactiveServiceTypeForUser_ThrowsSessionNotFound()
    {
        // Arrange
        var session = AddSession();
        session.ServiceType.Deactivate();

        // Act
        var exception = await Record.ExceptionAsync(() => CreateCreator().BookSessionAsync(UserA, session.Id, null, includeInactiveServiceTypes: false, TestContext.Current.CancellationToken));

        // Assert
        AssertDomainError(exception, ErrorCodes.SessionNotFound, ErrorType.NotFound);
        Assert.Equal(0, session.ActiveBookingCount);
    }

    [Fact]
    public async Task BookSession_InactiveServiceTypeForAdmin_Books()
    {
        // Arrange
        var session = AddSession();
        session.ServiceType.Deactivate();

        // Act
        await CreateCreator().BookSessionAsync(UserA, session.Id, null, includeInactiveServiceTypes: true, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(1, session.ActiveBookingCount);
    }

    [Fact]
    public async Task BookSession_Full_ThrowsSessionFullAndDoesNotSave()
    {
        // Arrange
        var session = AddSession(capacity: 1);
        session.Book(UserB, null, Now);

        // Act
        var exception = await Record.ExceptionAsync(() => CreateCreator().BookSessionAsync(UserA, session.Id, null, includeInactiveServiceTypes: false, TestContext.Current.CancellationToken));

        // Assert
        AssertDomainError(exception, ErrorCodes.SessionFull, ErrorType.Conflict);
        Assert.Equal(0, _unitOfWork.SaveCount);
    }

    [Fact]
    public async Task BookSeries_OneSessionFull_BooksNothing()
    {
        // Arrange
        var (series, sessions) = AddSeries(sessionCount: 3, capacity: 1);
        sessions[1].Book(UserB, null, Now);

        // Act
        var exception = await Record.ExceptionAsync(() => CreateCreator().BookSeriesAsync(UserA, series.Id, null, includeInactiveServiceTypes: false, TestContext.Current.CancellationToken));

        // Assert
        var error = AssertDomainError(exception, ErrorCodes.SessionFull, ErrorType.Conflict);
        Assert.Equal(sessions[1].Id, error.Parameters["sessionId"]);
        Assert.Equal(0, sessions[0].ActiveBookingCount);
        Assert.Equal(0, sessions[2].ActiveBookingCount);
        Assert.Equal(0, _unitOfWork.SaveCount);
    }

    [Fact]
    public async Task BookSeries_AllAvailable_BooksEverySessionInOneSave()
    {
        // Arrange
        var (series, sessions) = AddSeries(sessionCount: 3, capacity: 5);

        // Act
        var bookingIds = await CreateCreator().BookSeriesAsync(UserA, series.Id, null, includeInactiveServiceTypes: false, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(3, bookingIds.Count);
        Assert.All(sessions, s => Assert.Equal(1, s.ActiveBookingCount));
        Assert.Equal(1, _unitOfWork.SaveCount);
    }

    [Fact]
    public async Task BookSeries_InactiveServiceTypeForUser_ThrowsSeriesNotFound()
    {
        // Arrange
        var (series, _) = AddSeries(sessionCount: 1, capacity: 5);
        series.ServiceType.Deactivate();

        // Act
        var exception = await Record.ExceptionAsync(() => CreateCreator().BookSeriesAsync(UserA, series.Id, null, includeInactiveServiceTypes: false, TestContext.Current.CancellationToken));

        // Assert
        AssertDomainError(exception, ErrorCodes.SeriesNotFound, ErrorType.NotFound);
    }

    [Fact]
    public async Task Cancel_OtherUsersBooking_ThrowsBookingNotFound()
    {
        // Arrange
        var session = AddSession();
        var booking = session.Book(UserB, null, Now);

        // Act
        var exception = await Record.ExceptionAsync(() => CreateCanceller().CancelAsync(booking.Id, UserA, TestContext.Current.CancellationToken));

        // Assert
        AssertDomainError(exception, ErrorCodes.BookingNotFound, ErrorType.NotFound);
        Assert.True(booking.IsActive);
    }

    [Fact]
    public async Task Cancel_AlreadyCancelled_ThrowsAlreadyCancelled()
    {
        // Arrange
        var session = AddSession();
        var booking = session.Book(UserA, null, Now);
        session.CancelBooking(booking.Id, Now);

        // Act
        var exception = await Record.ExceptionAsync(() => CreateCanceller().CancelAsync(booking.Id, UserA, TestContext.Current.CancellationToken));

        // Assert
        AssertDomainError(exception, ErrorCodes.BookingAlreadyCancelled, ErrorType.Conflict);
    }

    [Fact]
    public async Task Cancel_JustBeforeStart_CancelsAndSaves()
    {
        // Arrange
        var session = AddSession();
        var booking = session.Book(UserA, null, Now);
        _time.SetUtcNow(new DateTimeOffset(session.StartsAtUtc.AddSeconds(-1)));

        // Act
        await CreateCanceller().CancelAsync(booking.Id, UserA, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(booking.IsActive);
        Assert.Equal(1, _unitOfWork.SaveCount);
    }

    [Fact]
    public async Task Cancel_AfterStart_ThrowsCancellationClosed()
    {
        // Arrange
        var session = AddSession();
        var booking = session.Book(UserA, null, Now);
        _time.SetUtcNow(new DateTimeOffset(session.StartsAtUtc.AddMinutes(1)));

        // Act
        var exception = await Record.ExceptionAsync(() => CreateCanceller().CancelAsync(booking.Id, ownerUserId: null, TestContext.Current.CancellationToken));

        // Assert
        AssertDomainError(exception, ErrorCodes.BookingCancellationClosed, ErrorType.Conflict);
        Assert.True(booking.IsActive);
    }

    private BookingCreator CreateCreator() => new(_sessions, _series, _unitOfWork, _time);

    private BookingCanceller CreateCanceller() => new(_sessions, _sessions, _unitOfWork, _time);

    private Session AddSession(int capacity = 5)
    {
        var session = CreateSession(capacity);
        _sessions.Add(session);
        return session;
    }

    private (SessionSeries Series, List<Session> Sessions) AddSeries(int sessionCount, int capacity)
    {
        var (series, sessions) = CreateSeries(sessionCount, capacity);
        _series.Add(series);
        sessions.ForEach(_sessions.Add);
        return (series, sessions);
    }
}
