namespace HjemIKroppenBooking.Application.Messaging;

/// <summary>A request that changes state. <typeparamref name="TResult"/> is what the handler returns.</summary>
public interface ICommand<TResult>;
