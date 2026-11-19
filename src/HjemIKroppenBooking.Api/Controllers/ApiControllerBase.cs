using HjemIKroppenBooking.Api.ErrorHandling;
using HjemIKroppenBooking.Application.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace HjemIKroppenBooking.Api.Controllers;

/// <summary>
/// Base for all controllers. Controllers only map HTTP requests to commands and queries and results to responses;
/// all rules live in the Application and Domain layers.
/// </summary>
[ApiController]
[Produces("application/json")]
[ProducesValidationProblem(StatusCodes.Status400BadRequest)]
[ProducesProblem(StatusCodes.Status429TooManyRequests)]
[ProducesProblem(StatusCodes.Status500InternalServerError)]
public abstract class ApiControllerBase(IDispatcher dispatcher) : ControllerBase
{
    protected IDispatcher Dispatcher { get; } = dispatcher;
}
