using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Results;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Mappers;

public sealed class FacetResultsToViewModelMapperUnitTests
{
    [Fact]
    public void Map_GivenNullInput_ThrowsArgumentNullException()
    {
        // Arrange
        FacetResultsToViewModelMapper mapper = new();

        // Act
        Func<IReadOnlyCollection<IFilterViewModel>> map = () =>
            mapper.Map(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(map);
    }

    [Fact]
    public void Map_GivenNoFacets_ReturnsEmptyList()
    {
        // Arrange
        FacetResultsToViewModelMapper mapper = new();

        FacetResultsMappingContext context = new(
            Facets: [],
            SelectedFacets: null);

        // Act
        IReadOnlyCollection<IFilterViewModel> result =
            mapper.Map(context);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Map_GivenFacets_MapsEachFacetToCheckboxFilterViewModel()
    {
        // Arrange
        FacetResultsToViewModelMapper mapper = new();

        IReadOnlyCollection<SearchFacet> facets =
        [
            MakeFacet(
                "Phase",
                ("1", "Primary", 10)),

            MakeFacet(
                "Type",
                ("2", "Academy", 5))
        ];

        FacetResultsMappingContext context = new(
            Facets: facets,
            SelectedFacets: null);

        // Act
        List<IFilterViewModel> result = [.. mapper.Map(context)];

        // Assert
        Assert.Equal(2, result.Count);

        CheckboxFilterViewModel phase =
            Assert.IsType<CheckboxFilterViewModel>(
                result[0]);

        CheckboxFilterViewModel type =
            Assert.IsType<CheckboxFilterViewModel>(
                result[1]);

        Assert.Equal("Phase", phase.Name);
        Assert.Equal("Type", type.Name);
    }

    [Fact]
    public void Map_GivenLocalAuthorityFacet_MapsToAutocompleteFilterViewModel()
    {
        // Arrange
        FacetResultsToViewModelMapper mapper = new();

        SearchFacet facet =
            MakeFacet(
                "localauthority",
                ("201", "City of London", 10));

        FacetResultsMappingContext context = new(
            Facets: [facet],
            SelectedFacets: null);

        // Act
        IReadOnlyCollection<IFilterViewModel> result =
            mapper.Map(context);

        // Assert
        AutocompleteFilterViewModel filter =
            Assert.IsType<AutocompleteFilterViewModel>(
                Assert.Single(result));

        Assert.Equal(
            "localauthority",
            filter.Name);

        Assert.Equal(
            "Local Authority",
            filter.Label);

        FacetValueViewModel value =
            Assert.Single(filter.Values);

        Assert.Equal("201", value.Value);
        Assert.Equal("City of London", value.Label);
        Assert.Equal(10, value.Count);
        Assert.False(value.IsSelected);
    }

    [Fact]
    public void Map_GivenFacetWithNullName_ThrowsArgumentNullException()
    {
        // Arrange
        FacetResultsToViewModelMapper mapper = new();

        SearchFacet facet =
            new(null!, []);

        FacetResultsMappingContext context = new(
            Facets: [facet],
            SelectedFacets: null);

        // Act
        Func<IReadOnlyCollection<IFilterViewModel>> map = () =>
            mapper.Map(context);

        // Assert
        Assert.Throws<ArgumentNullException>(map);
    }

    [Fact]
    public void Map_GivenFacetWithNullResults_ThrowsArgumentNullException()
    {
        // Arrange
        FacetResultsToViewModelMapper mapper = new();

        SearchFacet facet =
            new("Phase", null!);

        FacetResultsMappingContext context = new(
            Facets: [facet],
            SelectedFacets: null);

        // Act
        Func<IReadOnlyCollection<IFilterViewModel>> map = () =>
            mapper.Map(context);

        // Assert
        Assert.Throws<ArgumentNullException>(map);
    }

    [Fact]
    public void Map_GivenFacetResultWithNullValue_ThrowsArgumentNullException()
    {
        // Arrange
        FacetResultsToViewModelMapper mapper = new();

        SearchFacet facet = new(
            "Phase",
            [
                new FacetResult(
                    null!,
                    "Primary",
                    5)
            ]);

        FacetResultsMappingContext context = new(
            Facets: [facet],
            SelectedFacets: null);

        // Act
        Func<IReadOnlyCollection<IFilterViewModel>> map = () =>
            mapper.Map(context);

        // Assert
        Assert.Throws<ArgumentNullException>(map);
    }

    [Fact]
    public void Map_GivenFacetResultWithNullLabel_ThrowsArgumentNullException()
    {
        // Arrange
        FacetResultsToViewModelMapper mapper = new();

        SearchFacet facet = new(
            "Phase",
            [
                new FacetResult(
                    "1",
                    null!,
                    5)
            ]);

        FacetResultsMappingContext context = new(
            Facets: [facet],
            SelectedFacets: null);

        // Act
        Func<IReadOnlyCollection<IFilterViewModel>> map = () =>
            mapper.Map(context);

        // Assert
        Assert.Throws<ArgumentNullException>(map);
    }

    [Fact]
    public void Map_GivenFacetResultWithNullCount_ThrowsArgumentException()
    {
        // Arrange
        FacetResultsToViewModelMapper mapper = new();

        SearchFacet facet = new(
            "Phase",
            [
                new FacetResult(
                    "1",
                    "Primary",
                    null)
            ]);

        FacetResultsMappingContext context = new(
            Facets: [facet],
            SelectedFacets: null);

        // Act
        Func<IReadOnlyCollection<IFilterViewModel>> map = () =>
            mapper.Map(context);

        // Assert
        Assert.Throws<ArgumentException>(map);
    }

    [Fact]
    public void Map_GivenFacetValues_MapsValuesCorrectly()
    {
        // Arrange
        FacetResultsToViewModelMapper mapper = new();

        SearchFacet facet =
            MakeFacet(
                "Phase",
                ("1", "Primary", 10),
                ("2", "Secondary", 20));

        FacetResultsMappingContext context = new(
            Facets: [facet],
            SelectedFacets: null);

        // Act
        IReadOnlyCollection<IFilterViewModel> result =
            mapper.Map(context);

        // Assert
        CheckboxFilterViewModel filter =
            Assert.IsType<CheckboxFilterViewModel>(
                Assert.Single(result));

        Assert.Equal("Phase", filter.Name);
        Assert.Equal(2, filter.Values.Count);

        Assert.Equal(
            "1",
            filter.Values[0].Value);

        Assert.Equal(
            "Primary",
            filter.Values[0].Label);

        Assert.Equal(
            10,
            filter.Values[0].Count);

        Assert.False(
            filter.Values[0].IsSelected);

        Assert.Equal(
            "2",
            filter.Values[1].Value);

        Assert.Equal(
            "Secondary",
            filter.Values[1].Label);

        Assert.Equal(
            20,
            filter.Values[1].Count);

        Assert.False(
            filter.Values[1].IsSelected);
    }

    [Fact]
    public void Map_GivenSelectedFacetValue_MarksMatchingValueAsSelected()
    {
        // Arrange
        FacetResultsToViewModelMapper mapper = new();

        SearchFacet facet =
            MakeFacet(
                "Phase",
                ("1", "Primary", 10),
                ("2", "Secondary", 20));

        Dictionary<string, List<string>> selectedFacets = new()
        {
            ["Phase"] =
            [
                "1"
            ]
        };

        FacetResultsMappingContext context = new(
            Facets: [facet],
            SelectedFacets: selectedFacets);

        // Act
        IReadOnlyCollection<IFilterViewModel> result =
            mapper.Map(context);

        // Assert
        CheckboxFilterViewModel filter =
            Assert.IsType<CheckboxFilterViewModel>(
                Assert.Single(result));

        FacetValueViewModel primary =
            Assert.Single(
                filter.Values,
                value =>
                    value.Value == "1");

        FacetValueViewModel secondary =
            Assert.Single(
                filter.Values,
                value =>
                    value.Value == "2");

        Assert.True(primary.IsSelected);
        Assert.False(secondary.IsSelected);
    }

    [Fact]
    public void Map_GivenMultipleSelectedFacetValues_MarksAllMatchingValuesAsSelected()
    {
        // Arrange
        FacetResultsToViewModelMapper mapper = new();

        SearchFacet facet =
            MakeFacet(
                "Phase",
                ("1", "Primary", 10),
                ("2", "Secondary", 20),
                ("3", "All-through", 5));

        Dictionary<string, List<string>> selectedFacets = new()
        {
            ["Phase"] =
            [
                "1",
                "3"
            ]
        };

        FacetResultsMappingContext context = new(
            Facets: [facet],
            SelectedFacets: selectedFacets);

        // Act
        IReadOnlyCollection<IFilterViewModel> result =
            mapper.Map(context);

        // Assert
        CheckboxFilterViewModel filter =
            Assert.IsType<CheckboxFilterViewModel>(
                Assert.Single(result));

        Assert.True(
            Assert.Single(
                filter.Values,
                value => value.Value == "1")
            .IsSelected);

        Assert.False(
            Assert.Single(
                filter.Values,
                value => value.Value == "2")
            .IsSelected);

        Assert.True(
            Assert.Single(
                filter.Values,
                value => value.Value == "3")
            .IsSelected);
    }

    [Fact]
    public void Map_GivenSelectedValueForDifferentFacet_LeavesValuesUnselected()
    {
        // Arrange
        FacetResultsToViewModelMapper mapper = new();

        SearchFacet facet =
            MakeFacet(
                "Phase",
                ("1", "Primary", 10),
                ("2", "Secondary", 20));

        Dictionary<string, List<string>> selectedFacets = new()
        {
            ["EstablishmentType"] =
            [
                "1"
            ]
        };

        FacetResultsMappingContext context = new(
            Facets: [facet],
            SelectedFacets: selectedFacets);

        // Act
        IReadOnlyCollection<IFilterViewModel> result =
            mapper.Map(context);

        // Assert
        CheckboxFilterViewModel filter =
            Assert.IsType<CheckboxFilterViewModel>(
                Assert.Single(result));

        Assert.All(
            filter.Values,
            value =>
                Assert.False(value.IsSelected));
    }

    [Fact]
    public void Map_GivenSelectedFacetValueWithDifferentCase_MarksMatchingValueAsSelected()
    {
        // Arrange
        FacetResultsToViewModelMapper mapper = new();

        SearchFacet facet =
            MakeFacet(
                "Phase",
                ("PRIMARY", "Primary", 10));

        Dictionary<string, List<string>> selectedFacets = new()
        {
            ["Phase"] =
            [
                "primary"
            ]
        };

        FacetResultsMappingContext context = new(
            Facets: [facet],
            SelectedFacets: selectedFacets);

        // Act
        IReadOnlyCollection<IFilterViewModel> result = mapper.Map(context);

        // Assert
        CheckboxFilterViewModel filter =
            Assert.IsType<CheckboxFilterViewModel>(
                Assert.Single(result));

        FacetValueViewModel value =
            Assert.Single(filter.Values);

        Assert.True(value.IsSelected);
    }

    private static SearchFacet MakeFacet(
        string name = "Phase",
        params (string key, string value, int count)[] results)
    {
        List<FacetResult> facetResults = [];

        foreach ((string key, string value, int count) in results)
        {
            facetResults.Add(
                new FacetResult(
                    key,
                    value,
                    count));
        }

        return new(
            facetName: name,
            facetResults: facetResults);
    }
}
