using System.Collections.ObjectModel;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Controllers.TestDoubles;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Mappers;

public sealed class SearchFiltersToFilterRequestsMapperTests
{
    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenFacetMapperIsNull()
    {
        // act/assert
        Assert.Throws<ArgumentNullException>(() =>
            new SearchFiltersToFilterRequestsMapper(null!));
    }

    [Fact]
    public void Map_ThrowsArgumentNullException_WhenInputIsNull()
    {
        // arrange
        SearchFiltersToFilterRequestsMapper mapper =
            new(SearchFacetsResultsMapperTestDouble.Mock().Object);

        // act/assert
        Assert.Throws<ArgumentNullException>(() =>
            mapper.Map(null!));
    }

    [Fact]
    public void Map_ReturnsFacetFilters_WhenFiltersAreEmpty()
    {
        // arrange
        ReadOnlyCollection<FilterRequest> facetFilters =
        [
            new("Phase", ["Primary"])
        ];

        Mock<IMapper<Dictionary<string, List<string>>?, ReadOnlyCollection<FilterRequest>>> facetMapper =
            SearchFacetsResultsMapperTestDouble.MockFor(facetFilters);

        SearchFiltersToFilterRequestsMapper mapper =
            new(facetMapper.Object);

        SearchRequestViewModel input = new()
        {
            SelectedFacets = [],
            Filters = []
        };

        // act
        ReadOnlyCollection<FilterRequest> result =
            mapper.Map(input);

        // assert
        Assert.Single(result);

        FilterRequest filter = result[0];

        Assert.Equal("Phase", filter.FilterName);
        Assert.Single(filter.FilterValues);
        Assert.Equal("Primary", filter.FilterValues[0]);

        facetMapper.Verify();
    }

    [Fact]
    public void Map_DoesNotAddDateRangeFilter_WhenDateRangeIsEmpty()
    {
        // arrange
        Mock<IMapper<Dictionary<string, List<string>>?, ReadOnlyCollection<FilterRequest>>> facetMapper =
            SearchFacetsResultsMapperTestDouble.MockFor([]);

        SearchFiltersToFilterRequestsMapper mapper =
            new(facetMapper.Object);

        SearchRequestViewModel input = new()
        {
            Filters = new Dictionary<string, FilterInputViewModel>
            {
                {
                    "OpenDate",
                    new FilterInputViewModel
                    {
                        DateRange = new DateRangeFilterViewModel()
                    }
                }
            }
        };

        // act
        ReadOnlyCollection<FilterRequest> result =
            mapper.Map(input);

        // assert
        Assert.Empty(result);
    }

    [Fact]
    public void Map_AddsDateRangeFilter_WhenBothDatesProvided()
    {
        // arrange
        Mock<IMapper<Dictionary<string, List<string>>?, ReadOnlyCollection<FilterRequest>>> facetMapper =
            SearchFacetsResultsMapperTestDouble.MockFor([]);

        SearchFiltersToFilterRequestsMapper mapper =
            new(facetMapper.Object);

        SearchRequestViewModel input = new()
        {
            Filters = new Dictionary<string, FilterInputViewModel>
            {
                {
                    "OpenDate",
                    new FilterInputViewModel
                    {
                        DateRange = new DateRangeFilterViewModel
                        {
                            From = CreateDateInput(2024, 1, 1),
                            To = CreateDateInput(2024, 12, 31)
                        }
                    }
                }
            }
        };

        // act
        ReadOnlyCollection<FilterRequest> result =
            mapper.Map(input);

        // assert
        Assert.Single(result);

        FilterRequest filter = result[0];

        Assert.Equal("OpenDate", filter.FilterName);

        Assert.Equal(2, filter.FilterValues.Count);
        Assert.Equal("2024-01-01", filter.FilterValues[0]);
        Assert.Equal("2024-12-31", filter.FilterValues[1]);
    }

