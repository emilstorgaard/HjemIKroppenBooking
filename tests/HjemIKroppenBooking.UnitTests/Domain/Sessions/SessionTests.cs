using HjemIKroppenBooking.Domain.Bookings;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.Sessions;
using static HjemIKroppenBooking.UnitTests.Domain.TestData;

namespace HjemIKroppenBooking.UnitTests.Domain.Sessions;

public sealed class SessionTests
{
    private static readonly Guid UserA = Guid.NewGuid();
    private static readonly Guid UserB = Guid.NewGuid();

    [Fact]
    public void Schedule_ValidInput_CreatesSessionWithoutBookings()
    {
        // Arrange
        var start = Now.AddDays(2);

        // Act
        var session = Session.Schedule(GroupServiceType(), null, start, start.AddMinutes(90), 8, "  Studio 1  ", "  ", AdminId, Now);

        // Assert
        Assert.Equal(8, session.Capacity);
        Assert.Equal(0, session.ActiveBookingCount);
        Assert.Equal(8, session.AvailableSpots);
        Assert.Equal("Studio 1", session.Location);
        Assert.Null(session.Notes);
        Assert.False(session.IsCancelled);
    }

    [Fact]
    public void Schedule_StartNotInFuture_ThrowsStartInPast()
    {
        // Arrange
        var serviceType = GroupServiceType();

        // Act
        var exception = Record.Exception(() => Session.Schedule(serviceType, null, Now, Now.AddHours(1), 5, null, null, AdminId, Now));

        // Assert
        AssertDomainError(exception, ErrorCodes.SessionStartInPast, ErrorType.Validation);
    }

    [Fact]
    public void Schedule_EndBeforeStart_ThrowsEndBeforeStart()
    {
        // Arrange
        var start = Now.AddDays(1);

        // Act
        var exception = Record.Exception(() => Session.Schedule(GroupServiceType(), null, start, start.AddMinutes(-30), 5, null, null, AdminId, Now));

        // Assert
        AssertDomainError(exception, ErrorCodes.ValidationEndBeforeStart, ErrorType.Validation);
    }

    [Theory]
    [InlineData(Session.MinDurationMinutes - 1)]
    [InlineData(Session.MaxDurationMinutes + 1)]
    public void Schedule_DurationOutOfRange_ThrowsDurationOutOfRange(int durationMinutes)
    {
        // Arrange
        var start = Now.AddDays(1);

        // Act
        var exception = Record.Exception(() => Session.Schedule(GroupServiceType(), null, start, start.AddMinutes(durationMinutes), 5, null, null, AdminId, Now));

        // Assert
        AssertDomainError(exception, ErrorCodes.ValidationDurationOutOfRange, ErrorType.Validation);
    }

    [Theory]
    [InlineData(Session.MinCapacity - 1)]
    [InlineData(Session.MaxCapacity + 1)]
    public void Schedule_CapacityOutOfRange_ThrowsCapacityOutOfRange(int capacity)
    {
        // Arrange
        var serviceType = GroupServiceType();

        // Act
        var exception = Record.Exception(() => CreateSession(capacity, serviceType: serviceType));

        // Assert
        AssertDomainError(exception, ErrorCodes.ValidationCapacityOutOfRange, ErrorType.Validation);
    }

    [Fact]
    public void Schedule_InactiveServiceType_ThrowsServiceTypeInactive()
    {
        // Arrange
        var serviceType = GroupServiceType(isActive: false);

        // Act
        var exception = Record.Exception(() => CreateSession(serviceType: serviceType));

        // Assert
        AssertDomainError(exception, ErrorCodes.ServiceTypeInactive, ErrorType.Conflict);
    }

    [Fact]
    public void Schedule_SeriesOfOtherServiceType_ThrowsServiceTypeMismatch()
    {
        // Arrange
        var (series, _) = CreateSeries(sessionCount: 1);
        var otherServiceType = GroupServiceType();

        // Act
        var exception = Record.Exception(() => CreateSession(serviceType: otherServiceType, series: series));

        // Assert
        AssertDomainError(exception, ErrorCodes.SessionServiceTypeMismatch, ErrorType.Validation);
    }

