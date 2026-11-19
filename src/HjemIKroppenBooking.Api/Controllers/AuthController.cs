using Asp.Versioning;
using HjemIKroppenBooking.Api.Contracts.Requests.Auth;
using HjemIKroppenBooking.Api.ErrorHandling;
using HjemIKroppenBooking.Api.Extensions;
using HjemIKroppenBooking.Application.Contracts.Responses.Auth;
using HjemIKroppenBooking.Application.Contracts.Responses.Users;
using HjemIKroppenBooking.Application.Features.Auth.Login;
using HjemIKroppenBooking.Application.Features.Auth.Register;
using HjemIKroppenBooking.Application.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HjemIKroppenBooking.Api.Controllers;

[ApiVersion("1.0")]
[Route(ApiRoutes.Prefix + "/auth")]
[AllowAnonymous]
[EnableRateLimiting(RateLimitingExtensions.AuthPolicy)]
public sealed class AuthController(IDispatcher dispatcher) : ApiControllerBase(dispatcher)
{
    /// <summary>Creates a user account with the User role.</summary>
    [HttpPost("register")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status201Created)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    [ProducesProblem(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var user = await Dispatcher.Send(new RegisterCommand(request.Email, request.FullName, request.Password), cancellationToken);
        return Created(ApiRoutes.V1Path($"/me"), user);
    }

    /// <summary>Returns a 12-hour access token. Wrong email, wrong password and deactivated accounts give the same error.</summary>
    [HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    [ProducesProblem(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Send(new LoginCommand(request.Email, request.Password), cancellationToken));
}
