using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.ServiceCategories.GetServiceCategoryById;

public sealed class GetServiceCategoryByIdQueryHandler(IServiceCategoryQueries categories) : IQueryHandler<GetServiceCategoryByIdQuery, AdminServiceCategoryResponse>
{
    public async Task<AdminServiceCategoryResponse> Handle(GetServiceCategoryByIdQuery query, CancellationToken cancellationToken) =>
        await categories.GetByIdAsync(query.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.ServiceCategoryNotFound);
}
