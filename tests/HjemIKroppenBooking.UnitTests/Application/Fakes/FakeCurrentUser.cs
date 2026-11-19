using HjemIKroppenBooking.Application.Abstractions;

namespace HjemIKroppenBooking.UnitTests.Application.Fakes;

internal sealed class FakeCurrentUser(Guid userId) : ICurrentUser
{
    public Guid UserId { get; } = userId;
}
