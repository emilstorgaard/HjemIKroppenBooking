namespace HjemIKroppenBooking.Application.Messaging;

/// <summary>
/// Sends commands and queries to their handlers. Every request is validated first. Commands are retried once
/// after a persistence conflict.
/// </summary>
public interface IDispatcher
{
    Task<TResult> Send<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);

    Task<TResult> Query<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}
