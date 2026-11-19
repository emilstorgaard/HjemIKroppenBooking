using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Domain.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HjemIKroppenBooking.Application.Messaging;

/// <summary>Lets the dispatcher call a handler when only the result type is known at compile time.</summary>
internal abstract class CommandHandlerWrapper<TResult>
{
    public abstract Task<TResult> Handle(ICommand<TResult> command, IServiceProvider serviceProvider, CancellationToken cancellationToken);
}

/// <summary>
/// Pipeline for one command type: validate, run the handler, and on a persistence conflict discard the tracked
/// changes and run the handler once more so it reloads its aggregates. A second conflict becomes a
/// <see cref="ErrorType.Conflict"/> error.
/// </summary>
internal sealed class CommandHandlerWrapper<TCommand, TResult> : CommandHandlerWrapper<TResult>
    where TCommand : ICommand<TResult>
{
    public override async Task<TResult> Handle(ICommand<TResult> command, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var typedCommand = (TCommand)command;
        await RequestValidation.ValidateAsync(typedCommand, serviceProvider, cancellationToken);

        var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

        try
        {
            return await handler.Handle(typedCommand, cancellationToken);
        }
        catch (PersistenceConflictException conflict)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<Dispatcher>>();
            DispatcherLog.RetryingAfterConflict(logger, typeof(TCommand).Name, conflict.ErrorCode);
            unitOfWork.DiscardChanges();
        }

        try
        {
            return await handler.Handle(typedCommand, cancellationToken);
        }
        catch (PersistenceConflictException conflict)
        {
            unitOfWork.DiscardChanges();
            throw DomainException.Conflict(conflict.ErrorCode);
        }
    }
}
