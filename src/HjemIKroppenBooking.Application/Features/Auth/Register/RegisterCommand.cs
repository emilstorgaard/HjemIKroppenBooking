using HjemIKroppenBooking.Application.Contracts.Responses.Users;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Auth.Register;

public sealed record RegisterCommand(string Email, string FullName, string Password) : ICommand<UserResponse>;
