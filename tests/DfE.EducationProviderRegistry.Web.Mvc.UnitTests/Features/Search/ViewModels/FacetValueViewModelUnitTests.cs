using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.ViewModels;

public sealed class FacetValueViewModelUnitTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        // arrange
        string value = "Open, but proposed to close";
        long? count = 150;
        bool isSelected = true;

        // act
        FacetValueViewModel vm = new(value, count, isSelected);

        // assert
        Assert.Equal(value, vm.Value);
        Assert.Equal(count, vm.Count);
        Assert.True(vm.IsSelected);
    }

    [Fact]
    public void Constructor_AllowsNullCount()
    {
        // arrange
        string value = "Primary";
        long? count = null;

        // act
        FacetValueViewModel vm = new(value, count, false);

        // assert
        Assert.Null(vm.Count);
        Assert.Equal("Primary", vm.Value);
        Assert.False(vm.IsSelected);
    }

    [Fact]
    public void Record_Equality_WorksForSameValues()
    {
        // arrange
        FacetValueViewModel a = new("Primary", 10, true);
        FacetValueViewModel b = new("Primary", 10, true);

        // act
        bool equal = a == b;

        // assert
        Assert.True(equal);
        Assert.Equal(a, b);
    }

    [Fact]
    public void Record_Equality_FailsForDifferentValues()
    {
        // arrange
        FacetValueViewModel a = new("Primary", 10, true);
        FacetValueViewModel b = new("Secondary", 10, true);

        // act
        bool equal = a == b;

        // assert
        Assert.False(equal);
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void WithExpression_CreatesModifiedCopy()
    {
        // arrange
        FacetValueViewModel original = new("Primary", 10, false);

        // act
        FacetValueViewModel modified = original with { Count = 20, IsSelected = true };

        // assert
        Assert.Equal("Primary", modified.Value);
        Assert.Equal(20, modified.Count);
        Assert.True(modified.IsSelected);
        Assert.Equal("Primary", original.Value);
        Assert.Equal(10, original.Count);
        Assert.False(original.IsSelected);
    }

    [Fact]
    public void Properties_AreImmutable()
    {
        // arrange
        FacetValueViewModel vm = new("Primary", 10, false);

        // assert
        Assert.Equal("Primary", vm.Value);
        Assert.Equal(10, vm.Count);
        Assert.False(vm.IsSelected);
    }
}
