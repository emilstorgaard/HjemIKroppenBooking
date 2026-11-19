using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Users;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Users.GetUserById;

public sealed class GetUserByIdQueryHandler(IUserQueries users) : IQueryHandler<GetUserByIdQuery, AdminUserResponse>
{
    public async Task<AdminUserResponse> Handle(GetUserByIdQuery query, CancellationToken cancellationToken) =>
        await users.GetByIdAsync(query.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.UserNotFound);
}
