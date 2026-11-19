using Asp.Versioning;
using HjemIKroppenBooking.Api.Contracts.Requests.Profile;
using HjemIKroppenBooking.Api.ErrorHandling;
using HjemIKroppenBooking.Application.Contracts.Responses.Users;
using HjemIKroppenBooking.Application.Features.Users.DeactivateMe;
using HjemIKroppenBooking.Application.Features.Users.GetMe;
using HjemIKroppenBooking.Application.Features.Users.UpdateMe;
using HjemIKroppenBooking.Application.Features.Users.UpdateMyPassword;
using HjemIKroppenBooking.Application.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HjemIKroppenBooking.Api.Controllers;

/// <summary>The signed-in user's own profile.</summary>
[ApiVersion("1.0")]
[Route(ApiRoutes.Prefix + "/me")]
[Authorize]
[ProducesProblem(StatusCodes.Status401Unauthorized)]
public sealed class UsersController(IDispatcher dispatcher) : ApiControllerBase(dispatcher)
{
    [HttpGet]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<UserResponse>> GetMe(CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Query(new GetMeQuery(), cancellationToken));

    [HttpPut]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    [ProducesProblem(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> UpdateMe(UpdateMeRequest request, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Send(new UpdateMeCommand(request.FullName, request.Email), cancellationToken));

    /// <summary>A wrong current password is reported as a validation error on <c>currentPassword</c>.</summary>
    [HttpPut("password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        await Dispatcher.Send(new UpdateMyPasswordCommand(request.CurrentPassword, request.NewPassword), cancellationToken);
        return NoContent();
    }

    /// <summary>Deactivates the account and cancels all bookings on sessions that have not started.</summary>
    [HttpPost("deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Deactivate(DeactivateMeRequest request, CancellationToken cancellationToken)
    {
        await Dispatcher.Send(new DeactivateMeCommand(request.Password), cancellationToken);
        return NoContent();
    }
}
