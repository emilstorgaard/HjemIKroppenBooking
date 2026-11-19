using HjemIKroppenBooking.Domain.ServiceTypes;

namespace HjemIKroppenBooking.Api.Contracts.Requests.ServiceTypes;

public sealed class ListServiceTypesRequest : PagedRequest
{
    public ServiceCategory? Category { get; init; }

    public bool? IsActive { get; init; }
}
