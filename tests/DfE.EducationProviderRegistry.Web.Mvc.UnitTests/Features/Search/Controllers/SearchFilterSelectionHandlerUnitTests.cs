using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Services;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Services;

public sealed class SearchFilterSelectionHandlerTests
{
    [Fact]
    public void Handle_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        // arrange
        SearchFilterSelectionHandler handler = new();

        // act/assert
        Assert.Throws<ArgumentNullException>(
            () => handler.Handle(null!));
    }

    [Fact]
    public void Handle_ClearsSelectedFacets_WhenClearFiltersIsTrue()
    {
        // arrange
        SearchFilterSelectionHandler handler = new();

        SearchRequestViewModel request = new()
        {
            ClearFilters = true,

            SelectedFacets = new Dictionary<string, List<string>>
            {
                ["EstablishmentType"] =
                [
                    "Academy",
                    "Free school"
                ],

                ["LocalAuthority"] =
                [
                    "TestLocalAuthority1"
                ]
            }
        };

        // act
        handler.Handle(request);

        // assert
        Assert.Empty(request.SelectedFacets);
    }

    [Fact]
    public void Handle_ResetsClearFilters_WhenClearFiltersIsTrue()
    {
        // arrange
        SearchFilterSelectionHandler handler = new();

        SearchRequestViewModel request = new()
        {
            ClearFilters = true,
            SelectedFacets = []
        };

        // act
        handler.Handle(request);

        // assert
        Assert.False(request.ClearFilters);
    }

    [Fact]
    public void Handle_ClearsRemoveFilter_WhenClearFiltersIsTrue()
    {
        // arrange
        SearchFilterSelectionHandler handler = new();

        SearchRequestViewModel request = new()
        {
            ClearFilters = true,

            RemoveFilter =
                "SelectedFacets[EstablishmentType]|Academy",

            SelectedFacets = new Dictionary<string, List<string>>
            {
                ["EstablishmentType"] =
                [
                    "Academy"
                ]
            }
        };

        // act
        handler.Handle(request);

        // assert
        Assert.Null(request.RemoveFilter);
    }

    [Fact]
    public void Handle_RemovesSelectedFilter()
    {
        // arrange
        SearchFilterSelectionHandler handler = new();

        SearchRequestViewModel request = new()
        {
            RemoveFilter =
                "EstablishmentType|Academy",

            SelectedFacets = new Dictionary<string, List<string>>
            {
                ["EstablishmentType"] =
                [
                    "Academy",
                    "Free school"
                ]
            }
        };

        // act
        handler.Handle(request);

        // assert
        Assert.Equal(
            ["Free school"],
            request.SelectedFacets["EstablishmentType"]);
    }

    [Fact]
    public void Handle_RemovesSelectedFilter_IgnoringCase()
    {
        // arrange
        SearchFilterSelectionHandler handler = new();

        SearchRequestViewModel request = new()
        {
            RemoveFilter =
                "EstablishmentType|academy",

            SelectedFacets = new Dictionary<string, List<string>>
            {
                ["EstablishmentType"] =
                [
                    "Academy",
                    "Free school"
                ]
            }
        };

        // act
        handler.Handle(request);

        // assert
        Assert.Equal(
            ["Free school"],
            request.SelectedFacets["EstablishmentType"]);
    }

    [Fact]
    public void Handle_RemovesFacet_WhenLastSelectedValueIsRemoved()
    {
        // arrange
        SearchFilterSelectionHandler handler = new();

        SearchRequestViewModel request = new()
        {
            RemoveFilter =
                "EstablishmentType|Academy",

            SelectedFacets = new Dictionary<string, List<string>>
            {
                ["EstablishmentType"] =
                [
                    "Academy"
                ]
            }
        };

        // act
        handler.Handle(request);

        // assert
        Assert.DoesNotContain(
            "EstablishmentType",
            request.SelectedFacets);
    }

    [Fact]
    public void Handle_ClearsRemoveFilter_AfterRemovingSelectedFilter()
    {
        // arrange
        SearchFilterSelectionHandler handler = new();

        SearchRequestViewModel request = new()
        {
            RemoveFilter =
                "EstablishmentType|Academy",

            SelectedFacets = new Dictionary<string, List<string>>
            {
                ["EstablishmentType"] =
                [
                    "Academy"
                ]
            }
        };

        // act
        handler.Handle(request);

        // assert
        Assert.Null(request.RemoveFilter);
    }

    [Fact]
    public void Handle_DoesNothing_WhenRemoveFilterIsNull()
    {
        // arrange
        SearchFilterSelectionHandler handler = new();

        SearchRequestViewModel request = new()
        {
            RemoveFilter = null,

            SelectedFacets = new Dictionary<string, List<string>>
            {
                ["EstablishmentType"] =
                [
                    "Academy"
                ]
            }
        };

        // act
        handler.Handle(request);

        // assert
        Assert.Equal(
            ["Academy"],
            request.SelectedFacets["EstablishmentType"]);

        Assert.Null(request.RemoveFilter);
    }

    [Fact]
    public void Handle_DoesNothing_WhenRemoveFilterIsEmpty()
    {
        // arrange
        SearchFilterSelectionHandler handler = new();

        SearchRequestViewModel request = new()
        {
            RemoveFilter = string.Empty,

            SelectedFacets = new Dictionary<string, List<string>>
            {
                ["EstablishmentType"] =
                [
                    "Academy"
                ]
            }
        };

        // act
        handler.Handle(request);

        // assert
        Assert.Equal(
            ["Academy"],
            request.SelectedFacets["EstablishmentType"]);
    }

    [Fact]
    public void Handle_DoesNothing_WhenRemoveFilterIsWhitespace()
    {
        // arrange
        SearchFilterSelectionHandler handler = new();

        SearchRequestViewModel request = new()
        {
            RemoveFilter = "   ",

            SelectedFacets = new Dictionary<string, List<string>>
            {
                ["EstablishmentType"] =
                [
                    "Academy"
                ]
            }
        };

        // act
        handler.Handle(request);

        // assert
        Assert.Equal(
            ["Academy"],
            request.SelectedFacets["EstablishmentType"]);
    }

    [Theory]
    [InlineData("Invalid")]
    [InlineData("Invalid|Academy")]
    [InlineData("SelectedFacets[EstablishmentType]")]
    [InlineData("SelectedFacets[]|Academy")]
    [InlineData("SelectedFacets[EstablishmentType]|")]
    [InlineData("|Academy")]
    [InlineData("WrongPrefix[EstablishmentType]|Academy")]
    [InlineData("SelectedFacets[EstablishmentType|Academy")]
    public void Handle_DoesNothing_WhenRemoveFilterFormatIsInvalid(
        string removeFilter)
    {
        // arrange
        SearchFilterSelectionHandler handler = new();

        SearchRequestViewModel request = new()
        {
            RemoveFilter = removeFilter,

            SelectedFacets = new Dictionary<string, List<string>>
            {
                ["EstablishmentType"] =
                [
                    "Academy"
                ]
            }
        };

        // act
        handler.Handle(request);

        // assert
        Assert.Equal(
            ["Academy"],
            request.SelectedFacets["EstablishmentType"]);
    }

    [Fact]
    public void Handle_DoesNothing_WhenFacetDoesNotExist()
    {
        // arrange
        SearchFilterSelectionHandler handler = new();

        SearchRequestViewModel request = new()
        {
            RemoveFilter =
                "LocalAuthority|TestLocalAuthority1",

            SelectedFacets = new Dictionary<string, List<string>>
            {
                ["EstablishmentType"] =
                [
                    "Academy"
                ]
            }
        };

        // act
        handler.Handle(request);

        // assert
        Assert.Equal(
            ["Academy"],
            request.SelectedFacets["EstablishmentType"]);
    }

    [Fact]
    public void Handle_DoesNotRemoveAnything_WhenValueDoesNotExist()
    {
        // arrange
        SearchFilterSelectionHandler handler = new();

        SearchRequestViewModel request = new()
        {
            RemoveFilter =
                "EstablishmentType|Free school",

            SelectedFacets = new Dictionary<string, List<string>>
            {
                ["EstablishmentType"] =
                [
                    "Academy"
                ]
            }
        };

        // act
        handler.Handle(request);

        // assert
        Assert.Equal(
            ["Academy"],
            request.SelectedFacets["EstablishmentType"]);

        Assert.Null(request.RemoveFilter);
    }

    [Fact]
    public void Handle_TrimsBindingNameAndValue()
    {
        // arrange
        SearchFilterSelectionHandler handler = new();

        SearchRequestViewModel request = new()
        {
            RemoveFilter =
                " EstablishmentType | Academy ",

            SelectedFacets = new Dictionary<string, List<string>>
            {
                ["EstablishmentType"] =
                [
                    "Academy",
                    "Free school"
                ]
            }
        };

        // act
        handler.Handle(request);

        // assert
        Assert.Equal(
            ["Free school"],
            request.SelectedFacets["EstablishmentType"]);
    }

    [Fact]
    public void Handle_StopsAfterClearingFilters()
    {
        // arrange
        SearchFilterSelectionHandler handler = new();

        SearchRequestViewModel request = new()
        {
            ClearFilters = true,

            RemoveFilter =
                "EstablishmentType|Academy",

            SelectedFacets = new Dictionary<string, List<string>>
            {
                ["EstablishmentType"] =
                [
                    "Academy",
                    "Free school"
                ]
            }
        };

        // act
        handler.Handle(request);

        // assert
        Assert.Empty(request.SelectedFacets);
        Assert.False(request.ClearFilters);
        Assert.Null(request.RemoveFilter);
    }
}