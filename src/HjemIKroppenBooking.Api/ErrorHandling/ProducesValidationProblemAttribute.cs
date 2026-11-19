using Microsoft.AspNetCore.Mvc;

namespace HjemIKroppenBooking.Api.ErrorHandling;

/// <summary>Declares a validation problem response (<see cref="ApiValidationProblemDetails"/>) for OpenAPI.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class ProducesValidationProblemAttribute(int statusCode)
    : ProducesResponseTypeAttribute(typeof(ApiValidationProblemDetails), statusCode, ProblemFields.ContentType);
