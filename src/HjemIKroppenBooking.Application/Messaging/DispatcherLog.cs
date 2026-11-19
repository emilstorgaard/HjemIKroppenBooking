using Microsoft.Extensions.Logging;

namespace HjemIKroppenBooking.Application.Messaging;

internal static partial class DispatcherLog
{
    [LoggerMessage(EventId = 1000, Level = LogLevel.Information, Message = "Retrying {CommandName} once after persistence conflict {ErrorCode}")]
    public static partial void RetryingAfterConflict(ILogger logger, string commandName, string errorCode);
}
