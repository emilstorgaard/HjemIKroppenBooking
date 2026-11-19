using HjemIKroppenBooking.Application.Contracts.Responses.Sessions;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Sessions.CancelSession;

/// <summary>Cancels a session that has not started, together with all its active bookings.</summary>
public sealed record CancelSessionCommand(Guid Id) : ICommand<AdminSessionResponse>;
