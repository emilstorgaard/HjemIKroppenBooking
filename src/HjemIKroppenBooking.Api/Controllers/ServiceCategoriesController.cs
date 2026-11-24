using Asp.Versioning;
using HjemIKroppenBooking.Api.Contracts.Requests.ServiceCategories;
using HjemIKroppenBooking.Api.ErrorHandling;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;
using HjemIKroppenBooking.Application.Features.ServiceCategories.ListPublicServiceCategories;
using HjemIKroppenBooking.Application.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HjemIKroppenBooking.Api.Controllers;

/// <summary>All categories, readable without signing in. Used by the booking page and the category picker in Umbraco.</summary>
[ApiVersion("1.0")]
[Route(ApiRoutes.Prefix + "/service-categories")]
[AllowAnonymous]
public sealed class ServiceCategoriesController(IDispatcher dispatcher) : ApiControllerBase(dispatcher)
{
    [HttpGet]
    [ProducesResponseType<PagedResponse<ServiceCategoryResponse>>(StatusCodes.Status200OK)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PagedResponse<ServiceCategoryResponse>>> List([FromQuery] ListServiceCategoriesRequest request, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Query(new ListPublicServiceCategoriesQuery(request.Page, request.PageSize), cancellationToken));
}
