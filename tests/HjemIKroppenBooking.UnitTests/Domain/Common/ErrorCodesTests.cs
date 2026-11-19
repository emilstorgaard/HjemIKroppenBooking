using System.Reflection;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.UnitTests.Domain.Common;

public sealed class ErrorCodesTests
{
    /// <summary>Every code declared as a constant on <see cref="ErrorCodes"/>.</summary>
    public static IReadOnlyList<string> AllCodes { get; } = typeof(ErrorCodes)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.IsLiteral && f.FieldType == typeof(string))
        .Select(f => (string)f.GetRawConstantValue()!)
        .ToList();

    public static TheoryData<string> CodeData
    {
        get
        {
            var data = new TheoryData<string>();
            foreach (var code in AllCodes)
            {
                data.Add(code);
            }

            return data;
        }
    }

    [Fact]
    public void AllCodes_AreUnique()
    {
        // Arrange
        var codes = AllCodes;

        // Act
        var duplicates = codes.GroupBy(c => c, StringComparer.Ordinal).Where(g => g.Count() > 1).Select(g => g.Key).ToList();

        // Assert
        Assert.NotEmpty(codes);
        Assert.Empty(duplicates);
    }

    [Theory]
    [MemberData(nameof(CodeData))]
    public void Code_Always_FollowsAreaDotReasonConvention(string code)
    {
        // Arrange
        var parts = code.Split('.');

        // Act
        var isValid = parts.Length == 2 && parts.All(IsSnakeCase);

        // Assert
        Assert.True(isValid, $"'{code}' does not follow the area.snake_case_reason convention.");
    }

    private static bool IsSnakeCase(string part) =>
        part.Length > 0
        && part[0] != '_'
        && part[^1] != '_'
        && !part.Contains("__", StringComparison.Ordinal)
        && part.All(c => c is (>= 'a' and <= 'z') or '_');
}
