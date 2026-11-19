using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Features.Bookings.Common;

namespace HjemIKroppenBooking.Application.Abstractions.Persistence;

/// <summary>Read side for bookings.</summary>
public interface IBookingQueries
{
    Task<PagedResponse<AdminBookingResponse>> ListAsync(BookingListFilter filter, int page, int pageSize, CancellationToken cancellationToken);

    /// <summary>Same filtering as <see cref="ListAsync"/>, in the shape shown to the booking's own user.</summary>
    Task<PagedResponse<BookingResponse>> ListOwnAsync(BookingListFilter filter, int page, int pageSize, CancellationToken cancellationToken);

    Task<AdminBookingResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Returns the booking only if it belongs to the user.</summary>
    Task<BookingResponse?> GetOwnByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);

    /// <summary>The bookings ordered by session start.</summary>
    Task<IReadOnlyList<BookingResponse>> GetOwnByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken);

    /// <summary>The bookings ordered by session start.</summary>
    Task<IReadOnlyList<AdminBookingResponse>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken);
}
