using Asp.Versioning;
using HjemIKroppenBooking.Api.Contracts.Requests.Sessions;
using HjemIKroppenBooking.Api.ErrorHandling;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.Sessions;
using HjemIKroppenBooking.Application.Features.Sessions.GetPublicSessionById;
using HjemIKroppenBooking.Application.Features.Sessions.ListPublicSessions;
using HjemIKroppenBooking.Application.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HjemIKroppenBooking.Api.Controllers;

/// <summary>Bookable sessions, readable without signing in.</summary>
[ApiVersion("1.0")]
[Route(ApiRoutes.Prefix + "/sessions")]
[AllowAnonymous]
public sealed class SessionsController(IDispatcher dispatcher) : ApiControllerBase(dispatcher)
{
    /// <summary>Sessions that have not started, are not cancelled and belong to an active service type.</summary>
    [HttpGet]
    [ProducesResponseType<PagedResponse<SessionResponse>>(StatusCodes.Status200OK)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PagedResponse<SessionResponse>>> List([FromQuery] ListPublicSessionsRequest request, CancellationToken cancellationToken)
    {
        var query = new ListPublicSessionsQuery(
            request.ServiceTypeId, request.Category, request.From?.UtcDateTime, request.To?.UtcDateTime, request.OnlyAvailable, request.Page, request.PageSize);
        return Ok(await Dispatcher.Query(query, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<SessionResponse>(StatusCodes.Status200OK)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SessionResponse>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Query(new GetPublicSessionByIdQuery(id), cancellationToken));
}
