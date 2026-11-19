namespace HjemIKroppenBooking.Application.Contracts.Responses.Common;

/// <summary>One page of a larger result. <c>page</c> is 1-based.</summary>
public sealed record PagedResponse<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => TotalCount == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
