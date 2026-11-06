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
    public void Map_ReturnsFacetFilters_WhenNoSearchFiltersProvided()
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
        ReadOnlyCollection<FilterRequest> result = mapper.Map(input);

        // assert
        FilterRequest filter = Assert.Single(result);

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
                        DateRange = new()
                    }
                }
            }
        };

        // act
        ReadOnlyCollection<FilterRequest> result = mapper.Map(input);

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
                        DateRange = new()
                        {
                            From = CreateDateInput(1, 1, 2024),
                            To = CreateDateInput(31, 12, 2024)
                        }
                    }
                }
            }
        };

        // act
        ReadOnlyCollection<FilterRequest> result = mapper.Map(input);

        // assert
        FilterRequest filter = Assert.Single(result);

        Assert.Equal("OpenDate", filter.FilterName);
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
                        DateRange = new()
                        {
                            From = CreateDateInput(1, 1, 2024)
                        }
                    }
                }
            }
        };

        // act
        ReadOnlyCollection<FilterRequest> result = mapper.Map(input);

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
                        DateRange = new()
                        {
                            To = CreateDateInput(31, 12, 2024)
                        }
                    }
                }
            }
        };

        // act
        ReadOnlyCollection<FilterRequest> result = mapper.Map(input);

        // assert
        FilterRequest filter = Assert.Single(result);

        Assert.Null(filter.FilterValues[0]);
        Assert.Equal("2024-12-31", filter.FilterValues[1]);
    }

    [Fact]
    public void Map_ThrowsInvalidOperationException_WhenFromDateIsInvalid()
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
                        DateRange = new()
                        {
                            From = new DateInputViewModel
                            {
                                Day = "32",
                                Month = "1",
                                Year = "2024"
                            }
                        }
                    }
                }
            }
        };

        // act/assert
        Assert.Throws<InvalidOperationException>(() =>
            mapper.Map(input));
    }

    [Fact]
    public void Map_ThrowsInvalidOperationException_WhenToDateIsInvalid()
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
                        DateRange = new()
                        {
                            To = new DateInputViewModel
                            {
                                Day = "31",
                                Month = "13",
                                Year = "2024"
                            }
                        }
                    }
                }
            }
        };

        // act/assert
        Assert.Throws<InvalidOperationException>(() =>
            mapper.Map(input));
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
                        DateRange = new()
                        {
                            From = CreateDateInput(31, 12, 2024),
                            To = CreateDateInput(1, 1, 2024)
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
                        DateRange = new()
                        {
                            From = CreateDateInput(1, 1, 2024),
                            To = CreateDateInput(31, 12, 2024)
                        }
                    }
                }
            }
        };

        // act
        ReadOnlyCollection<FilterRequest> result = mapper.Map(input);

        // assert
        Assert.Equal(2, result.Count);

        Assert.Contains(
            result,
            filter => filter.FilterName == "Phase");

        Assert.Contains(
            result,
            filter => filter.FilterName == "OpenDate");
    }

    [Fact]
    public void Map_DoesNotAddNumberRangeFilter_WhenNumberRangeIsEmpty()
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
                "AgeRange",
                new FilterInputViewModel
                {
                    NumberRange = new NumberRangeFilterViewModel()
                }
            }
        }
        };

        // act
        ReadOnlyCollection<FilterRequest> result = mapper.Map(input);

        // assert
        Assert.Empty(result);
    }

    [Fact]
    public void Map_AddsNumberRangeFilter_WhenBothValuesProvided()
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
                "AgeRange",
                new FilterInputViewModel
                {
                    NumberRange = new NumberRangeFilterViewModel
                    {
                        From = 5,
                        To = 11
                    }
                }
            }
        }
        };

        // act
        ReadOnlyCollection<FilterRequest> result = mapper.Map(input);

        // assert
        FilterRequest filter = Assert.Single(result);

        Assert.Equal("AgeRange", filter.FilterName);
        Assert.Equal(5, filter.FilterValues[0]);
        Assert.Equal(11, filter.FilterValues[1]);
    }

    [Fact]
    public void Map_AddsNumberRangeFilter_WhenOnlyFromProvided()
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
                "AgeRange",
                new FilterInputViewModel
                {
                    NumberRange = new NumberRangeFilterViewModel
                    {
                        From = 5
                    }
                }
            }
        }
        };

        // act
        ReadOnlyCollection<FilterRequest> result = mapper.Map(input);

        // assert
        FilterRequest filter = Assert.Single(result);

        Assert.Equal("AgeRange", filter.FilterName);
        Assert.Equal(5, filter.FilterValues[0]);
        Assert.Null(filter.FilterValues[1]);
    }

    [Fact]
    public void Map_AddsNumberRangeFilter_WhenOnlyToProvided()
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
                "AgeRange",
                new FilterInputViewModel
                {
                    NumberRange = new NumberRangeFilterViewModel
                    {
                        To = 11
                    }
                }
            }
        }
        };

        // act
        ReadOnlyCollection<FilterRequest> result = mapper.Map(input);

        // assert
        FilterRequest filter = Assert.Single(result);

        Assert.Equal("AgeRange", filter.FilterName);
        Assert.Null(filter.FilterValues[0]);
        Assert.Equal(11, filter.FilterValues[1]);
    }

    [Fact]
    public void Map_ReturnsFacetAndNumberRangeFilters()
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
            Filters = new Dictionary<string, FilterInputViewModel>
        {
            {
                "AgeRange",
                new FilterInputViewModel
                {
                    NumberRange = new NumberRangeFilterViewModel
                    {
                        From = 5,
                        To = 11
                    }
                }
            }
        }
        };

        // act
        ReadOnlyCollection<FilterRequest> result = mapper.Map(input);

        // assert
        Assert.Equal(2, result.Count);

        Assert.Contains(
            result,
            filter => filter.FilterName == "Phase");

        Assert.Contains(
            result,
            filter => filter.FilterName == "AgeRange");
    }

    private static DateInputViewModel CreateDateInput(
        int day,
        int month,
        int year) =>
        new()
        {
            Day = day.ToString(),
            Month = month.ToString(),
            Year = year.ToString()
        };
}
