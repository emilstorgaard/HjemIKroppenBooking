using Microsoft.Extensions.DependencyInjection;

namespace HjemIKroppenBooking.Application.Messaging;

internal abstract class QueryHandlerWrapper<TResult>
{
    public abstract Task<TResult> Handle(IQuery<TResult> query, IServiceProvider serviceProvider, CancellationToken cancellationToken);
}

/// <summary>Pipeline for one query type: validate, then run the handler.</summary>
internal sealed class QueryHandlerWrapper<TQuery, TResult> : QueryHandlerWrapper<TResult>
    where TQuery : IQuery<TResult>
{
    public override async Task<TResult> Handle(IQuery<TResult> query, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var typedQuery = (TQuery)query;
        await RequestValidation.ValidateAsync(typedQuery, serviceProvider, cancellationToken);
        var handler = serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
        return await handler.Handle(typedQuery, cancellationToken);
    }
}
