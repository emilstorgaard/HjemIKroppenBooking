namespace HjemIKroppenBooking.Application.Common.Paging;

/// <summary>Offset paging: <see cref="Page"/> is 1-based.</summary>
public interface IPagedQuery
{
    int Page { get; }

    int PageSize { get; }
}
