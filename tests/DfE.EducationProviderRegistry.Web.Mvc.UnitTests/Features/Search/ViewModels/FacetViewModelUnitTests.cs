using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.ViewModels;

public sealed class FacetViewModelUnitTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        // arrange
        string name = "Establishment status";
        List<FacetValueViewModel> values =
        [
            new FacetValueViewModel("Open", 120, false),
            new FacetValueViewModel("Closed", 30, true)
        ];

        // act
        FacetViewModel vm = new(name, values);

        // assert
        Assert.Equal(name, vm.Name);
        Assert.Equal(values, vm.Values);
        Assert.Equal(2, vm.Values.Count);
    }

    [Fact]
    public void Constructor_AllowsEmptyValuesList()
    {
        // arrange
        string name = "Phase";
        List<FacetValueViewModel> values = [];

        // act
        FacetViewModel vm = new(name, values);

        // assert
        Assert.Equal("Phase", vm.Name);
        Assert.Empty(vm.Values);
    }

    [Fact]
    public void Record_Equality_WorksForSameValues()
    {
        // arrange
        List<FacetValueViewModel> values =
        [
            new FacetValueViewModel("Primary", 10, false)
        ];

        FacetViewModel a = new("Phase", values);
        FacetViewModel b = new("Phase", values);

        // act
        bool equal = a == b;

        // assert
        Assert.True(equal);
        Assert.Equal(a, b);
    }

    [Fact]
    public void Record_Equality_FailsForDifferentNames()
    {
        // arrange
        List<FacetValueViewModel> values =
        [
            new FacetValueViewModel("Primary", 10, false)
        ];

        FacetViewModel a = new("Phase", values);
        FacetViewModel b = new("Type", values);

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
        List<FacetValueViewModel> values =
        [
            new FacetValueViewModel("Primary", 10, false)
        ];

        FacetViewModel original = new("Phase", values);

        // act
        FacetViewModel modified = original with { Name = "Updated Phase" };

        // assert
        Assert.Equal("Updated Phase", modified.Name);
        Assert.Equal(values, modified.Values);

        Assert.Equal("Phase", original.Name);
        Assert.Equal(values, original.Values);
    }

    [Fact]
    public void ValuesList_IsNotDeepCloned()
    {
        // arrange
        List<FacetValueViewModel> values =
        [
            new FacetValueViewModel("Primary", 10, false)
        ];

        FacetViewModel vm = new("Phase", values);

        // act
        values.Add(new FacetValueViewModel("Secondary", 20, true));

        // assert
        Assert.Equal(2, vm.Values.Count);
        Assert.Contains(vm.Values, v => v.Value == "Secondary");
    }
}
