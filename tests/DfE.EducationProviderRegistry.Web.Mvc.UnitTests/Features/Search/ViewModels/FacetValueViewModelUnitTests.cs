using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.ViewModels;

public sealed class FacetValueViewModelUnitTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        // arrange
        string value = "1";
        string label = "Open, but proposed to close";
        long? count = 150;
        bool isSelected = true;

        // act
        FacetValueViewModel vm = new(value, label, count, isSelected);

        // assert
        Assert.Equal(value, vm.Value);
        Assert.Equal(label, vm.Label);
        Assert.Equal(count, vm.Count);
        Assert.True(vm.IsSelected);
    }

    [Fact]
    public void Constructor_AllowsNullCount()
    {
        // arrange
        string value = "1";
        string label = "Primary";
        long? count = null;

        // act
        FacetValueViewModel vm = new(value, label, count, false);

        // assert
        Assert.Null(vm.Count);
        Assert.Equal(value, vm.Value);
        Assert.Equal(label, vm.Label);
        Assert.False(vm.IsSelected);
    }

    [Fact]
    public void Record_Equality_WorksForSameValues()
    {
        // arrange
        FacetValueViewModel a = new("1", "Primary", 10, true);
        FacetValueViewModel b = new("1", "Primary", 10, true);

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
        FacetValueViewModel a = new("1", "Primary", 10, true);
        FacetValueViewModel b = new("2", "Secondary", 10, true);

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
        FacetValueViewModel original = new("1", "Primary", 10, false);

        // act
        FacetValueViewModel modified = original with { Count = 20, IsSelected = true };

        // assert
        Assert.Equal("1", modified.Value);
        Assert.Equal("Primary", modified.Label);
        Assert.Equal(20, modified.Count);
        Assert.True(modified.IsSelected);
        Assert.Equal("1", original.Value);
        Assert.Equal("Primary", original.Label);
        Assert.Equal(10, original.Count);
        Assert.False(original.IsSelected);
    }

    [Fact]
    public void Properties_AreImmutable()
    {
        // arrange
        FacetValueViewModel vm = new("1", "Primary", 10, false);

        // assert
        Assert.Equal("1", vm.Value);
        Assert.Equal("Primary", vm.Label);
        Assert.Equal(10, vm.Count);
        Assert.False(vm.IsSelected);
    }
}
