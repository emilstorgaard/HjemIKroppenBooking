namespace HjemIKroppenBooking.Api.Contracts.Requests.ServiceTypes;

public sealed class ListPublicServiceTypesRequest : PagedRequest
{
    public Guid? CategoryId { get; init; }
}
