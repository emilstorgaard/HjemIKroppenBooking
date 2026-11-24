using HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;
using HjemIKroppenBooking.Domain.ServiceTypes;

namespace HjemIKroppenBooking.Application.Common.Mapping;

public static class ServiceTypeMapping
{
    public static AdminServiceTypeResponse ToAdminResponse(this ServiceType serviceType) => new(
        serviceType.Id,
        serviceType.Category.ToSummary(),
        serviceType.Name,
        serviceType.Description,
        serviceType.DefaultDurationMinutes,
        serviceType.DefaultCapacity,
        serviceType.IsActive,
        serviceType.CreatedAtUtc,
        serviceType.UpdatedAtUtc);
}
