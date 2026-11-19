using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.Sessions;
using HjemIKroppenBooking.Application.Features.Sessions.Common;
using HjemIKroppenBooking.Application.Features.Sessions.DeleteSession;
using HjemIKroppenBooking.Application.Features.Sessions.UpdateSession;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.UnitTests.Application.Fakes;
using Microsoft.Extensions.Time.Testing;
using static HjemIKroppenBooking.UnitTests.Domain.TestData;

namespace HjemIKroppenBooking.UnitTests.Application.Sessions;

public sealed class SessionHandlerTests
{
    private readonly InMemorySessionRepository _sessions = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly FakeTimeProvider _time = new(new DateTimeOffset(Now));

    [Fact]
    public async Task Update_CapacityBelowActiveBookings_ThrowsAndDoesNotSave()
    {
        // Arrange
        var session = CreateSession(capacity: 3);
        session.Book(Guid.NewGuid(), null, Now);
        session.Book(Guid.NewGuid(), null, Now);
        _sessions.Add(session);
        var handler = new UpdateSessionCommandHandler(_sessions, new NoSessionQueries(), _unitOfWork, _time);
        var command = new UpdateSessionCommand(session.Id, session.StartsAtUtc, session.EndsAtUtc, 1, null, null);

        // Act
        var exception = await Record.ExceptionAsync(() => handler.Handle(command, TestContext.Current.CancellationToken));

        // Assert
        AssertDomainError(exception, ErrorCodes.SessionCapacityBelowActiveBookings, ErrorType.Conflict);
        Assert.Equal(3, session.Capacity);
        Assert.Equal(0, _unitOfWork.SaveCount);
    }

    [Fact]
    public async Task Delete_WithActiveBookings_ThrowsHasActiveBookingsAndKeepsSession()
    {
        // Arrange
        var session = CreateSession();
        session.Book(Guid.NewGuid(), null, Now);
        _sessions.Add(session);
        var handler = new DeleteSessionCommandHandler(_sessions, _unitOfWork);

        // Act
        var exception = await Record.ExceptionAsync(() => handler.Handle(new DeleteSessionCommand(session.Id), TestContext.Current.CancellationToken));

        // Assert
        AssertDomainError(exception, ErrorCodes.SessionHasActiveBookings, ErrorType.Conflict);
        Assert.Empty(_sessions.Removed);
    }

    [Fact]
    public async Task Delete_OnlyCancelledBookings_RemovesSession()
    {
        // Arrange
        var session = CreateSession();
        var booking = session.Book(Guid.NewGuid(), null, Now);
        session.CancelBooking(booking.Id, Now);
        _sessions.Add(session);
        var handler = new DeleteSessionCommandHandler(_sessions, _unitOfWork);

        // Act
        await handler.Handle(new DeleteSessionCommand(session.Id), TestContext.Current.CancellationToken);

        // Assert
        Assert.Same(session, Assert.Single(_sessions.Removed));
        Assert.Equal(1, _unitOfWork.SaveCount);
    }

    [Fact]
    public async Task Delete_UnknownSession_ThrowsSessionNotFound()
    {
        // Arrange
        var handler = new DeleteSessionCommandHandler(_sessions, _unitOfWork);

        // Act
        var exception = await Record.ExceptionAsync(() => handler.Handle(new DeleteSessionCommand(Guid.NewGuid()), TestContext.Current.CancellationToken));

        // Assert
        AssertDomainError(exception, ErrorCodes.SessionNotFound, ErrorType.NotFound);
    }

    /// <summary>The tested paths fail before reading back, so the read side is never called.</summary>
    private sealed class NoSessionQueries : ISessionQueries
    {
        public Task<PagedResponse<SessionResponse>> ListPublicAsync(SessionListFilter filter, int page, int pageSize, CancellationToken cancellationToken) => throw new NotSupportedException();

        public Task<SessionResponse?> GetPublicByIdAsync(Guid id, CancellationToken cancellationToken) => throw new NotSupportedException();

        public Task<PagedResponse<AdminSessionResponse>> ListAsync(SessionListFilter filter, int page, int pageSize, CancellationToken cancellationToken) => throw new NotSupportedException();

        public Task<AdminSessionResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken) => throw new NotSupportedException();

        public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken) => throw new NotSupportedException();
    }
}
