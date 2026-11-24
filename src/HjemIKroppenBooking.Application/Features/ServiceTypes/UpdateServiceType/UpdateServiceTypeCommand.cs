using HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;
using HjemIKroppenBooking.Application.Features.ServiceTypes.Common;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.UpdateServiceType;

public sealed record UpdateServiceTypeCommand(
    Guid Id, Guid CategoryId, string Name, string? Description, int DefaultDurationMinutes, int DefaultCapacity, int SortOrder, bool IsActive)
    : ICommand<AdminServiceTypeResponse>, IServiceTypeFields;
