using HjemIKroppenBooking.Api.ErrorHandling;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.UnitTests.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace HjemIKroppenBooking.UnitTests.Api.ErrorHandling;

public sealed class ErrorCatalogTests
{
    [Theory]
    [MemberData(nameof(ErrorCodesTests.CodeData), MemberType = typeof(ErrorCodesTests))]
    public void EveryErrorCode_HasADanishMessage(string code)
    {
        Assert.True(ErrorCatalog.All.TryGetValue(code, out var message), $"Missing message for '{code}'.");
        Assert.False(string.IsNullOrWhiteSpace(message));
    }

    [Fact]
    public void Catalog_HasNoMessagesForUnknownCodes()
    {
        var orphans = ErrorCatalog.All.Keys.Except(ErrorCodesTests.AllCodes, StringComparer.Ordinal).ToList();
        Assert.Empty(orphans);
    }

    [Fact]
    public void UnknownCode_FallsBackToTheGenericMessage() =>
        Assert.Equal(ErrorCatalog.All[ErrorCodes.ServerUnexpectedError], ErrorCatalog.GetMessage("does.not_exist"));

    [Theory]
    [InlineData(ErrorType.Validation, StatusCodes.Status422UnprocessableEntity)]
    [InlineData(ErrorType.NotFound, StatusCodes.Status404NotFound)]
    [InlineData(ErrorType.Conflict, StatusCodes.Status409Conflict)]
    [InlineData(ErrorType.Forbidden, StatusCodes.Status403Forbidden)]
    [InlineData(ErrorType.Unauthorized, StatusCodes.Status401Unauthorized)]
    public void ErrorType_MapsToStatusCode(ErrorType errorType, int expected) => Assert.Equal(expected, ErrorStatusCodes.FromErrorType(errorType));

    [Fact]
    public void EveryErrorType_HasAnExplicitStatusCode()
    {
        foreach (var errorType in Enum.GetValues<ErrorType>())
        {
            Assert.NotEqual(StatusCodes.Status500InternalServerError, ErrorStatusCodes.FromErrorType(errorType));
        }
    }

    [Fact]
    public void ValidationProblem_GroupsErrorsPerFieldWithCodeAndMessage()
    {
        var problem = Problems.Validation([("email", ErrorCodes.ValidationRequired), ("email", ErrorCodes.ValidationEmailInvalid), ("password", ErrorCodes.ValidationPasswordTooShort)]);

        Assert.Equal(StatusCodes.Status422UnprocessableEntity, problem.Status);
        Assert.Equal(ErrorCodes.ValidationFailed, problem.Extensions[ProblemFields.Code]);
        var errors = Assert.IsType<Dictionary<string, List<FieldError>>>(problem.Extensions[ProblemFields.Errors]);
        Assert.Equal([ErrorCodes.ValidationRequired, ErrorCodes.ValidationEmailInvalid], errors["email"].Select(e => e.Code));
        Assert.Equal(ErrorCatalog.GetMessage(ErrorCodes.ValidationPasswordTooShort), Assert.Single(errors["password"]).Message);
    }

    [Fact]
    public void DomainProblem_KeepsParametersButNotTheFieldName()
    {
        var sessionId = Guid.NewGuid();
        var parameters = new Dictionary<string, object?> { ["sessionId"] = sessionId, ["field"] = "x" };

        var problem = Problems.Error(StatusCodes.Status409Conflict, ErrorCodes.SessionFull, parameters);

        Assert.Equal(ErrorCatalog.GetMessage(ErrorCodes.SessionFull), problem.Detail);
        var extra = Assert.IsType<Dictionary<string, object?>>(problem.Extensions[ProblemFields.Parameters]);
        Assert.Equal(sessionId, extra["sessionId"]);
        Assert.False(extra.ContainsKey("field"));
    }
}
