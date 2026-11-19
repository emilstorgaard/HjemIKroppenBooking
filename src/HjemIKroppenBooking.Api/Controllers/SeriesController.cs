using Asp.Versioning;
using HjemIKroppenBooking.Api.Contracts.Requests.Series;
using HjemIKroppenBooking.Api.ErrorHandling;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Features.Series.GetPublicSeriesById;
using HjemIKroppenBooking.Application.Features.Series.ListBookableSeries;
using HjemIKroppenBooking.Application.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HjemIKroppenBooking.Api.Controllers;

/// <summary>Series of group sessions, readable without signing in. Booking a series is under <c>/me/series/{id}/bookings</c>.</summary>
[ApiVersion("1.0")]
[Route(ApiRoutes.Prefix + "/series")]
[AllowAnonymous]
public sealed class SeriesController(IDispatcher dispatcher) : ApiControllerBase(dispatcher)
{
    /// <summary>Series with at least one session that has not started.</summary>
    [HttpGet]
    [ProducesResponseType<PagedResponse<SeriesResponse>>(StatusCodes.Status200OK)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PagedResponse<SeriesResponse>>> List([FromQuery] ListBookableSeriesRequest request, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Query(new ListBookableSeriesQuery(request.ServiceTypeId, request.OnlyAvailable, request.Page, request.PageSize), cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType<SeriesResponse>(StatusCodes.Status200OK)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SeriesResponse>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Query(new GetPublicSeriesByIdQuery(id), cancellationToken));
}
