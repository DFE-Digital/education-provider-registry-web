using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Request;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Results;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Mappers;

public sealed class AvailableFiltersToViewModelMapperTests
{
    [Fact]
    public void Map_GivenNullInput_ThrowsArgumentNullException()
    {
        // Arrange
        AvailableFiltersToViewModelMapper mapper = new();

        // Act
        Func<IReadOnlyCollection<IFilterViewModel>> map = () =>
            mapper.Map(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(map);
    }

    [Fact]
    public void Map_GivenNoAvailableFilters_ReturnsEmptyCollection()
    {
        // Arrange
        AvailableFiltersToViewModelMapper mapper = new();

        AvailableFiltersMappingContext context = new(
            AvailableFilters: [],
            SearchRequest: new SearchRequestViewModel());

        // Act
        IReadOnlyCollection<IFilterViewModel> result =
            mapper.Map(context);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Map_GivenOpenedDate_ReturnsDateRangeFilter()
    {
        // Arrange
        AvailableFiltersToViewModelMapper mapper = new();

        AvailableFiltersMappingContext context = new(
            AvailableFilters:
            [
                "opened_date"
            ],
            SearchRequest: new SearchRequestViewModel());

        // Act
        IReadOnlyCollection<IFilterViewModel> result =
            mapper.Map(context);

        // Assert
        DateRangeFilterViewModel filter =
            Assert.IsType<DateRangeFilterViewModel>(
                Assert.Single(result));

        Assert.Equal(
            "opened_date",
            filter.Name);

        Assert.Equal(
            "Opened date",
            filter.Label);

        Assert.NotNull(filter.Range);
        Assert.True(filter.Range.IsEmpty);
    }

    [Fact]
    public void Map_GivenClosedDate_ReturnsDateRangeFilter()
    {
        // Arrange
        AvailableFiltersToViewModelMapper mapper = new();

        AvailableFiltersMappingContext context = new(
            AvailableFilters:
            [
                "closed_date"
            ],
            SearchRequest: new SearchRequestViewModel());

        // Act
        IReadOnlyCollection<IFilterViewModel> result =
            mapper.Map(context);

        // Assert
        DateRangeFilterViewModel filter =
            Assert.IsType<DateRangeFilterViewModel>(
                Assert.Single(result));

        Assert.Equal(
            "closed_date",
            filter.Name);

        Assert.Equal(
            "Closed date",
            filter.Label);

        Assert.NotNull(filter.Range);
        Assert.True(filter.Range.IsEmpty);
    }

    [Fact]
    public void Map_GivenAgeRange_ReturnsNumberRangeFilter()
    {
        // Arrange
        AvailableFiltersToViewModelMapper mapper = new();

        AvailableFiltersMappingContext context = new(
            AvailableFilters:
            [
                "age_range"
            ],
            SearchRequest: new SearchRequestViewModel());

        // Act
        IReadOnlyCollection<IFilterViewModel> result =
            mapper.Map(context);

        // Assert
        NumberRangeFilterViewModel filter =
            Assert.IsType<NumberRangeFilterViewModel>(
                Assert.Single(result));

        Assert.Equal(
            "age_range",
            filter.Name);

        Assert.Equal(
            "Age range",
            filter.Label);

        Assert.NotNull(filter.Range);
        Assert.True(filter.Range.IsEmpty);
    }

    [Fact]
    public void Map_GivenAllAvailableFilters_MapsEachFilterInOrder()
    {
        // Arrange
        AvailableFiltersToViewModelMapper mapper = new();

        AvailableFiltersMappingContext context = new(
            AvailableFilters:
            [
                "opened_date",
                "closed_date",
                "age_range"
            ],
            SearchRequest: new SearchRequestViewModel());

        // Act
        IReadOnlyCollection<IFilterViewModel> result =
            mapper.Map(context);

        // Assert
        Assert.Collection(
            result,
            filter =>
            {
                DateRangeFilterViewModel openedDate =
                    Assert.IsType<DateRangeFilterViewModel>(
                        filter);

                Assert.Equal(
                    "opened_date",
                    openedDate.Name);

                Assert.Equal(
                    "Opened date",
                    openedDate.Label);
            },
            filter =>
            {
                DateRangeFilterViewModel closedDate =
                    Assert.IsType<DateRangeFilterViewModel>(
                        filter);

                Assert.Equal(
                    "closed_date",
                    closedDate.Name);

                Assert.Equal(
                    "Closed date",
                    closedDate.Label);
            },
            filter =>
            {
                NumberRangeFilterViewModel ageRange =
                    Assert.IsType<NumberRangeFilterViewModel>(
                        filter);

                Assert.Equal(
                    "age_range",
                    ageRange.Name);

                Assert.Equal(
                    "Age range",
                    ageRange.Label);
            });
    }

    [Fact]
    public void Map_GivenOpenedDateWithExistingRequestState_UsesExistingDateRange()
    {
        // Arrange
        AvailableFiltersToViewModelMapper mapper = new();

        DateRangeInputFilterViewModel range = new()
        {
            From = new DateInputViewModel
            {
                Day = "1",
                Month = "9",
                Year = "2024"
            },
            To = new DateInputViewModel
            {
                Day = "30",
                Month = "9",
                Year = "2024"
            }
        };

        SearchRequestViewModel searchRequest = new()
        {
            Filters = new Dictionary<string, FilterInputViewModel>
            {
                ["opened_date"] = new FilterInputViewModel
                {
                    DateRange = range
                }
            }
        };

        AvailableFiltersMappingContext context = new(
            AvailableFilters:
            [
                "opened_date"
            ],
            SearchRequest: searchRequest);

        // Act
        IReadOnlyCollection<IFilterViewModel> result =
            mapper.Map(context);

        // Assert
        DateRangeFilterViewModel filter =
            Assert.IsType<DateRangeFilterViewModel>(
                Assert.Single(result));

        Assert.Same(
            range,
            filter.Range);
    }

    [Fact]
    public void Map_GivenClosedDateWithExistingRequestState_UsesExistingDateRange()
    {
        // Arrange
        AvailableFiltersToViewModelMapper mapper = new();

        DateRangeInputFilterViewModel range = new()
        {
            From = new DateInputViewModel
            {
                Year = "2026"
            }
        };

        SearchRequestViewModel searchRequest = new()
        {
            Filters = new Dictionary<string, FilterInputViewModel>
            {
                ["closed_date"] = new FilterInputViewModel
                {
                    DateRange = range
                }
            }
        };

        AvailableFiltersMappingContext context = new(
            AvailableFilters:
            [
                "closed_date"
            ],
            SearchRequest: searchRequest);

        // Act
        IReadOnlyCollection<IFilterViewModel> result =
            mapper.Map(context);

        // Assert
        DateRangeFilterViewModel filter =
            Assert.IsType<DateRangeFilterViewModel>(
                Assert.Single(result));

        Assert.Same(
            range,
            filter.Range);

        Assert.Equal(
            "2026",
            filter.Range.From.Year);
    }

    [Fact]
    public void Map_GivenAgeRangeWithExistingRequestState_UsesExistingNumberRange()
    {
        // Arrange
        AvailableFiltersToViewModelMapper mapper = new();

        NumberRangeInputFilterViewModel range = new()
        {
            From = "5",
            To = "16"
        };

        SearchRequestViewModel searchRequest = new()
        {
            Filters = new Dictionary<string, FilterInputViewModel>
            {
                ["age_range"] = new FilterInputViewModel
                {
                    NumberRange = range
                }
            }
        };

        AvailableFiltersMappingContext context = new(
            AvailableFilters:
            [
                "age_range"
            ],
            SearchRequest: searchRequest);

        // Act
        IReadOnlyCollection<IFilterViewModel> result =
            mapper.Map(context);

        // Assert
        NumberRangeFilterViewModel filter =
            Assert.IsType<NumberRangeFilterViewModel>(
                Assert.Single(result));

        Assert.Same(
            range,
            filter.Range);

        Assert.Equal(
            "5",
            filter.Range.From);

        Assert.Equal(
            "16",
            filter.Range.To);
    }

    [Fact]
    public void Map_GivenFilterKeyWithDifferentCase_MapsFilter()
    {
        // Arrange
        AvailableFiltersToViewModelMapper mapper = new();

        AvailableFiltersMappingContext context = new(
            AvailableFilters:
            [
                "OPENED_DATE"
            ],
            SearchRequest: new SearchRequestViewModel());

        // Act
        IReadOnlyCollection<IFilterViewModel> result =
            mapper.Map(context);

        // Assert
        DateRangeFilterViewModel filter =
            Assert.IsType<DateRangeFilterViewModel>(
                Assert.Single(result));

        Assert.Equal(
            "OPENED_DATE",
            filter.Name);

        Assert.Equal(
            "Opened date",
            filter.Label);
    }

    [Fact]
    public void Map_GivenUnsupportedFilter_ThrowsInvalidOperationException()
    {
        // Arrange
        AvailableFiltersToViewModelMapper mapper = new();

        AvailableFiltersMappingContext context = new(
            AvailableFilters:
            [
                "unsupported_filter"
            ],
            SearchRequest: new SearchRequestViewModel());

        // Act
        Func<IReadOnlyCollection<IFilterViewModel>> map = () =>
            mapper.Map(context);

        // Assert
        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(map);

        Assert.Equal(
            "No Web filter presentation is configured for 'unsupported_filter'.",
            exception.Message);
    }
}
