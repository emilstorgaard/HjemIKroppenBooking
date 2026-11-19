using Microsoft.AspNetCore.Mvc;

namespace HjemIKroppenBooking.Api.ErrorHandling;

/// <summary>Declares a problem response (<see cref="ApiProblemDetails"/>) for OpenAPI.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class ProducesProblemAttribute(int statusCode)
    : ProducesResponseTypeAttribute(typeof(ApiProblemDetails), statusCode, ProblemFields.ContentType);
