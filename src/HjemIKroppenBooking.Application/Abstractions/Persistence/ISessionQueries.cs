using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.Sessions;
using HjemIKroppenBooking.Application.Features.Sessions.Common;

namespace HjemIKroppenBooking.Application.Abstractions.Persistence;

/// <summary>Read side for sessions. Public methods only return non-cancelled sessions of active service types.</summary>
public interface ISessionQueries
{
    Task<PagedResponse<SessionResponse>> ListPublicAsync(SessionListFilter filter, int page, int pageSize, CancellationToken cancellationToken);

    Task<SessionResponse?> GetPublicByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<PagedResponse<AdminSessionResponse>> ListAsync(SessionListFilter filter, int page, int pageSize, CancellationToken cancellationToken);

    Task<AdminSessionResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
}
