using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.ViewModels;

public sealed class SearchResultsViewModelTests
{
    [Fact]
    public void Facets_WhenSet_OrdersFacetsByName()
    {
        // Arrange
        SearchResultsViewModel model = CreateModel();

        List<FacetViewModel> facets =
        [
            CreateFacet("Type"),
            CreateFacet("Local Authority"),
            CreateFacet("Status")
        ];

        // Act
        model.Facets = facets;

        // Assert
        Assert.Collection(
            model.Facets!,
            facet => Assert.Equal("Local Authority", facet.Name),
            facet => Assert.Equal("Status", facet.Name),
            facet => Assert.Equal("Type", facet.Name));
    }

    [Fact]
    public void Facets_WhenSetToNull_ReturnsNull()
    {
        // Arrange
        SearchResultsViewModel model = CreateModel();

        // Act
        model.Facets = null;

        // Assert
        Assert.Null(model.Facets);
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
        model.TotalEstablishmentResults = totalEstablishmentResults;

        // Act
        bool result = model.HasResults;

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, false)]
    [InlineData(2, true)]
    public void HasMoreThanOneResult_ReturnsExpectedResult(
        int totalEstablishmentResults,
        bool expected)
    {
        // Arrange
        SearchResultsViewModel model = CreateModel();
        model.TotalEstablishmentResults = totalEstablishmentResults;

        // Act
        bool result = model.HasMoreThanOneResult;

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void HasFilters_WithFacets_ReturnsTrue()
    {
        // Arrange
        SearchResultsViewModel model = CreateModel();

        model.Facets =
        [
            CreateFacet("Type")
        ];

        // Act
        bool result = model.HasFilters;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasFilters_WithEmptyFacets_ReturnsFalse()
    {
        // Arrange
        SearchResultsViewModel model = CreateModel
        ();

        model.Facets = [];

        // Act
        bool result = model.HasFilters;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasFilters_WithNullFacets_ReturnsFalse()
    {
        // Arrange
        SearchResultsViewModel model = CreateModel();
        model.Facets = null;

        // Act
        bool result = model.HasFilters;

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
        SearchResultsViewModel model = CreateModel();

        model.TotalEstablishmentResults = totalEstablishmentResults;
        model.SearchRequest.RecordsPerPage = recordsPerPage;

        // Act
        int result = model.TotalPages;

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void TotalPages_WithInvalidRecordsPerPage_ReturnsZero(
        int recordsPerPage)
    {
        // Arrange
        SearchResultsViewModel model = CreateModel();

        model.TotalEstablishmentResults = 100;
        model.SearchRequest.RecordsPerPage = recordsPerPage;

        // Act
        int result = model.TotalPages;

        // Assert
        Assert.Equal(0, result);
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
        SearchResultsViewModel model = CreateModel();
        model.SearchRequest.PageNumber = pageNumber;

        // Act
        bool result = model.HasPreviousPage;

        // Assert
        Assert.Equal(expected, result);
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
        SearchResultsViewModel model = CreateModel();

        model.TotalEstablishmentResults = totalEstablishmentResults;
        model.SearchRequest.PageNumber = pageNumber;
        model.SearchRequest.RecordsPerPage = 20;

        // Act
        bool result = model.HasNextPage;

        // Assert
        Assert.Equal(expected, result);
    }

    private static SearchResultsViewModel CreateModel()
    {
        return new SearchResultsViewModel
        {
            EstablishmentResults = [],
            SearchRequest = new SearchRequestViewModel
            {
                PageNumber = 1,
                RecordsPerPage = 20
            }
        };
    }

    private static FacetViewModel CreateFacet(string name)
    {
        return new FacetViewModel
        (
            Name: name,
            Label: name,
            Values: []
        );
    }
}