using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Request;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Results;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.ViewModels;

public sealed class SearchResultsViewModelTests
{
    [Fact]
    public void Filters_WhenSet_ReturnsFiltersInSameOrder()
    {
        // Arrange
        SearchResultsViewModel model = CreateModel();

        List<IFilterViewModel> filters =
        [
            CreateFilter("Type"),
            CreateFilter("Local Authority"),
            CreateFilter("Status")
        ];

        // Act
        model.Filters = filters;

        // Assert
        Assert.Collection(
            model.Filters,
            filter => Assert.Equal("Type", filter.Name),
            filter => Assert.Equal("Local Authority", filter.Name),
            filter => Assert.Equal("Status", filter.Name));

    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(2, true)]
    public void HasResults_ReturnsExpectedResult(
        int totalEstablishmentResults,
        bool expected)
    {
        // Arrange
        SearchResultsViewModel model = CreateModel();

        model.TotalSearchResults =
            totalEstablishmentResults;

        // Act
        bool result =
            model.HasResults;

        // Assert
        Assert.Equal(
            expected,
            result);
    }

    [Fact]
    public void HasFilters_WithFilters_ReturnsTrue()
    {
        // Arrange
        SearchResultsViewModel model =
            CreateModel();

        model.Filters =
        [
            CreateFilter("Type")
        ];

        // Act
        bool result =
            model.HasFilters;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasFilters_WithEmptyFilters_ReturnsFalse()
    {
        // Arrange
        SearchResultsViewModel model =
            CreateModel();

        model.Filters = [];

        // Act
        bool result =
            model.HasFilters;

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(20, 20, 1)]
    [InlineData(21, 20, 2)]
    [InlineData(40, 20, 2)]
    [InlineData(41, 20, 3)]
    [InlineData(0, 20, 0)]
    public void TotalPages_ReturnsExpectedNumberOfPages(
        int totalEstablishmentResults,
        int recordsPerPage,
        int expected)
    {
        // Arrange
        SearchResultsViewModel model =
            CreateModel();

        model.TotalSearchResults =
            totalEstablishmentResults;

        model.SearchRequest.RecordsPerPage =
            recordsPerPage;

        // Act
        int result =
            model.TotalPages;

        // Assert
        Assert.Equal(
            expected,
            result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void TotalPages_WithInvalidRecordsPerPage_ReturnsZero(
        int recordsPerPage)
    {
        // Arrange
        SearchResultsViewModel model =
            CreateModel();

        model.TotalSearchResults = 100;

        model.SearchRequest.RecordsPerPage =
            recordsPerPage;

        // Act
        int result =
            model.TotalPages;

        // Assert
        Assert.Equal(
            0,
            result);
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(2, true)]
    [InlineData(3, true)]
    public void HasPreviousPage_ReturnsExpectedResult(
        int pageNumber,
        bool expected)
    {
        // Arrange
        SearchResultsViewModel model =
            CreateModel();

        model.SearchRequest.PageNumber =
            pageNumber;

        // Act
        bool result =
            model.HasPreviousPage;

        // Assert
        Assert.Equal(
            expected,
            result);
    }

    [Theory]
    [InlineData(1, 40, true)]
    [InlineData(2, 40, false)]
    [InlineData(3, 40, false)]
    public void HasNextPage_ReturnsExpectedResult(
        int pageNumber,
        int totalEstablishmentResults,
        bool expected)
    {
        // Arrange
        SearchResultsViewModel model =
            CreateModel();

        model.TotalSearchResults =
            totalEstablishmentResults;

        model.SearchRequest.PageNumber =
            pageNumber;

        model.SearchRequest.RecordsPerPage =
            20;

        // Act
        bool result =
            model.HasNextPage;

        // Assert
        Assert.Equal(
            expected,
            result);
    }

    private static SearchResultsViewModel CreateModel()
    {
        return new SearchResultsViewModel
        {
            SearchAggregateResults = [],
            Filters = [],
            SearchRequest = new SearchRequestViewModel
            {
                PageNumber = 1,
                RecordsPerPage = 20
            }
        };
    }

    private static IFilterViewModel CreateFilter(
        string name)
    {
        return new CheckboxFilterViewModel(
            Name: name,
            Label: name,
            Values: []);
    }
}
