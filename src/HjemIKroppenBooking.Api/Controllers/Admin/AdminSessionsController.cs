using Asp.Versioning;
using HjemIKroppenBooking.Api.Contracts.Requests.Sessions;
using HjemIKroppenBooking.Api.ErrorHandling;
using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.Sessions;
using HjemIKroppenBooking.Application.Features.Bookings.ListSessionBookings;
using HjemIKroppenBooking.Application.Features.Sessions.CancelSession;
using HjemIKroppenBooking.Application.Features.Sessions.CreateSession;
using HjemIKroppenBooking.Application.Features.Sessions.DeleteSession;
using HjemIKroppenBooking.Application.Features.Sessions.GetSessionById;
using HjemIKroppenBooking.Application.Features.Sessions.ListSessions;
using HjemIKroppenBooking.Application.Features.Sessions.UpdateSession;
using HjemIKroppenBooking.Application.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace HjemIKroppenBooking.Api.Controllers.Admin;

[ApiVersion("1.0")]
[Route(AdminControllerBase.RoutePrefix + "/sessions")]
public sealed class AdminSessionsController(IDispatcher dispatcher) : AdminControllerBase(dispatcher)
{
    [HttpGet]
    [ProducesResponseType<PagedResponse<AdminSessionResponse>>(StatusCodes.Status200OK)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PagedResponse<AdminSessionResponse>>> List([FromQuery] ListSessionsRequest request, CancellationToken cancellationToken)
    {
        var query = new ListSessionsQuery(
            request.ServiceTypeId, request.Category, request.SeriesId, request.IsCancelled, request.Upcoming,
            request.From?.UtcDateTime, request.To?.UtcDateTime, request.OnlyAvailable, request.Page, request.PageSize);
        return Ok(await Dispatcher.Query(query, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<AdminSessionResponse>(StatusCodes.Status200OK)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminSessionResponse>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Query(new GetSessionByIdQuery(id), cancellationToken));

    /// <summary>Creates a session. Sessions may overlap other sessions.</summary>
    [HttpPost]
    [ProducesResponseType<AdminSessionResponse>(StatusCodes.Status201Created)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    [ProducesProblem(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdminSessionResponse>> Create(CreateSessionRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateSessionCommand(
            request.ServiceTypeId, request.SeriesId, request.StartsAt.UtcDateTime, request.DurationMinutes, request.Capacity, request.Location, request.Notes);
        var session = await Dispatcher.Send(command, cancellationToken);
        return Created(ApiRoutes.V1Path($"/admin/sessions/{session.Id}"), session);
    }

    /// <summary>Changes time, capacity and texts. Capacity cannot go below the number of active bookings.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType<AdminSessionResponse>(StatusCodes.Status200OK)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    [ProducesProblem(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdminSessionResponse>> Update(Guid id, UpdateSessionRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateSessionCommand(id, request.StartsAt.UtcDateTime, request.EndsAt.UtcDateTime, request.Capacity, request.Location, request.Notes);
        return Ok(await Dispatcher.Send(command, cancellationToken));
    }

    /// <summary>Deletes a session without active bookings. Cancel it instead to keep the history.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    [ProducesProblem(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Dispatcher.Send(new DeleteSessionCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>Cancels the session and all its active bookings. Only possible before the session starts.</summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType<AdminSessionResponse>(StatusCodes.Status200OK)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    [ProducesProblem(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdminSessionResponse>> Cancel(Guid id, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Send(new CancelSessionCommand(id), cancellationToken));

    [HttpGet("{id:guid}/bookings")]
    [ProducesResponseType<PagedResponse<AdminBookingResponse>>(StatusCodes.Status200OK)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResponse<AdminBookingResponse>>> ListBookings(
        Guid id, [FromQuery] ListSessionBookingsRequest request, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Query(new ListSessionBookingsQuery(id, request.IncludeCancelled, request.Page, request.PageSize), cancellationToken));
}
