using Asp.Versioning;
using HjemIKroppenBooking.Api.Contracts.Requests.ServiceTypes;
using HjemIKroppenBooking.Api.ErrorHandling;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;
using HjemIKroppenBooking.Application.Features.ServiceTypes.GetPublicServiceTypeById;
using HjemIKroppenBooking.Application.Features.ServiceTypes.ListPublicServiceTypes;
using HjemIKroppenBooking.Application.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HjemIKroppenBooking.Api.Controllers;

/// <summary>Active service types, readable without signing in.</summary>
[ApiVersion("1.0")]
[Route(ApiRoutes.Prefix + "/service-types")]
[AllowAnonymous]
public sealed class ServiceTypesController(IDispatcher dispatcher) : ApiControllerBase(dispatcher)
{
    [HttpGet]
    [ProducesResponseType<PagedResponse<ServiceTypeResponse>>(StatusCodes.Status200OK)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PagedResponse<ServiceTypeResponse>>> List([FromQuery] ListPublicServiceTypesRequest request, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Query(new ListPublicServiceTypesQuery(request.Category, request.Page, request.PageSize), cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType<ServiceTypeResponse>(StatusCodes.Status200OK)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceTypeResponse>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Query(new GetPublicServiceTypeByIdQuery(id), cancellationToken));
}