    [Fact]
    public void Map_AddsDateRangeFilter_WhenOnlyFromDateProvided()
    {
        // arrange
        Mock<IMapper<Dictionary<string, List<string>>?, ReadOnlyCollection<FilterRequest>>> facetMapper =
            SearchFacetsResultsMapperTestDouble.MockFor([]);

        SearchFiltersToFilterRequestsMapper mapper =
            new(facetMapper.Object);

        SearchRequestViewModel input = new()
        {
            Filters = new Dictionary<string, FilterInputViewModel>
            {
                {
                    "OpenDate",
                    new FilterInputViewModel
                    {
                        DateRange = new DateRangeFilterViewModel
                        {
                            From = CreateDateInput(2024, 1, 1)
                        }
                    }
                }
            }
        };

        // act
        ReadOnlyCollection<FilterRequest> result =
            mapper.Map(input);

        // assert
        FilterRequest filter = Assert.Single(result);

        Assert.Equal("2024-01-01", filter.FilterValues[0]);
        Assert.Null(filter.FilterValues[1]);
    }

    [Fact]
    public void Map_AddsDateRangeFilter_WhenOnlyToDateProvided()
    {
        // arrange
        Mock<IMapper<Dictionary<string, List<string>>?, ReadOnlyCollection<FilterRequest>>> facetMapper =
            SearchFacetsResultsMapperTestDouble.MockFor([]);

        SearchFiltersToFilterRequestsMapper mapper =
            new(facetMapper.Object);

        SearchRequestViewModel input = new()
        {
            Filters = new Dictionary<string, FilterInputViewModel>
            {
                {
                    "OpenDate",
                    new FilterInputViewModel
                    {
                        DateRange = new DateRangeFilterViewModel
                        {
                            To = CreateDateInput(2024, 12, 31)
                        }
                    }
                }
            }
        };

        // act
        ReadOnlyCollection<FilterRequest> result =
            mapper.Map(input);

        // assert
        FilterRequest filter = Assert.Single(result);

        Assert.Null(filter.FilterValues[0]);
        Assert.Equal("2024-12-31", filter.FilterValues[1]);
    }

    [Fact]
    public void Map_ThrowsInvalidOperationException_WhenFromDateIsAfterToDate()
    {
        // arrange
        Mock<IMapper<Dictionary<string, List<string>>?, ReadOnlyCollection<FilterRequest>>> facetMapper =
            SearchFacetsResultsMapperTestDouble.MockFor([]);

        SearchFiltersToFilterRequestsMapper mapper =
            new(facetMapper.Object);

        SearchRequestViewModel input = new()
        {
            Filters = new Dictionary<string, FilterInputViewModel>
            {
                {
                    "OpenDate",
                    new FilterInputViewModel
                    {
                        DateRange = new DateRangeFilterViewModel
                        {
                            From = CreateDateInput(2024, 12, 31),
                            To = CreateDateInput(2024, 1, 1)
                        }
                    }
                }
            }
        };

        // act/assert
        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(() =>
                mapper.Map(input));

        Assert.Equal(
            "Invalid date range for filter 'OpenDate'.",
            exception.Message);
    }

    [Fact]
    public void Map_ReturnsFacetAndDateRangeFilters()
    {
        // arrange
        ReadOnlyCollection<FilterRequest> facetFilters =
        [
            new("Phase", ["Primary"])
        ];

        Mock<IMapper<Dictionary<string, List<string>>?, ReadOnlyCollection<FilterRequest>>> facetMapper =
            SearchFacetsResultsMapperTestDouble.MockFor(facetFilters);

        SearchFiltersToFilterRequestsMapper mapper =
            new(facetMapper.Object);

        SearchRequestViewModel input = new()
        {
            SelectedFacets = [],
            Filters = new Dictionary<string, FilterInputViewModel>
            {
                {
                    "OpenDate",
                    new FilterInputViewModel
                    {
                        DateRange = new DateRangeFilterViewModel
                        {
                            From = CreateDateInput(2024, 1, 1),
                            To = CreateDateInput(2024, 12, 31)
                        }
                    }
                }
            }
        };

        // act
        ReadOnlyCollection<FilterRequest> result =
            mapper.Map(input);

        // assert
        Assert.Equal(2, result.Count);

        Assert.Contains(
            result,
            filter => filter.FilterName == "Phase");

        Assert.Contains(
            result,
            filter => filter.FilterName == "OpenDate");
    }

    private static DateInputViewModel CreateDateInput(
        int year,
        int month,
        int day)
    {
        return new DateInputViewModel
        {
            Day = day.ToString(),
            Month = month.ToString(),
            Year = year.ToString()
        };
    }
}
