using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.ServiceCategories;
using static HjemIKroppenBooking.UnitTests.Domain.TestData;

namespace HjemIKroppenBooking.UnitTests.Domain.ServiceCategories;

public sealed class ServiceCategoryTests
{
    [Theory]
    [InlineData("Danseterapi", "danseterapi")]
    [InlineData("  Dans & bevægelse  ", "dans-bevaegelse")]
    [InlineData("Åndedræt og ro", "aandedraet-og-ro")]
    [InlineData("Øvelser", "oevelser")]
    [InlineData("Café-workshop!!", "cafe-workshop")]
    [InlineData("Krop/Sind 2027", "krop-sind-2027")]
    [InlineData("!!!", "kategori")]
    public void SlugFrom_Name_MakesUrlKey(string name, string expected)
    {
        // Act
        var slug = ServiceCategory.SlugFrom(name);

        // Assert
        Assert.Equal(expected, slug);
    }

    [Fact]
    public void SlugFrom_VeryLongName_LeavesRoomForSuffix()
    {
        // Arrange
        var name = new string('a', ServiceCategory.NameMaxLength);

        // Act
        var slug = ServiceCategory.SlugFrom(name);

        // Assert
        Assert.True(slug.Length <= ServiceCategory.SlugMaxLength - 10);
    }

    [Fact]
    public void Create_ValidName_StoresTrimmedNameAndSlug()
    {
        // Act
        var category = ServiceCategory.Create("  Samtaleterapi  ", "samtaleterapi");

        // Assert
        Assert.Equal("Samtaleterapi", category.Name);
        Assert.Equal("samtaleterapi", category.Slug);
    }

    [Fact]
    public void Create_InvalidSlug_ThrowsSlugInvalid()
    {
        // Act
        var exception = Record.Exception(() => ServiceCategory.Create("Samtaleterapi", "Samtale terapi"));

        // Assert
        AssertDomainError(exception, ErrorCodes.ValidationSlugInvalid, ErrorType.Validation);
    }
}
