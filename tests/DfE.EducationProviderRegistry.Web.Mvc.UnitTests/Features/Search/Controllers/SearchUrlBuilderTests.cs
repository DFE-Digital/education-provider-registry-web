using DfE.EducationProviderRegistry.Web.Mvc.Features.Search;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search;

public sealed class SearchUrlBuilderTests
{
    private readonly Mock<IUrlHelper> _urlHelper = new();

    public SearchUrlBuilderTests()
    {
        _urlHelper
            .Setup(url => url.Action(It.IsAny<UrlActionContext>()))
            .Returns("/Search");
    }

    [Fact]
    public void BuildPageUrl_WithSearchRequest_ReturnsExpectedUrl()
    {
        // Arrange
        SearchRequestViewModel searchRequest = new()
        {
            SearchKeywords = "test school",
            Address = "testLocation",
            Sort = "NameAscending",
            RecordsPerPage = 20
        };

        // Act
        string result = SearchUrlBuilder.BuildPageUrl(
            _urlHelper.Object,
            searchRequest,
            2);

        // Assert
        Uri uri = new($"https://test.local{result}");
        Dictionary<string, StringValues> query =
            QueryHelpers.ParseQuery(uri.Query);

        Assert.Equal("/Search", uri.AbsolutePath);
        Assert.Equal("test school", query["SearchKeywords"]);
        Assert.Equal("testLocation", query["Address"]);
        Assert.Equal("NameAscending", query["Sort"]);
        Assert.Equal("2", query["PageNumber"]);
        Assert.Equal("20", query["RecordsPerPage"]);
    }

    [Fact]
    public void BuildPageUrl_WithSelectedFacets_AddsFacetsToQueryString()
    {
        // Arrange
        SearchRequestViewModel searchRequest = new()
        {
            RecordsPerPage = 20,
            SelectedFacets = new Dictionary<string, List<string>>
            {
                ["EstablishmentType"] = ["Academy", "College"],
                ["LocalAuthority"] = ["testLocation"]
            }
        };

        // Act
        string result = SearchUrlBuilder.BuildPageUrl(
            _urlHelper.Object,
            searchRequest,
            1);

        // Assert
        Uri uri = new($"https://test.local{result}");
        Dictionary<string, StringValues> query =
            QueryHelpers.ParseQuery(uri.Query);

        Assert.Equal(
            new[] { "Academy", "College" },
            query["SelectedFacets[EstablishmentType]"].ToArray());

        Assert.Equal(
            "testLocation",
            query["SelectedFacets[LocalAuthority]"]);
    }

    [Fact]
    public void BuildPageUrl_WithNullSelectedFacets_DoesNotAddSelectedFacets()
    {
        // Arrange
        SearchRequestViewModel searchRequest = new()
        {
            RecordsPerPage = 20,
        };

        // Act
        string result = SearchUrlBuilder.BuildPageUrl(
            _urlHelper.Object,
            searchRequest,
            1);

        // Assert
        Uri uri = new($"https://test.local{result}");
        Dictionary<string, StringValues> query =
            QueryHelpers.ParseQuery(uri.Query);

        Assert.DoesNotContain(
            query.Keys,
            key => key.StartsWith("SelectedFacets"));
    }

    [Fact]
    public void BuildPageUrl_WithEmptyOptionalValues_DoesNotAddThemToQueryString()
    {
        // Arrange
        SearchRequestViewModel searchRequest = new()
        {
            SearchKeywords = string.Empty,
            Address = " ",
            Sort = null,
            RecordsPerPage = 20
        };

        // Act
        string result = SearchUrlBuilder.BuildPageUrl(
            _urlHelper.Object,
            searchRequest,
            3);

        // Assert
        Uri uri = new($"https://test.local{result}");
        Dictionary<string, StringValues> query =
            QueryHelpers.ParseQuery(uri.Query);

        Assert.False(query.ContainsKey("SearchKeywords"));
        Assert.False(query.ContainsKey("Address"));
        Assert.False(query.ContainsKey("Sort"));

        Assert.Equal("3", query["PageNumber"]);
        Assert.Equal("20", query["RecordsPerPage"]);
    }

    [Fact]
    public void BuildPageUrl_WithSpecialCharacters_EncodesAndPreservesValues()
    {
        // Arrange
        SearchRequestViewModel searchRequest = new()
        {
            SearchKeywords = "test school & college",
            Address = "testLocation",
            RecordsPerPage = 20
        };

        // Act
        string result = SearchUrlBuilder.BuildPageUrl(
            _urlHelper.Object,
            searchRequest,
            1);

        // Assert
        Uri uri = new($"https://test.local{result}");
        Dictionary<string, StringValues> query =
            QueryHelpers.ParseQuery(uri.Query);

        Assert.Equal(
            "test school & college",
            query["SearchKeywords"]);

        Assert.Equal(
            "testLocation",
            query["Address"]);
    }

    [Fact]
    public void BuildPageUrl_WithNullUrlHelper_ThrowsArgumentNullException()
    {
        // Arrange
        SearchRequestViewModel searchRequest = new()
        {
            RecordsPerPage = 20
        };

        // Act
        void action() => SearchUrlBuilder.BuildPageUrl(
            null!,
            searchRequest,
            1);

        // Assert
        Assert.Throws<ArgumentNullException>(action);
    }

    [Fact]
    public void BuildPageUrl_WithNullSearchRequest_ThrowsArgumentNullException()
    {
        // Act
        void action() => SearchUrlBuilder.BuildPageUrl(
            _urlHelper.Object,
            null!,
            1);

        // Assert
        Assert.Throws<ArgumentNullException>(action);
    }
}
