using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Users;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Users.GetMe;

public sealed class GetMeQueryHandler(ICurrentUser currentUser, IUserQueries users) : IQueryHandler<GetMeQuery, UserResponse>
{
    public async Task<UserResponse> Handle(GetMeQuery query, CancellationToken cancellationToken) =>
        await users.GetProfileAsync(currentUser.UserId, cancellationToken) ?? throw DomainException.Unauthorized(ErrorCodes.AuthUnauthorized);
}
