using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Mappers;

public sealed class FacetResultsToViewModelMapperUnitTests
{
    private static SearchFacet MakeFacet(
        string name = "Phase",
        params (string key, string value, int count)[] results)
    {
        List<FacetResult> facetResults = new();

        foreach (var (key, value, count) in results)
        {
            facetResults.Add(new FacetResult(key, value, count));
        }

        return new SearchFacet(name, facetResults);
    }

    [Fact]
    public void Map_ThrowsArgumentNullException_WhenInputIsNull()
    {
        // arrange
        FacetResultsToViewModelMapper mapper = new();

        // act/assert
        Assert.Throws<ArgumentNullException>(() => mapper.Map(null!));
    }

    [Fact]
    public void Map_ReturnsEmptyList_WhenInputIsEmpty()
    {
        // arrange
        FacetResultsToViewModelMapper mapper = new();

        // act
        List<FacetViewModel> result = mapper.Map(Array.Empty<SearchFacet>());

        // assert
        Assert.Empty(result);
    }

    [Fact]
    public void Map_MapsEachFacet_ToFacetViewModel()
    {
        // arrange
        FacetResultsToViewModelMapper mapper = new();

        IReadOnlyCollection<SearchFacet> input =
        [
            MakeFacet("Phase", ("1", "Primary", 10)),
            MakeFacet("Type", ("2", "Academy", 5))
        ];

        // act
        List<FacetViewModel> result = mapper.Map(input);

        // assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Phase", result[0].Name);
        Assert.Equal("Type", result[1].Name);
    }

    [Fact]
    public void MapFacet_Throws_WhenFacetNameIsNull()
    {
        // arrange
        FacetResultsToViewModelMapper mapper = new();
        SearchFacet facet = new(null!, []);

        // act/assert
        Assert.Throws<ArgumentNullException>(() => mapper.Map([facet]));
    }

    [Fact]
    public void MapFacetValues_Throws_WhenFacetResultsIsNull()
    {
        // arrange
        FacetResultsToViewModelMapper mapper = new();
        SearchFacet facet = new("Phase", null!);

        // act/assert
        Assert.Throws<ArgumentNullException>(() => mapper.Map([facet]));
    }

    [Fact]
    public void MapFacetValue_Throws_WhenKeyIsNull()
    {
        // arrange
        FacetResultsToViewModelMapper mapper = new();
        SearchFacet facet = new("Phase", [new FacetResult(null!, "Primary", 5)]);

        // act/assert
        Assert.Throws<ArgumentNullException>(() => mapper.Map([facet]));
    }

    [Fact]
    public void MapFacetValue_Throws_WhenValueIsNull()
    {
        // arrange
        FacetResultsToViewModelMapper mapper = new();
        SearchFacet facet = new("Phase", [new FacetResult("1", null!, 5)]);

        // act/assert
        Assert.Throws<ArgumentNullException>(() => mapper.Map([facet]));
    }

    [Fact]
    public void MapFacetValue_Throws_WhenCountIsNull()
    {
        // arrange
        FacetResultsToViewModelMapper mapper = new();
        FacetResult result = new("1", "Primary", null);
        SearchFacet facet = new("Phase", [result]);

        // act/assert
        Assert.Throws<ArgumentException>(() => mapper.Map([facet]));
    }

    [Fact]
    public void Map_MapsFacetValuesCorrectly()
    {
        // arrange
        FacetResultsToViewModelMapper mapper = new();

        SearchFacet facet = MakeFacet(
            "Phase",
            ("1", "Primary", 10),
            ("2", "Secondary", 20)
        );

        // act
        List<FacetViewModel> result = mapper.Map([facet]);

        // assert
        FacetViewModel vm = result.Single();

        Assert.Equal("Phase", vm.Name);
        Assert.Equal(2, vm.Values.Count);
        Assert.Equal("Primary", vm.Values[0].Label);
        Assert.Equal(10, vm.Values[0].Count);
        Assert.False(vm.Values[0].IsSelected);
        Assert.Equal("Secondary", vm.Values[1].Label);
        Assert.Equal(20, vm.Values[1].Count);
        Assert.False(vm.Values[1].IsSelected);
    }
}
