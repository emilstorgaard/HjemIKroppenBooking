using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.ServiceCategories.GetServiceCategoryById;

public sealed record GetServiceCategoryByIdQuery(Guid Id) : IQuery<AdminServiceCategoryResponse>;
