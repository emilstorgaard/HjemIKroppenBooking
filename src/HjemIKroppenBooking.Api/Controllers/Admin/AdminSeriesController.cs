using Asp.Versioning;
using HjemIKroppenBooking.Api.Contracts.Requests.Series;
using HjemIKroppenBooking.Api.ErrorHandling;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Features.Series.CancelSeries;
using HjemIKroppenBooking.Application.Features.Series.CancelSeriesBookingForUser;
using HjemIKroppenBooking.Application.Features.Series.CreateSeries;
using HjemIKroppenBooking.Application.Features.Series.CreateSeriesBookingForUser;
using HjemIKroppenBooking.Application.Features.Series.GetSeriesById;
using HjemIKroppenBooking.Application.Features.Series.ListSeries;
using HjemIKroppenBooking.Application.Features.Series.ListSeriesParticipants;
using HjemIKroppenBooking.Application.Features.Series.UpdateSeries;
using HjemIKroppenBooking.Application.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace HjemIKroppenBooking.Api.Controllers.Admin;

[ApiVersion("1.0")]
[Route(AdminControllerBase.RoutePrefix + "/series")]
public sealed class AdminSeriesController(IDispatcher dispatcher) : AdminControllerBase(dispatcher)
{
    [HttpGet]
    [ProducesResponseType<PagedResponse<AdminSeriesListItemResponse>>(StatusCodes.Status200OK)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PagedResponse<AdminSeriesListItemResponse>>> List([FromQuery] ListSeriesRequest request, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Query(new ListSeriesQuery(request.Search, request.ServiceTypeId, request.Upcoming, request.IsCancelled, request.Page, request.PageSize), cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType<AdminSeriesResponse>(StatusCodes.Status200OK)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminSeriesResponse>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Query(new GetSeriesByIdQuery(id), cancellationToken));

    /// <summary>Creates a series of a service type with one session per start time.</summary>
    [HttpPost]
    [ProducesResponseType<AdminSeriesResponse>(StatusCodes.Status201Created)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    [ProducesProblem(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdminSeriesResponse>> Create(CreateSeriesRequest request, CancellationToken cancellationToken)
    {
        var startTimes = (request.StartTimes ?? []).Select(t => t.UtcDateTime).ToList();
        var command = new CreateSeriesCommand(request.ServiceTypeId, request.Title, startTimes, request.DurationMinutes, request.Capacity, request.Location, request.Notes);
        var series = await Dispatcher.Send(command, cancellationToken);
        return Created(ApiRoutes.V1Path($"/admin/series/{series.Id}"), series);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<AdminSeriesResponse>(StatusCodes.Status200OK)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminSeriesResponse>> Update(Guid id, UpdateSeriesRequest request, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Send(new UpdateSeriesCommand(id, request.Title), cancellationToken));

    /// <summary>Cancels the series, its sessions that have not started and their bookings.</summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType<AdminSeriesResponse>(StatusCodes.Status200OK)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    [ProducesProblem(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdminSeriesResponse>> Cancel(Guid id, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Send(new CancelSeriesCommand(id), cancellationToken));

    [HttpGet("{id:guid}/participants")]
    [ProducesResponseType<PagedResponse<SeriesParticipantResponse>>(StatusCodes.Status200OK)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResponse<SeriesParticipantResponse>>> ListParticipants(
        Guid id, [FromQuery] ListSeriesParticipantsRequest request, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Query(new ListSeriesParticipantsQuery(id, request.IncludeCancelled, request.Page, request.PageSize), cancellationToken));

    /// <summary>Books a user on every session of the series that has not started, all or nothing.</summary>
    [HttpPost("{id:guid}/bookings")]
    [ProducesResponseType<AdminSeriesBookingResponse>(StatusCodes.Status201Created)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    [ProducesProblem(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdminSeriesBookingResponse>> BookUser(Guid id, AdminCreateSeriesBookingRequest request, CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(new CreateSeriesBookingForUserCommand(id, request.UserId, request.Note), cancellationToken);
        return Created(ApiRoutes.V1Path($"/admin/bookings?seriesId={id}&userId={request.UserId}"), result);
    }

    /// <summary>Cancels the user's bookings on the series' sessions that have not started.</summary>
    [HttpDelete("{id:guid}/bookings/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelUserBookings(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        await Dispatcher.Send(new CancelSeriesBookingForUserCommand(id, userId), cancellationToken);
        return NoContent();
    }
}