    [Fact]
    public void Schedule_LocalTime_ThrowsArgumentException()
    {
        // Arrange
        var localStart = DateTime.SpecifyKind(Now.AddDays(1), DateTimeKind.Local);

        // Act
        var exception = Record.Exception(() => Session.Schedule(GroupServiceType(), null, localStart, localStart.AddHours(1), 5, null, null, AdminId, Now));

        // Assert
        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void Schedule_SameTimeAsExistingSession_IsAllowed()
    {
        // Arrange
        var start = Now.AddDays(3);
        var existing = CreateSession(startsAt: start);

        // Act
        var overlapping = CreateSession(startsAt: start.AddMinutes(30));

        // Assert
        Assert.NotEqual(existing.Id, overlapping.Id);
        Assert.True(overlapping.StartsAtUtc < existing.EndsAtUtc);
    }

    [Fact]
    public void Book_SpotAvailable_AddsActiveBookingAndIncrementsCount()
    {
        // Arrange
        var session = CreateSession(capacity: 2);

        // Act
        var booking = session.Book(UserA, "  Looking forward  ", Now);

        // Assert
        Assert.True(booking.IsActive);
        Assert.Equal(session.Id, booking.SessionId);
        Assert.Equal(UserA, booking.UserId);
        Assert.Equal("Looking forward", booking.Note);
        Assert.Equal(1, session.ActiveBookingCount);
        Assert.Contains(booking, session.Bookings);
    }

    [Fact]
    public void Book_LastSpot_SucceedsAndSessionIsFull()
    {
        // Arrange
        var session = CreateSession(capacity: 2);
        session.Book(UserA, null, Now);

        // Act
        session.Book(UserB, null, Now);

        // Assert
        Assert.True(session.IsFull);
        Assert.Equal(0, session.AvailableSpots);
    }

    [Fact]
    public void Book_SessionFull_ThrowsSessionFullAndLeavesCountUnchanged()
    {
        // Arrange
        var session = CreateSession(capacity: 1);
        session.Book(UserA, null, Now);

        // Act
        var exception = Record.Exception(() => session.Book(UserB, null, Now));

        // Assert
        var error = AssertDomainError(exception, ErrorCodes.SessionFull, ErrorType.Conflict);
        Assert.Equal(session.Id, error.Parameters["sessionId"]);
        Assert.Equal(1, session.ActiveBookingCount);
        Assert.Single(session.Bookings);
    }

    [Fact]
    public void Book_SameUserTwice_ThrowsBookingAlreadyExists()
    {
        // Arrange
        var session = CreateSession(capacity: 5);
        session.Book(UserA, null, Now);

        // Act
        var exception = Record.Exception(() => session.Book(UserA, null, Now));

        // Assert
        AssertDomainError(exception, ErrorCodes.BookingAlreadyExists, ErrorType.Conflict);
        Assert.Equal(1, session.ActiveBookingCount);
    }

    [Fact]
    public void Book_AfterCancellingPreviousBooking_Succeeds()
    {
        // Arrange
        var session = CreateSession(capacity: 1);
        var first = session.Book(UserA, null, Now);
        session.CancelBooking(first.Id, Now);

        // Act
        var second = session.Book(UserA, null, Now);

        // Assert
        Assert.True(second.IsActive);
        Assert.Equal(1, session.ActiveBookingCount);
        Assert.Equal(2, session.Bookings.Count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void Book_AtOrAfterStart_ThrowsAlreadyStarted(int minutesAfterStart)
    {
        // Arrange
        var session = CreateSession();
        var now = session.StartsAtUtc.AddMinutes(minutesAfterStart);

        // Act
        var exception = Record.Exception(() => session.Book(UserA, null, now));

        // Assert
        AssertDomainError(exception, ErrorCodes.SessionAlreadyStarted, ErrorType.Conflict);
    }

    [Fact]
    public void Book_CancelledSession_ThrowsSessionCancelled()
    {
        // Arrange
        var session = CreateSession();
        session.Cancel(Now);

        // Act
        var exception = Record.Exception(() => session.Book(UserA, null, Now));

        // Assert
        AssertDomainError(exception, ErrorCodes.SessionCancelled, ErrorType.Conflict);
    }

    [Fact]
    public void Book_OverlappingSessionsBySameUser_BothSucceed()
    {
        // Arrange
        var start = Now.AddDays(1);
        var first = CreateSession(startsAt: start, durationMinutes: 120);
        var second = CreateSession(startsAt: start.AddMinutes(30), durationMinutes: 60);

        // Act
        var firstBooking = first.Book(UserA, null, Now);
        var secondBooking = second.Book(UserA, null, Now);

        // Assert
        Assert.True(firstBooking.IsActive);
        Assert.True(secondBooking.IsActive);
    }

    [Fact]
    public void Book_NoteTooLong_ThrowsMaxLengthAndLeavesCountUnchanged()
    {
        // Arrange
        var session = CreateSession();
        var note = new string('x', Booking.NoteMaxLength + 1);

        // Act
        var exception = Record.Exception(() => session.Book(UserA, note, Now));

        // Assert
        AssertDomainError(exception, ErrorCodes.ValidationMaxLength, ErrorType.Validation);
        Assert.Equal(0, session.ActiveBookingCount);
        Assert.Empty(session.Bookings);
    }

    [Fact]
    public void CancelBooking_BeforeStart_CancelsBookingAndFreesSpot()
    {
        // Arrange
        var session = CreateSession(capacity: 1);
        var booking = session.Book(UserA, null, Now);

        // Act
        session.CancelBooking(booking.Id, Now);

        // Assert
        Assert.Equal(BookingStatus.Cancelled, booking.Status);
        Assert.Equal(Now, booking.CancelledAtUtc);
        Assert.Equal(0, session.ActiveBookingCount);
        Assert.False(session.IsFull);
    }

    [Fact]
    public void CancelBooking_JustBeforeStart_Succeeds()
    {
        // Arrange
        var session = CreateSession();
        var booking = session.Book(UserA, null, Now);
        var justBeforeStart = session.StartsAtUtc.AddTicks(-1);

        // Act
        session.CancelBooking(booking.Id, justBeforeStart);

        // Assert
        Assert.False(booking.IsActive);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    public void CancelBooking_AtOrAfterStart_ThrowsCancellationClosed(int minutesAfterStart)
    {
        // Arrange
        var session = CreateSession();
        var booking = session.Book(UserA, null, Now);

        // Act
        var exception = Record.Exception(() => session.CancelBooking(booking.Id, session.StartsAtUtc.AddMinutes(minutesAfterStart)));

        // Assert
        AssertDomainError(exception, ErrorCodes.BookingCancellationClosed, ErrorType.Conflict);
        Assert.True(booking.IsActive);
        Assert.Equal(1, session.ActiveBookingCount);
    }

    [Fact]
    public void CancelBooking_AlreadyCancelled_ThrowsAlreadyCancelled()
    {
        // Arrange
        var session = CreateSession();
        var booking = session.Book(UserA, null, Now);
        session.CancelBooking(booking.Id, Now);

        // Act
        var exception = Record.Exception(() => session.CancelBooking(booking.Id, Now));

        // Assert
        AssertDomainError(exception, ErrorCodes.BookingAlreadyCancelled, ErrorType.Conflict);
        Assert.Equal(0, session.ActiveBookingCount);
    }

    [Fact]
    public void CancelBooking_UnknownBooking_ThrowsBookingNotFound()
    {
        // Arrange
        var session = CreateSession();

        // Act
        var exception = Record.Exception(() => session.CancelBooking(Guid.NewGuid(), Now));

        // Assert
        AssertDomainError(exception, ErrorCodes.BookingNotFound, ErrorType.NotFound);
    }

    [Fact]
    public void Update_CapacityBelowActiveBookings_ThrowsAndKeepsCapacity()
    {
        // Arrange
        var session = CreateSession(capacity: 3);
        session.Book(UserA, null, Now);
        session.Book(UserB, null, Now);

        // Act
        var exception = Record.Exception(() => session.Update(session.StartsAtUtc, session.EndsAtUtc, 1, null, null, Now));

        // Assert
        var error = AssertDomainError(exception, ErrorCodes.SessionCapacityBelowActiveBookings, ErrorType.Conflict);
        Assert.Equal(2, error.Parameters["activeBookingCount"]);
        Assert.Equal(3, session.Capacity);
    }

    [Fact]
    public void Update_CapacityEqualToActiveBookings_Succeeds()
    {
        // Arrange
        var session = CreateSession(capacity: 3);
        session.Book(UserA, null, Now);
        session.Book(UserB, null, Now);

        // Act
        session.Update(session.StartsAtUtc, session.EndsAtUtc, 2, "Hall", "Bring water", Now);

        // Assert
        Assert.Equal(2, session.Capacity);
        Assert.True(session.IsFull);
        Assert.Equal("Hall", session.Location);
        Assert.Equal("Bring water", session.Notes);
    }

    [Fact]
    public void Update_CancelledSession_ThrowsSessionCancelled()
    {
        // Arrange
        var session = CreateSession();
        session.Cancel(Now);

        // Act
        var exception = Record.Exception(() => session.Update(session.StartsAtUtc, session.EndsAtUtc, 5, null, null, Now));

        // Assert
        AssertDomainError(exception, ErrorCodes.SessionCancelled, ErrorType.Conflict);
    }

    [Fact]
    public void Update_AfterEnd_ThrowsAlreadyEnded()
    {
        // Arrange
        var session = CreateSession();
        var afterEnd = session.EndsAtUtc.AddMinutes(1);

        // Act
        var exception = Record.Exception(() => session.Update(session.StartsAtUtc, session.EndsAtUtc, 5, null, null, afterEnd));

        // Assert
        AssertDomainError(exception, ErrorCodes.SessionAlreadyEnded, ErrorType.Conflict);
    }

    [Fact]
    public void Update_MoveStartIntoPast_ThrowsStartInPast()
    {
        // Arrange
        var session = CreateSession();
        var pastStart = Now.AddHours(-1);

        // Act
        var exception = Record.Exception(() => session.Update(pastStart, pastStart.AddHours(1), 5, null, null, Now));

        // Assert
        AssertDomainError(exception, ErrorCodes.SessionStartInPast, ErrorType.Validation);
    }

    [Fact]
    public void Update_InProgressWithUnchangedStart_UpdatesDetails()
    {
        // Arrange
        var session = CreateSession();
        var duringSession = session.StartsAtUtc.AddMinutes(10);

        // Act
        session.Update(session.StartsAtUtc, session.EndsAtUtc, 5, "Moved to hall B", null, duringSession);

        // Assert
        Assert.Equal("Moved to hall B", session.Location);
    }

    [Fact]
    public void Cancel_WithActiveBookings_CancelsSessionAndAllBookings()
    {
        // Arrange
        var session = CreateSession(capacity: 3);
        var first = session.Book(UserA, null, Now);
        var second = session.Book(UserB, null, Now);

        // Act
        session.Cancel(Now);

        // Assert
        Assert.True(session.IsCancelled);
        Assert.Equal(Now, session.CancelledAtUtc);
        Assert.Equal(0, session.ActiveBookingCount);
        Assert.False(first.IsActive);
        Assert.False(second.IsActive);
    }

    [Fact]
    public void Cancel_AfterStart_ThrowsAlreadyStarted()
    {
        // Arrange
        var session = CreateSession();

        // Act
        var exception = Record.Exception(() => session.Cancel(session.StartsAtUtc));

        // Assert
        AssertDomainError(exception, ErrorCodes.SessionAlreadyStarted, ErrorType.Conflict);
        Assert.False(session.IsCancelled);
    }

    [Fact]
    public void Cancel_AlreadyCancelled_ThrowsSessionCancelled()
    {
        // Arrange
        var session = CreateSession();
        session.Cancel(Now);

        // Act
        var exception = Record.Exception(() => session.Cancel(Now));

        // Assert
        AssertDomainError(exception, ErrorCodes.SessionCancelled, ErrorType.Conflict);
    }

    [Fact]
    public void EnsureCanBeDeleted_WithActiveBookings_ThrowsHasActiveBookings()
    {
        // Arrange
        var session = CreateSession();
        session.Book(UserA, null, Now);

        // Act
        var exception = Record.Exception(session.EnsureCanBeDeleted);

        // Assert
        AssertDomainError(exception, ErrorCodes.SessionHasActiveBookings, ErrorType.Conflict);
    }

    [Fact]
    public void EnsureCanBeDeleted_OnlyCancelledBookings_DoesNotThrow()
    {
        // Arrange
        var session = CreateSession();
        var booking = session.Book(UserA, null, Now);
        session.CancelBooking(booking.Id, Now);

        // Act
        var exception = Record.Exception(session.EnsureCanBeDeleted);

        // Assert
        Assert.Null(exception);
    }
}
