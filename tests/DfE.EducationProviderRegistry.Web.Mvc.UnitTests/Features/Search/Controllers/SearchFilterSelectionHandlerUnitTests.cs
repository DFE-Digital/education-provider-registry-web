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

            SelectedFacets = new Dictionary<string, List<SelectedFacetValueViewModel>>
            {
                ["EstablishmentType"] =
                [
                    new SelectedFacetValueViewModel("1", "Academy"),
                    new SelectedFacetValueViewModel("2", "Free school")
                ],

                ["LocalAuthority"] =
                [
                    new SelectedFacetValueViewModel("3", "TestLocalAuthority1")
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
                "SelectedFacets[EstablishmentType]|1",

            SelectedFacets = new Dictionary<string, List<SelectedFacetValueViewModel>>
            {
                ["EstablishmentType"] =
                [
                    new SelectedFacetValueViewModel("1", "Academy")
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
                "EstablishmentType|1",

            SelectedFacets = new Dictionary<string, List<SelectedFacetValueViewModel>>
            {
                ["EstablishmentType"] =
                [
                    new SelectedFacetValueViewModel("1", "Academy"),
                    new SelectedFacetValueViewModel("2", "Free school")
                ]
            }
        };

        // act
        handler.Handle(request);

        // assert
        Assert.Collection(
            request.SelectedFacets["EstablishmentType"],
            selected =>
            {
                Assert.Equal("2", selected.Key);
                Assert.Equal("Free school", selected.Value);
            });
    }

    [Fact]
    public void Handle_RemovesSelectedFilter_IgnoringCase()
    {
        // arrange
        SearchFilterSelectionHandler handler = new();

        SearchRequestViewModel request = new()
        {
            RemoveFilter =
                "EstablishmentType|academy-key",

            SelectedFacets = new Dictionary<string, List<SelectedFacetValueViewModel>>
            {
                ["EstablishmentType"] =
                [
                    new SelectedFacetValueViewModel("ACADEMY-KEY", "Academy"),
                    new SelectedFacetValueViewModel("2", "Free school")
                ]
            }
        };

        // act
        handler.Handle(request);

        // assert
        Assert.Collection(
            request.SelectedFacets["EstablishmentType"],
            selected =>
            {
                Assert.Equal("2", selected.Key);
                Assert.Equal("Free school", selected.Value);
            });
    }

    [Fact]
    public void Handle_RemovesFacet_WhenLastSelectedValueIsRemoved()
    {
        // arrange
        SearchFilterSelectionHandler handler = new();

        SearchRequestViewModel request = new()
        {
            RemoveFilter =
                "EstablishmentType|1",

            SelectedFacets = new Dictionary<string, List<SelectedFacetValueViewModel>>
            {
                ["EstablishmentType"] =
                [
                    new SelectedFacetValueViewModel("1", "Academy")
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
                "EstablishmentType|1",

            SelectedFacets = new Dictionary<string, List<SelectedFacetValueViewModel>>
            {
                ["EstablishmentType"] =
                [
                    new SelectedFacetValueViewModel("1", "Academy")
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

            SelectedFacets = new Dictionary<string, List<SelectedFacetValueViewModel>>
            {
                ["EstablishmentType"] =
                [
                    new SelectedFacetValueViewModel("1", "Academy")
                ]
            }
        };

        // act
        handler.Handle(request);

        // assert
        Assert.Collection(
            request.SelectedFacets["EstablishmentType"],
            selected =>
            {
                Assert.Equal("1", selected.Key);
                Assert.Equal("Academy", selected.Value);
            });

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

            SelectedFacets = new Dictionary<string, List<SelectedFacetValueViewModel>>
            {
                ["EstablishmentType"] =
                [
                    new SelectedFacetValueViewModel("1", "Academy")
                ]
            }
        };

        // act
        handler.Handle(request);

        // assert
        Assert.Collection(
            request.SelectedFacets["EstablishmentType"],
            selected =>
            {
                Assert.Equal("1", selected.Key);
                Assert.Equal("Academy", selected.Value);
            });
    }

    [Fact]
    public void Handle_DoesNothing_WhenRemoveFilterIsWhitespace()
    {
        // arrange
        SearchFilterSelectionHandler handler = new();

        SearchRequestViewModel request = new()
        {
            RemoveFilter = "   ",

            SelectedFacets = new Dictionary<string, List<SelectedFacetValueViewModel>>
            {
                ["EstablishmentType"] =
                [
                    new SelectedFacetValueViewModel("1", "Academy")
                ]
            }
        };

        // act
        handler.Handle(request);

        // assert
        Assert.Collection(
            request.SelectedFacets["EstablishmentType"],
            selected =>
            {
                Assert.Equal("1", selected.Key);
                Assert.Equal("Academy", selected.Value);
            });
    }

    [Theory]
    [InlineData("Invalid")]
    [InlineData("Invalid|1")]
    [InlineData("SelectedFacets[EstablishmentType]")]
    [InlineData("SelectedFacets[]|1")]
    [InlineData("SelectedFacets[EstablishmentType]|")]
    [InlineData("|1")]
    [InlineData("WrongPrefix[EstablishmentType]|1")]
    [InlineData("SelectedFacets[EstablishmentType|1")]
    public void Handle_DoesNothing_WhenRemoveFilterFormatIsInvalid(
        string removeFilter)
    {
        // arrange
        SearchFilterSelectionHandler handler = new();

        SearchRequestViewModel request = new()
        {
            RemoveFilter = removeFilter,

            SelectedFacets = new Dictionary<string, List<SelectedFacetValueViewModel>>
            {
                ["EstablishmentType"] =
                [
                    new SelectedFacetValueViewModel("1", "Academy")
                ]
            }
        };

        // act
        handler.Handle(request);

        // assert
        Assert.Collection(
            request.SelectedFacets["EstablishmentType"],
            selected =>
            {
                Assert.Equal("1", selected.Key);
                Assert.Equal("Academy", selected.Value);
            });
    }

    [Fact]
    public void Handle_DoesNothing_WhenFacetDoesNotExist()
    {
        // arrange
        SearchFilterSelectionHandler handler = new();

        SearchRequestViewModel request = new()
        {
            RemoveFilter =
                "LocalAuthority|3",

            SelectedFacets = new Dictionary<string, List<SelectedFacetValueViewModel>>
            {
                ["EstablishmentType"] =
                [
                    new SelectedFacetValueViewModel("1", "Academy")
                ]
            }
        };

        // act
        handler.Handle(request);

        // assert
        Assert.Collection(
            request.SelectedFacets["EstablishmentType"],
            selected =>
            {
                Assert.Equal("1", selected.Key);
                Assert.Equal("Academy", selected.Value);
            });
    }

    [Fact]
    public void Handle_DoesNotRemoveAnything_WhenValueDoesNotExist()
    {
        // arrange
        SearchFilterSelectionHandler handler = new();

        SearchRequestViewModel request = new()
        {
            RemoveFilter =
                "EstablishmentType|2",

            SelectedFacets = new Dictionary<string, List<SelectedFacetValueViewModel>>
            {
                ["EstablishmentType"] =
                [
                    new SelectedFacetValueViewModel("1", "Academy")
                ]
            }
        };

        // act
        handler.Handle(request);

        // assert
        Assert.Collection(
            request.SelectedFacets["EstablishmentType"],
            selected =>
            {
                Assert.Equal("1", selected.Key);
                Assert.Equal("Academy", selected.Value);
            });

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
                " EstablishmentType | 1 ",

            SelectedFacets = new Dictionary<string, List<SelectedFacetValueViewModel>>
            {
                ["EstablishmentType"] =
                [
                    new SelectedFacetValueViewModel("1", "Academy"),
                    new SelectedFacetValueViewModel("2", "Free school")
                ]
            }
        };

        // act
        handler.Handle(request);

        // assert
        Assert.Collection(
            request.SelectedFacets["EstablishmentType"],
            selected =>
            {
                Assert.Equal("2", selected.Key);
                Assert.Equal("Free school", selected.Value);
            });
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
                "EstablishmentType|1",

            SelectedFacets = new Dictionary<string, List<SelectedFacetValueViewModel>>
            {
                ["EstablishmentType"] =
                [
                    new SelectedFacetValueViewModel("1", "Academy"),
                    new SelectedFacetValueViewModel("2", "Free school")
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