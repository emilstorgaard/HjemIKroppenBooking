using System.Runtime.CompilerServices;
using FluentValidation;
using HjemIKroppenBooking.Application;
using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.UnitTests.Domain.Common;

namespace HjemIKroppenBooking.UnitTests.Application.Validation;

/// <summary>Checks every validator in the Application assembly against the error architecture rules.</summary>
public sealed class ValidatorConventionTests
{
    private static readonly IReadOnlyList<Type> ValidatorTypes = typeof(DependencyInjection).Assembly.GetTypes()
        .Where(t => t is { IsClass: true, IsAbstract: false } && RequestTypeOf(t) is { IsInterface: false })
        .OrderBy(t => t.FullName, StringComparer.Ordinal)
        .ToList();

    public static TheoryData<string> ValidatorNames
    {
        get
        {
            var data = new TheoryData<string>();
            foreach (var type in ValidatorTypes)
            {
                data.Add(type.FullName!);
            }

            return data;
        }
    }

    [Fact]
    public void ValidatorTypes_Always_AreDiscovered()
    {
        // Arrange
        var validatorTypes = ValidatorTypes;

        // Act
        var count = validatorTypes.Count;

        // Assert
        Assert.True(count >= 25, $"Expected at least 25 validators, found {count}.");
    }

    [Theory]
    [MemberData(nameof(ValidatorNames))]
    public void Validate_UninitializedRequest_OnlyEmitsDeclaredErrorCodes(string validatorName)
    {
        // Arrange
        var validatorType = ValidatorTypes.Single(t => t.FullName == validatorName);
        var validator = (IValidator)Activator.CreateInstance(validatorType)!;
        var request = RuntimeHelpers.GetUninitializedObject(RequestTypeOf(validatorType)!);

        // Act
        var result = validator.Validate(new ValidationContext<object>(request));

        // Assert
        Assert.All(result.Errors, failure => Assert.Contains(failure.ErrorCode, ErrorCodesTests.AllCodes));
    }

    [Theory]
    [MemberData(nameof(ValidatorNames))]
    public void ValidatorType_Always_DerivesFromAppValidator(string validatorName)
    {
        // Arrange
        var validatorType = ValidatorTypes.Single(t => t.FullName == validatorName);

        // Act
        var baseDefinition = validatorType.BaseType is { IsGenericType: true } baseType ? baseType.GetGenericTypeDefinition() : null;

        // Assert
        Assert.Equal(typeof(AppValidator<>), baseDefinition);
    }

    private static Type? RequestTypeOf(Type type)
    {
        for (var current = type.BaseType; current is not null; current = current.BaseType)
        {
            if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(AbstractValidator<>))
            {
                return current.GetGenericArguments()[0];
            }
        }

        return null;
    }
}
