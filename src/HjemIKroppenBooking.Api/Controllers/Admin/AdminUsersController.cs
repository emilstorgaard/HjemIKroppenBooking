using Asp.Versioning;
using HjemIKroppenBooking.Api.Contracts.Requests.Users;
using HjemIKroppenBooking.Api.ErrorHandling;
using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.Users;
using HjemIKroppenBooking.Application.Features.Bookings.ListBookings;
using HjemIKroppenBooking.Application.Features.Users.ActivateUser;
using HjemIKroppenBooking.Application.Features.Users.ChangeUserRole;
using HjemIKroppenBooking.Application.Features.Users.DeactivateUser;
using HjemIKroppenBooking.Application.Features.Users.GetUserById;
using HjemIKroppenBooking.Application.Features.Users.ListUsers;
using HjemIKroppenBooking.Application.Features.Users.ResetUserPassword;
using HjemIKroppenBooking.Application.Features.Users.UpdateUser;
using HjemIKroppenBooking.Application.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace HjemIKroppenBooking.Api.Controllers.Admin;

/// <summary>User administration. Admins cannot change their own role or active state here; that prevents locking out the last admin by mistake.</summary>
[ApiVersion("1.0")]
[Route(AdminControllerBase.RoutePrefix + "/users")]
public sealed class AdminUsersController(IDispatcher dispatcher) : AdminControllerBase(dispatcher)
{
    [HttpGet]
    [ProducesResponseType<PagedResponse<AdminUserResponse>>(StatusCodes.Status200OK)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PagedResponse<AdminUserResponse>>> List([FromQuery] ListUsersRequest request, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Query(new ListUsersQuery(request.Search, request.Role, request.IsActive, request.Page, request.PageSize), cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType<AdminUserResponse>(StatusCodes.Status200OK)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminUserResponse>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Query(new GetUserByIdQuery(id), cancellationToken));

    [HttpPut("{id:guid}")]
    [ProducesResponseType<AdminUserResponse>(StatusCodes.Status200OK)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    [ProducesProblem(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdminUserResponse>> Update(Guid id, UpdateUserRequest request, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Send(new UpdateUserCommand(id, request.FullName, request.Email), cancellationToken));

    /// <summary>Changes the role. Takes effect at the user's next sign-in, because the role is part of the access token.</summary>
    [HttpPut("{id:guid}/role")]
    [ProducesResponseType<AdminUserResponse>(StatusCodes.Status200OK)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    [ProducesProblem(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdminUserResponse>> ChangeRole(Guid id, ChangeUserRoleRequest request, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Send(new ChangeUserRoleCommand(id, request.Role), cancellationToken));

    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    [ProducesProblem(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        await Dispatcher.Send(new ActivateUserCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>Deactivates the user, which blocks existing tokens immediately, and cancels bookings on sessions that have not started.</summary>
    [HttpPost("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    [ProducesProblem(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        await Dispatcher.Send(new DeactivateUserCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetPassword(Guid id, ResetUserPasswordRequest request, CancellationToken cancellationToken)
    {
        await Dispatcher.Send(new ResetUserPasswordCommand(id, request.NewPassword), cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}/bookings")]
    [ProducesResponseType<PagedResponse<AdminBookingResponse>>(StatusCodes.Status200OK)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PagedResponse<AdminBookingResponse>>> ListBookings(Guid id, [FromQuery] ListUserBookingsRequest request, CancellationToken cancellationToken)
    {
        var query = new ListBookingsQuery(UserId: id, SeriesId: request.SeriesId, Status: request.Status, Upcoming: request.Upcoming, Page: request.Page, PageSize: request.PageSize);
        return Ok(await Dispatcher.Query(query, cancellationToken));
    }
}
