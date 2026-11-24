using Asp.Versioning;
using HjemIKroppenBooking.Api.Contracts.Requests.ServiceTypes;
using HjemIKroppenBooking.Api.ErrorHandling;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;
using HjemIKroppenBooking.Application.Features.ServiceTypes.ActivateServiceType;
using HjemIKroppenBooking.Application.Features.ServiceTypes.CreateServiceType;
using HjemIKroppenBooking.Application.Features.ServiceTypes.DeactivateServiceType;
using HjemIKroppenBooking.Application.Features.ServiceTypes.DeleteServiceType;
using HjemIKroppenBooking.Application.Features.ServiceTypes.GetServiceTypeById;
using HjemIKroppenBooking.Application.Features.ServiceTypes.ListServiceTypes;
using HjemIKroppenBooking.Application.Features.ServiceTypes.UpdateServiceType;
using HjemIKroppenBooking.Application.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace HjemIKroppenBooking.Api.Controllers.Admin;

[ApiVersion("1.0")]
[Route(AdminControllerBase.RoutePrefix + "/service-types")]
public sealed class AdminServiceTypesController(IDispatcher dispatcher) : AdminControllerBase(dispatcher)
{
    [HttpGet]
    [ProducesResponseType<PagedResponse<AdminServiceTypeResponse>>(StatusCodes.Status200OK)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PagedResponse<AdminServiceTypeResponse>>> List([FromQuery] ListServiceTypesRequest request, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Query(new ListServiceTypesQuery(request.CategoryId, request.IsActive, request.Page, request.PageSize), cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType<AdminServiceTypeResponse>(StatusCodes.Status200OK)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminServiceTypeResponse>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await Dispatcher.Query(new GetServiceTypeByIdQuery(id), cancellationToken));

    [HttpPost]
    [ProducesResponseType<AdminServiceTypeResponse>(StatusCodes.Status201Created)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    [ProducesProblem(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdminServiceTypeResponse>> Create(ServiceTypeRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateServiceTypeCommand(
            request.CategoryId, request.Name, request.Description, request.DefaultDurationMinutes, request.DefaultCapacity, request.IsActive);
        var serviceType = await Dispatcher.Send(command, cancellationToken);
        return Created(ApiRoutes.V1Path($"/admin/service-types/{serviceType.Id}"), serviceType);
    }

    /// <summary>Replaces the service type. It can move to another category at any time.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType<AdminServiceTypeResponse>(StatusCodes.Status200OK)]
    [ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    [ProducesProblem(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdminServiceTypeResponse>> Update(Guid id, ServiceTypeRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateServiceTypeCommand(
            id, request.CategoryId, request.Name, request.Description, request.DefaultDurationMinutes, request.DefaultCapacity, request.IsActive);
        return Ok(await Dispatcher.Send(command, cancellationToken));
    }

    /// <summary>Deletes a service type that no session or series uses. Deactivate it instead when it is in use.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    [ProducesProblem(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Dispatcher.Send(new DeleteServiceTypeCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        await Dispatcher.Send(new ActivateServiceTypeCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>Hides the service type and its sessions from the public lists. Existing bookings are kept.</summary>
    [HttpPost("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesProblem(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        await Dispatcher.Send(new DeactivateServiceTypeCommand(id), cancellationToken);
        return NoContent();
    }
}
