using Asp.Versioning;
using HjemIKroppenBooking.Api.Contracts.Requests.Bookings;
using HjemIKroppenBooking.Api.Contracts.Requests.Series;
using HjemIKroppenBooking.Api.ErrorHandling;
using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Features.Bookings.CancelMyBooking;
using HjemIKroppenBooking.Application.Features.Bookings.CreateBooking;
using HjemIKroppenBooking.Application.Features.Bookings.GetMyBookingById;
using HjemIKroppenBooking.Application.Features.Bookings.ListMyBookings;
using HjemIKroppenBooking.Application.Features.Series.CancelMySeriesBooking;
using HjemIKroppenBooking.Application.Features.Series.CreateSeriesBooking;
using HjemIKroppenBooking.Application.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HjemIKroppenBooking.Api.Controllers;

/// <summary>The signed-in user's bookings of sessions and series.</summary>
[ApiVersion("1.0")]
[Route(ApiRoutes.Prefix + "/me")]
[Authorize]
[ProducesProblem(StatusCodes.Status401Unauthorized)]
public sealed class BookingsController(IDispatcher dispatcher) : ApiControllerBase(dispatcher)
{
    [HttpGet("bookings")]
    [ProducesResponseType<PagedResponse<BookingResponse>>(StatusCodes.Status200OK)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PagedResponse<BookingResponse>>> List([FromQuery] ListMyBookingsRequest request, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Query(new ListMyBookingsQuery(request.Status, request.Upcoming, request.SeriesId, request.Page, request.PageSize), cancellationToken));

    [HttpGet("bookings/{id:guid}")]
    [ProducesResponseType<BookingResponse>(StatusCodes.Status200OK)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingResponse>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Query(new GetMyBookingByIdQuery(id), cancellationToken));

    /// <summary>Books a session. Overlapping sessions may be booked; a session can only be booked once per user.</summary>
    [HttpPost("bookings")]
    [ProducesResponseType<BookingResponse>(StatusCodes.Status201Created)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    [ProducesProblem(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BookingResponse>> Create(CreateBookingRequest request, CancellationToken cancellationToken)
    {
        var booking = await Dispatcher.Send(new CreateBookingCommand(request.SessionId, request.Note), cancellationToken);
        return Created(ApiRoutes.V1Path($"/me/bookings/{booking.Id}"), booking);
    }

    /// <summary>Cancels the booking. Allowed right up to the session start.</summary>
    [HttpDelete("bookings/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    [ProducesProblem(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        await Dispatcher.Send(new CancelMyBookingCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Books every session of the series that has not started, all or nothing. Sessions already booked are skipped.
    /// If a session is full nothing is booked, and the error's <c>parameters.sessionId</c> names the full session.
    /// </summary>
    [HttpPost("series/{seriesId:guid}/bookings")]
    [ProducesResponseType<SeriesBookingResponse>(StatusCodes.Status201Created)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    [ProducesProblem(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SeriesBookingResponse>> BookSeries(
        Guid seriesId, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] CreateSeriesBookingRequest? request, CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(new CreateSeriesBookingCommand(seriesId, request?.Note), cancellationToken);
        return Created(ApiRoutes.V1Path($"/me/bookings?seriesId={seriesId}"), result);
    }

    /// <summary>Cancels the user's bookings on the series' sessions that have not started.</summary>
    [HttpDelete("series/{seriesId:guid}/bookings")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelSeries(Guid seriesId, CancellationToken cancellationToken)
    {
        await Dispatcher.Send(new CancelMySeriesBookingCommand(seriesId), cancellationToken);
        return NoContent();
    }
}
