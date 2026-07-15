using DfE.EducationProviderRegistry.Web.Mvc.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Extensions;

public sealed class ModelStateExtensionsTests
{
    [Fact]
    public void HasErrorFor_ReturnsTrue_WhenFieldHasErrors()
    {
        // Arrange
        ViewDataDictionary viewData = CreateViewData();
        viewData.ModelState.AddModelError("SearchKeywords", "Required");

        // Act
        bool result = viewData.HasErrorFor("SearchKeywords");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasErrorFor_ReturnsFalse_WhenFieldHasNoErrors()
    {
        // Arrange
        ViewDataDictionary viewData = CreateViewData();
        viewData.ModelState.SetModelValue(
            "SearchKeywords",
            "School",
            "School");

        // Act
        bool result = viewData.HasErrorFor("SearchKeywords");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasErrorFor_ReturnsFalse_WhenFieldDoesNotExist()
    {
        // Arrange
        ViewDataDictionary viewData = CreateViewData();

        // Act
        bool result = viewData.HasErrorFor("SearchKeywords");

        // Assert
        Assert.False(result);
    }

    private static ViewDataDictionary CreateViewData()
    {
        return new ViewDataDictionary(
            new EmptyModelMetadataProvider(),
            new ModelStateDictionary());
    }
}