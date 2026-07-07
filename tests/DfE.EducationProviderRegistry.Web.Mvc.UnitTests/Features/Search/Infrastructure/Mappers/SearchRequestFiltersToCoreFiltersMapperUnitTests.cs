using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Filtering;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Mappers;
using System.Collections.ObjectModel;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Infrastructure.Mappers;

public sealed class SearchRequestFiltersToCoreFiltersMapperTests
{
    private static ReadOnlyCollection<FilterRequest> Build(
        params FilterRequest[] filters) => new(filters);

    [Fact]
    public void Map_ReturnsEmpty_WhenInputIsNull()
    {
        // arrange
        SearchRequestFiltersToCoreFiltersMapper mapper = new SearchRequestFiltersToCoreFiltersMapper();

        // act
        ReadOnlyCollection<SearchFilterRequest> result = mapper.Map(null!);

        // assert
        Assert.Empty(result);
    }

    [Fact]
    public void Map_ReturnsEmpty_WhenInputIsEmpty()
    {
        // arrange
        SearchRequestFiltersToCoreFiltersMapper mapper = new SearchRequestFiltersToCoreFiltersMapper();
        ReadOnlyCollection<FilterRequest> input = Build();

        // act
        ReadOnlyCollection<SearchFilterRequest> result = mapper.Map(input);

        // assert
        Assert.Empty(result);
    }

    [Fact]
    public void Map_IgnoresFiltersWithNullValues()
    {
        // arrange
        SearchRequestFiltersToCoreFiltersMapper mapper = new();

        FilterRequest filter =
            new(
                filterName: "type",
                filterValues: [] // empty list = “null values” scenario
            );

        ReadOnlyCollection<FilterRequest> input = new([filter]);

        // act
        ReadOnlyCollection<SearchFilterRequest> result = mapper.Map(input);

        // assert
        Assert.Empty(result);
    }

    [Fact]
    public void Map_IgnoresFiltersWithEmptyValues()
    {
        // arrange
        SearchRequestFiltersToCoreFiltersMapper mapper = new SearchRequestFiltersToCoreFiltersMapper();

        FilterRequest filter =
            new(
                filterName: "type",
                filterValues: []
            );

        ReadOnlyCollection<FilterRequest> input = Build(filter);

        // act
        ReadOnlyCollection<SearchFilterRequest> result = mapper.Map(input);

        // assert
        Assert.Empty(result);
    }

    [Fact]
    public void Map_MapsSingleFilterCorrectly()
    {
        // arrange
        SearchRequestFiltersToCoreFiltersMapper mapper = new SearchRequestFiltersToCoreFiltersMapper();

        FilterRequest filter =
            new(
                filterName: "type",
                filterValues: ["Academy"]
            );

        ReadOnlyCollection<FilterRequest> input = Build(filter);

        // act
        ReadOnlyCollection<SearchFilterRequest> result = mapper.Map(input);

        // assert
        Assert.Single(result);
        Assert.Equal("type", result[0].FilterKey);
        Assert.Single(result[0].FilterValues);
        Assert.Equal("Academy", result[0].FilterValues[0]);
    }

    [Fact]
    public void Map_MapsMultipleFiltersCorrectly()
    {
        // arrange
        SearchRequestFiltersToCoreFiltersMapper mapper = new SearchRequestFiltersToCoreFiltersMapper();

        FilterRequest filter1 =
            new(
                filterName: "type",
                filterValues: ["Academy", "Free School"]
            );

        FilterRequest filter2 =
            new(
                filterName: "phase",
                filterValues: ["Primary"]
            );

        ReadOnlyCollection<FilterRequest> input = Build(filter1, filter2);

        // act
        ReadOnlyCollection<SearchFilterRequest> result = mapper.Map(input);

        // assert
        Assert.Equal(2, result.Count);
        Assert.Equal("type", result[0].FilterKey);
        Assert.Equal(2, result[0].FilterValues.Length);
        Assert.Equal("Academy", result[0].FilterValues[0]);
        Assert.Equal("Free School", result[0].FilterValues[1]);
        Assert.Equal("phase", result[1].FilterKey);
        Assert.Single(result[1].FilterValues);
        Assert.Equal("Primary", result[1].FilterValues[0]);
    }

    [Fact]
    public void Map_CopiesValuesRatherThanReferencingOriginalCollection()
    {
        // arrange
        SearchRequestFiltersToCoreFiltersMapper mapper = new();

        List<object> originalValues = ["Academy"];

        FilterRequest filter =
            new(
                filterName: "type",
                filterValues: originalValues
            );

        ReadOnlyCollection<FilterRequest> input = Build(filter);

        // act
        ReadOnlyCollection<SearchFilterRequest> result = mapper.Map(input);
        originalValues[0] = "Changed"; // mutate original to ensure copy was made.

        // assert
        Assert.Equal("Academy", result[0].FilterValues[0]);
    }
}
