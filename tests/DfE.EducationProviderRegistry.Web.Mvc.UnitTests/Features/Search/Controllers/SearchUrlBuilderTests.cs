using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Controllers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Controllers;

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
            What = "test school",
            Where = "testLocation",
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
        Assert.Equal("test school", query["What"]);
        Assert.Equal("testLocation", query["Where"]);
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
            What = string.Empty,
            Where = " ",
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
            What = "test school & college",
            Where = "testLocation",
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
            query["What"]);

        Assert.Equal(
            "testLocation",
            query["Where"]);
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

    [Fact]
    public void BuildPageUrl_WithDateRangeFilter_AddsDateRangeToQueryString()
    {
        // Arrange
        SearchRequestViewModel searchRequest = new()
        {
            RecordsPerPage = 20,
            Filters = new Dictionary<string, FilterInputViewModel>
        {
            {
                "OpenDate",
                new FilterInputViewModel
                {
                    DateRange = new DateRangeInputFilterViewModel
                    {
                        From = new DateInputViewModel
                        {
                            Day = "1",
                            Month = "1",
                            Year = "2024"
                        },
                        To = new DateInputViewModel
                        {
                            Day = "31",
                            Month = "12",
                            Year = "2024"
                        }
                    }
                }
            }
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

        Assert.Equal("1", query["Filters[OpenDate].DateRange.From.Day"]);
        Assert.Equal("1", query["Filters[OpenDate].DateRange.From.Month"]);
        Assert.Equal("2024", query["Filters[OpenDate].DateRange.From.Year"]);
        Assert.Equal("31", query["Filters[OpenDate].DateRange.To.Day"]);
        Assert.Equal("12", query["Filters[OpenDate].DateRange.To.Month"]);
        Assert.Equal("2024", query["Filters[OpenDate].DateRange.To.Year"]);
    }

    [Fact]
    public void BuildPageUrl_WithEmptyDateRange_DoesNotAddDateRangeToQueryString()
    {
        // Arrange
        SearchRequestViewModel searchRequest = new()
        {
            RecordsPerPage = 20,
            Filters = new Dictionary<string, FilterInputViewModel>
        {
            {
                "OpenDate",
                new FilterInputViewModel
                {
                    DateRange = new DateRangeInputFilterViewModel()
                }
            }
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

        Assert.DoesNotContain(
            query.Keys,
            key => key.StartsWith("Filters[OpenDate]"));
    }

    [Fact]
    public void BuildPageUrl_WithPartialDateRange_AddsOnlyProvidedValues()
    {
        // Arrange
        SearchRequestViewModel request = new()
        {
            Filters = new Dictionary<string, FilterInputViewModel>
            {
                ["OpenDate"] = new FilterInputViewModel
                {
                    DateRange = new DateRangeInputFilterViewModel
                    {
                        From = new DateInputViewModel
                        {
                            Year = "2026"
                        }
                    }
                }
            }
        };

        // Act
        string result =
            SearchUrlBuilder.BuildPageUrl(
                _urlHelper.Object,
                request,
                1);

        Dictionary<string, StringValues> query = QueryHelpers.ParseQuery(result);

        // Assert
        Assert.False(
            query.ContainsKey(
                "Filters[OpenDate].DateRange.From.Day"));

        Assert.False(
            query.ContainsKey(
                "Filters[OpenDate].DateRange.From.Month"));

        Assert.Equal(
            "2026",
            query["Filters[OpenDate].DateRange.From.Year"]);

        Assert.False(
            query.ContainsKey(
                "Filters[OpenDate].DateRange.To.Day"));

        Assert.False(
            query.ContainsKey(
                "Filters[OpenDate].DateRange.To.Month"));

        Assert.False(
            query.ContainsKey(
                "Filters[OpenDate].DateRange.To.Year"));
    }

    [Fact]
    public void BuildPageUrl_WithNumberRangeFilter_AddsNumberRangeToQueryString()
    {
        // Arrange
        SearchRequestViewModel searchRequest = new()
        {
            RecordsPerPage = 20,
            Filters = new Dictionary<string, FilterInputViewModel>
        {
            {
                "AgeRange",
                new FilterInputViewModel
                {
                    NumberRange = new NumberRangeInputFilterViewModel
                    {
                        From = 5,
                        To = 11
                    }
                }
            }
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
            "5",
            query["Filters[AgeRange].NumberRange.From"]);

        Assert.Equal(
            "11",
            query["Filters[AgeRange].NumberRange.To"]);
    }

    [Fact]
    public void BuildPageUrl_WithNumberRangeFromOnly_AddsOnlyFromValue()
    {
        // Arrange
        SearchRequestViewModel searchRequest = new()
        {
            RecordsPerPage = 20,
            Filters = new Dictionary<string, FilterInputViewModel>
        {
            {
                "AgeRange",
                new FilterInputViewModel
                {
                    NumberRange = new NumberRangeInputFilterViewModel
                    {
                        From = 5
                    }
                }
            }
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
            "5",
            query["Filters[AgeRange].NumberRange.From"]);

        Assert.False(
            query.ContainsKey(
                "Filters[AgeRange].NumberRange.To"));
    }

    [Fact]
    public void BuildPageUrl_WithNumberRangeToOnly_AddsOnlyToValue()
    {
        // Arrange
        SearchRequestViewModel searchRequest = new()
        {
            RecordsPerPage = 20,
            Filters = new Dictionary<string, FilterInputViewModel>
        {
            {
                "AgeRange",
                new FilterInputViewModel
                {
                    NumberRange = new NumberRangeInputFilterViewModel
                    {
                        To = 11
                    }
                }
            }
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

        Assert.False(
            query.ContainsKey(
                "Filters[AgeRange].NumberRange.From"));

        Assert.Equal(
            "11",
            query["Filters[AgeRange].NumberRange.To"]);
    }

    [Fact]
    public void BuildPageUrl_WithEmptyNumberRange_DoesNotAddNumberRangeToQueryString()
    {
        // Arrange
        SearchRequestViewModel searchRequest = new()
        {
            RecordsPerPage = 20,
            Filters = new Dictionary<string, FilterInputViewModel>
        {
            {
                "AgeRange",
                new FilterInputViewModel
                {
                    NumberRange = new NumberRangeInputFilterViewModel()
                }
            }
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

        Assert.DoesNotContain(
            query.Keys,
            key => key.StartsWith(
                "Filters[AgeRange].NumberRange"));
    }
}
