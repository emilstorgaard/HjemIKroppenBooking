using HjemIKroppenBooking.Application.Contracts.Responses.Users;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Users.UpdateMe;

public sealed record UpdateMeCommand(string FullName, string Email) : ICommand<UserResponse>;
