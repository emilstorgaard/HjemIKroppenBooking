using HjemIKroppenBooking.Application.Contracts.Responses.Users;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Users.GetMe;

public sealed record GetMeQuery : IQuery<UserResponse>;
