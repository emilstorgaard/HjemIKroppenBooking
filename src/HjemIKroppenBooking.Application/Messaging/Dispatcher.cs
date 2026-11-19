using System.Collections.Concurrent;

namespace HjemIKroppenBooking.Application.Messaging;

internal sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    // Static, so one wrapper per request type is created for the lifetime of the app.
    private static readonly ConcurrentDictionary<Type, object> CommandWrappers = new();
    private static readonly ConcurrentDictionary<Type, object> QueryWrappers = new();

    public Task<TResult> Send<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        var wrapper = (CommandHandlerWrapper<TResult>)CommandWrappers.GetOrAdd(
            command.GetType(), static type => Activator.CreateInstance(typeof(CommandHandlerWrapper<,>).MakeGenericType(type, typeof(TResult)))!);
        return wrapper.Handle(command, serviceProvider, cancellationToken);
    }

    public Task<TResult> Query<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        var wrapper = (QueryHandlerWrapper<TResult>)QueryWrappers.GetOrAdd(
            query.GetType(), static type => Activator.CreateInstance(typeof(QueryHandlerWrapper<,>).MakeGenericType(type, typeof(TResult)))!);
        return wrapper.Handle(query, serviceProvider, cancellationToken);
    }
}
