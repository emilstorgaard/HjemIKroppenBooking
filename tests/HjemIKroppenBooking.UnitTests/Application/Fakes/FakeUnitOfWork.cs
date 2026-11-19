using HjemIKroppenBooking.Application.Abstractions;

namespace HjemIKroppenBooking.UnitTests.Application.Fakes;

internal sealed class FakeUnitOfWork : IUnitOfWork
{
    public int SaveCount { get; private set; }

    public int DiscardCount { get; private set; }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveCount++;
        return Task.CompletedTask;
    }

    public void DiscardChanges() => DiscardCount++;
}
