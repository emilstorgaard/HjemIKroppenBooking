using HjemIKroppenBooking.Domain.ServiceTypes;

namespace HjemIKroppenBooking.Api.Contracts.Requests.ServiceTypes;

public sealed class ListPublicServiceTypesRequest : PagedRequest
{
    public ServiceCategory? Category { get; init; }
}
