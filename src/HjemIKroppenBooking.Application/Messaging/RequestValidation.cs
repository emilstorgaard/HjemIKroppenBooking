using FluentValidation;
using HjemIKroppenBooking.Application.Common.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace HjemIKroppenBooking.Application.Messaging;

/// <summary>Pipeline step that runs every registered FluentValidation validator for a request.</summary>
internal static class RequestValidation
{
    public static async Task ValidateAsync<TRequest>(TRequest request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        List<ValidationError>? errors = null;
        foreach (var validator in serviceProvider.GetServices<IValidator<TRequest>>())
        {
            var result = await validator.ValidateAsync(request, cancellationToken);
            foreach (var failure in result.Errors)
            {
                errors ??= [];
                errors.Add(new ValidationError(ValidationError.ToFieldPath(failure.PropertyName), failure.ErrorCode));
            }
        }

        if (errors is not null)
        {
            throw new RequestValidationException(errors);
        }
    }
}
