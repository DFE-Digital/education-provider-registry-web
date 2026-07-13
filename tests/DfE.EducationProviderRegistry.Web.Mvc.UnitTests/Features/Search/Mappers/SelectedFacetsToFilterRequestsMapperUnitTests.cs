using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Mappers;

public sealed class SelectedFacetsToFilterRequestsMapperTests
{
    [Fact]
    public void Map_ReturnsEmptyCollection_WhenInputIsNull()
    {
        // arrange
        SelectedFacetsToFilterRequestsMapper mapper = new();

        // act
        var result = mapper.Map(null);

        // assert
        Assert.Empty(result);
    }

    [Fact]
    public void Map_ReturnsEmptyCollection_WhenInputIsEmpty()
    {
        // arrange
        SelectedFacetsToFilterRequestsMapper mapper = new();
        Dictionary<string, List<string>> input = [];

        // act
        var result = mapper.Map(input);

        // assert
        Assert.Empty(result);
    }

    [Fact]
    public void Map_MapsSingleFacetCorrectly()
    {
        // arrange
        SelectedFacetsToFilterRequestsMapper mapper = new();

        Dictionary<string, List<string>> input = new()
        {
            { "Phase", new List<string> { "Primary", "Secondary" } }
        };

        // act
        var result = mapper.Map(input);

        // assert
        Assert.Single(result);

        FilterRequest request = result[0];
        Assert.Equal("Phase", request.FilterName);
        Assert.Equal(2, request.FilterValues.Count);
        Assert.Contains("Primary", request.FilterValues);
        Assert.Contains("Secondary", request.FilterValues);
    }

    [Fact]
    public void Map_MapsMultipleFacetsCorrectly()
    {
        // arrange
        SelectedFacetsToFilterRequestsMapper mapper = new();

        Dictionary<string, List<string>> input = new()
        {
            { "Phase", new List<string> { "Primary" } },
            { "Type", new List<string> { "Academy", "Free School" } }
        };

        // act
        var result = mapper.Map(input);

        // assert
        Assert.Equal(2, result.Count);

        FilterRequest phase = result.Single(filterRequest =>
            filterRequest.FilterName == "Phase");
        Assert.Single(phase.FilterValues);
        Assert.Equal("Primary", phase.FilterValues[0]);

        FilterRequest type = result.Single(filterRequest =>
            filterRequest.FilterName == "Type");
        Assert.Equal(2, type.FilterValues.Count);
        Assert.Contains("Academy", type.FilterValues);
        Assert.Contains("Free School", type.FilterValues);
    }

    [Fact]
    public void Map_CastsValuesToObject()
    {
        // arrange
        SelectedFacetsToFilterRequestsMapper mapper = new();

        Dictionary<string, List<string>> input = new()
        {
            { "Phase", new List<string> { "Primary" } }
        };

        // act
        var result = mapper.Map(input);

        // assert
        var values = result[0].FilterValues;

        Assert.All(values, value => Assert.IsType<string>(value));
        Assert.Equal("Primary", values[0]);
    }
}
