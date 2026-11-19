using Asp.Versioning;
using HjemIKroppenBooking.Api.Contracts.Requests.Bookings;
using HjemIKroppenBooking.Api.ErrorHandling;
using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Features.Bookings.CancelBooking;
using HjemIKroppenBooking.Application.Features.Bookings.CreateBookingForUser;
using HjemIKroppenBooking.Application.Features.Bookings.GetBookingById;
using HjemIKroppenBooking.Application.Features.Bookings.ListBookings;
using HjemIKroppenBooking.Application.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace HjemIKroppenBooking.Api.Controllers.Admin;

[ApiVersion("1.0")]
[Route(AdminControllerBase.RoutePrefix + "/bookings")]
public sealed class AdminBookingsController(IDispatcher dispatcher) : AdminControllerBase(dispatcher)
{
    [HttpGet]
    [ProducesResponseType<PagedResponse<AdminBookingResponse>>(StatusCodes.Status200OK)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PagedResponse<AdminBookingResponse>>> List([FromQuery] ListBookingsRequest request, CancellationToken cancellationToken)
    {
        var query = new ListBookingsQuery(
            request.UserId, request.SessionId, request.SeriesId, request.ServiceTypeId, request.Status, request.Upcoming,
            request.From?.UtcDateTime, request.To?.UtcDateTime, request.Page, request.PageSize);
        return Ok(await Dispatcher.Query(query, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<AdminBookingResponse>(StatusCodes.Status200OK)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminBookingResponse>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Query(new GetBookingByIdQuery(id), cancellationToken));

    /// <summary>Books a session on behalf of an active user. Same rules as when users book themselves.</summary>
    [HttpPost]
    [ProducesResponseType<AdminBookingResponse>(StatusCodes.Status201Created)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    [ProducesProblem(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdminBookingResponse>> Create(AdminCreateBookingRequest request, CancellationToken cancellationToken)
    {
        var booking = await Dispatcher.Send(new CreateBookingForUserCommand(request.UserId, request.SessionId, request.Note), cancellationToken);
        return Created(ApiRoutes.V1Path($"/admin/bookings/{booking.Id}"), booking);
    }

    /// <summary>Cancels the booking. Allowed right up to the session start.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    [ProducesProblem(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        await Dispatcher.Send(new CancelBookingCommand(id), cancellationToken);
        return NoContent();
    }
}
