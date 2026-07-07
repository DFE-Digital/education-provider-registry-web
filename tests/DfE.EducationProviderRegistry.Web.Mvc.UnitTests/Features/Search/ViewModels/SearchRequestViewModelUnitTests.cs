using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.ViewModels;

public sealed class SearchRequestViewModelUnitTests
{
    [Fact]
    public void SelectedFacets_ReturnsValue_WhenClearFiltersIsFalse()
    {
        // arrange
        SearchRequestViewModel vm = new()
        {
            SelectedFacets = new Dictionary<string, List<string>>
            {
                { "Phase", new List<string> { "Primary" } }
            },
            ClearFilters = false
        };

        // act
        var result = vm.SelectedFacets;

        // assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Primary", result["Phase"][0]);
    }

    [Fact]
    public void SelectedFacets_ReturnsNull_WhenClearFiltersIsTrue()
    {
        // arrange
        SearchRequestViewModel vm = new()
        {
            SelectedFacets = new Dictionary<string, List<string>>
            {
                { "Phase", new List<string> { "Primary" } }
            },
            ClearFilters = true
        };

        // act
        var result = vm.SelectedFacets;

        // assert
        Assert.Null(result);
    }

    [Fact]
    public void SelectedFacets_BackFieldIsReset_WhenClearFiltersIsTrue()
    {
        // arrange
        SearchRequestViewModel vm = new()
        {
            SelectedFacets = new Dictionary<string, List<string>>
            {
                { "Phase", new List<string> { "Primary" } }
            },

            // act
            ClearFilters = true
        };
        _ = vm.SelectedFacets; // triggers the getter logic

        // assert
        Assert.Null(vm.SelectedFacets);
    }

    [Fact]
    public void SearchKeywords_CanBeSetAndRetrieved()
    {
        // arrange
        SearchRequestViewModel vm = new()
        {
            // act
            SearchKeywords = "academy"
        };

        // assert
        Assert.Equal("academy", vm.SearchKeywords);
    }

    [Fact]
    public void RecordsPerPage_CanBeSetAndRetrieved()
    {
        // arrange
        SearchRequestViewModel vm = new()
        {
            // act
            RecordsPerPage = 25
        };

        // assert
        Assert.Equal(25, vm.RecordsPerPage);
    }

    [Fact]
    public void PageNumber_DefaultsToOne()
    {
        // arrange
        SearchRequestViewModel vm = new();

        // act
        int page = vm.PageNumber;

        // assert
        Assert.Equal(1, page);
    }

    [Fact]
    public void Offset_IsZero_WhenPageNumberIsOne()
    {
        // arrange
        SearchRequestViewModel vm = new()
        {
            RecordsPerPage = 20,
            PageNumber = 1
        };

        // act
        int offset = vm.Offset;

        // assert
        Assert.Equal(0, offset);
    }

    [Fact]
    public void Offset_ComputesCorrectly_ForSubsequentPages()
    {
        // arrange
        SearchRequestViewModel vm = new()
        {
            RecordsPerPage = 20,
            PageNumber = 3
        };

        // act
        int offset = vm.Offset;

        // assert
        Assert.Equal(40, offset); // (3 - 1) * 20
    }

    [Fact]
    public void Offset_Updates_WhenPageNumberChanges()
    {
        // arrange
        SearchRequestViewModel vm = new()
        {
            RecordsPerPage = 10,

            // act
            PageNumber = 5
        };
        int offset = vm.Offset;

        // assert
        Assert.Equal(40, offset); // (5 - 1) * 10
    }
}
