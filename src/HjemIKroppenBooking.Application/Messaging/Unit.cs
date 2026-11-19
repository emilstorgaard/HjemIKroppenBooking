namespace HjemIKroppenBooking.Application.Messaging;

/// <summary>Result type for commands that return nothing.</summary>
public readonly record struct Unit
{
    public static Unit Value => default;
}
