using HjemIKroppenBooking.Application.Contracts.Responses.Users;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Users.GetUserById;

public sealed record GetUserByIdQuery(Guid Id) : IQuery<AdminUserResponse>;
