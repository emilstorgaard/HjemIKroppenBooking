namespace HjemIKroppenBooking.Api.Contracts.Requests.ServiceTypes;

public sealed class ListServiceTypesRequest : PagedRequest
{
    public Guid? CategoryId { get; init; }

    public bool? IsActive { get; init; }
}
