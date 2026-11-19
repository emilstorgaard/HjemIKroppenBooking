using HjemIKroppenBooking.Api.ErrorHandling;
using HjemIKroppenBooking.Api.Security;
using HjemIKroppenBooking.Application.Messaging;
using Microsoft.AspNetCore.Authorization;

namespace HjemIKroppenBooking.Api.Controllers.Admin;

/// <summary>Base for the administration controllers under <c>/api/v1/admin</c>. Requires the Admin role.</summary>
[Authorize(Policy = AuthorizationPolicies.Admin)]
[ProducesProblem(StatusCodes.Status401Unauthorized)]
[ProducesProblem(StatusCodes.Status403Forbidden)]
public abstract class AdminControllerBase(IDispatcher dispatcher) : ApiControllerBase(dispatcher)
{
    public const string RoutePrefix = ApiRoutes.Prefix + "/admin";
}
