using Asp.Versioning;
using HjemIKroppenBooking.Api.Contracts.Requests.ServiceCategories;
using HjemIKroppenBooking.Api.ErrorHandling;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;
using HjemIKroppenBooking.Application.Features.ServiceCategories.CreateServiceCategory;
using HjemIKroppenBooking.Application.Features.ServiceCategories.DeleteServiceCategory;
using HjemIKroppenBooking.Application.Features.ServiceCategories.GetServiceCategoryById;
using HjemIKroppenBooking.Application.Features.ServiceCategories.ListServiceCategories;
using HjemIKroppenBooking.Application.Features.ServiceCategories.UpdateServiceCategory;
using HjemIKroppenBooking.Application.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace HjemIKroppenBooking.Api.Controllers.Admin;

[ApiVersion("1.0")]
[Route(AdminControllerBase.RoutePrefix + "/service-categories")]
public sealed class AdminServiceCategoriesController(IDispatcher dispatcher) : AdminControllerBase(dispatcher)
{
    [HttpGet]
    [ProducesResponseType<PagedResponse<AdminServiceCategoryResponse>>(StatusCodes.Status200OK)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PagedResponse<AdminServiceCategoryResponse>>> List([FromQuery] ListServiceCategoriesRequest request, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Query(new ListServiceCategoriesQuery(request.Page, request.PageSize), cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType<AdminServiceCategoryResponse>(StatusCodes.Status200OK)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminServiceCategoryResponse>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Query(new GetServiceCategoryByIdQuery(id), cancellationToken));

    [HttpPost]
    [ProducesResponseType<AdminServiceCategoryResponse>(StatusCodes.Status201Created)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    [ProducesProblem(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdminServiceCategoryResponse>> Create(ServiceCategoryRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateServiceCategoryCommand(request.Name);
        var category = await Dispatcher.Send(command, cancellationToken);
        return Created(ApiRoutes.V1Path($"/admin/service-categories/{category.Id}"), category);
    }

    /// <summary>Renames the category. Its slug follows the new name.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType<AdminServiceCategoryResponse>(StatusCodes.Status200OK)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    [ProducesProblem(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdminServiceCategoryResponse>> Update(Guid id, ServiceCategoryRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateServiceCategoryCommand(id, request.Name);
        return Ok(await Dispatcher.Send(command, cancellationToken));
    }

    /// <summary>Deletes a category without service types. Move or delete its service types first.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    [ProducesProblem(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Dispatcher.Send(new DeleteServiceCategoryCommand(id), cancellationToken);
        return NoContent();
    }
}
