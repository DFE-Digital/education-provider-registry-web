using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.FilterViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Controllers.TestDoubles;
using System.Collections.ObjectModel;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Mappers;

public sealed class SearchResponseToSearchFiltersViewModelMapperTests
{
    private static EstablishmentSearchResult MakeResult(
        string urn = "111111",
        string name = "Test School",
        string establishmentType = "Academy",
        string localAuthorityName = "Local Authority",
        string localAuthorityCode = "123")
    {
        UniqueReferenceNumber urnVo = new(urn);
        Name nameVo = new(name);

        Address addressVo = new(
            Street: "Street",
            Town: "Town",
            County: "County",
            Postcode: "AB1 2CD");

        EstablishmentType typeVo = new(establishmentType);

        GroupDetail groupVo = new(
            partOfName: "Group Name",
            partOfCode: "G123");

        LocalAuthority localAuthorityVo = new(
            localAuthorityName: localAuthorityName,
            localAuthorityCode: localAuthorityCode);

        return new EstablishmentSearchResult(
            urnVo,
            nameVo,
            addressVo,
            typeVo,
            groupVo,
            localAuthorityVo);
    }

    private static SearchFiltersMappingContext MakeContext(
        IReadOnlyCollection<EstablishmentSearchResult>? results = null,
        Dictionary<string, List<string>>? selectedFacets = null,
        ReadOnlyCollection<FilterRequest>? filterRequests = null)
    {
        EstablishmentSearchResults establishmentResults =
            new(results ?? []);

        SearchFacets searchFacets = SearchFacetsStub.Empty();

        UseCaseResponse<SearchResponse> searchResponse =
            UseCaseResponseSearchResponseTestDouble.Success(
                establishmentResults,
                searchFacets);

        SearchRequestViewModel searchRequest = new()
        {
            SelectedFacets = selectedFacets ?? []
        };

        return new SearchFiltersMappingContext(
            filterRequests ?? new ReadOnlyCollection<FilterRequest>([]),
            searchRequest,
            searchResponse);
    }

    [Fact]
    public void Map_ThrowsArgumentNullException_WhenInputIsNull()
    {
        // arrange
        SearchResponseToSearchFiltersViewModelMapper mapper = new();

        // act/assert
        Assert.Throws<ArgumentNullException>(() => mapper.Map(null!));
    }

    [Fact]
    public void Map_ReturnsLocalAuthorityAndEstablishmentTypeFilters()
    {
        // arrange
        SearchResponseToSearchFiltersViewModelMapper mapper = new();

        SearchFiltersMappingContext context =
            MakeContext(
                results:
                [
                    MakeResult()
                ]);

        // act
        SearchFiltersViewModel result = mapper.Map(context);

        // assert
        Assert.Equal(2, result.Filters.Count);

        Assert.Contains(
            result.Filters,
            filter => filter is AutocompleteFilterViewModel
            {
                Name: "LocalAuthority"
            });

        Assert.Contains(
            result.Filters,
            filter => filter is CheckboxFilterViewModel
            {
                Name: "EstablishmentType"
            });
    }

    [Fact]
    public void Map_ConfiguresLocalAuthorityFilterCorrectly()
    {
        // arrange
        SearchResponseToSearchFiltersViewModelMapper mapper = new();

        SearchFiltersMappingContext context =
            MakeContext(
                results:
                [
                    MakeResult(
                        localAuthorityName: "TestLocalAuthority")
                ]);

        // act
        SearchFiltersViewModel result = mapper.Map(context);

        // assert
        AutocompleteFilterViewModel filter =
            Assert.IsType<AutocompleteFilterViewModel>(
                result.Filters.Single(
                    filter => filter.Name == "LocalAuthority"));

        Assert.Equal(
            "SelectedFacets[LocalAuthority]",
            filter.BindingName);

        Assert.Equal(
            "Local authority",
            filter.Label);

        Assert.Equal(
            "Start typing a local authority name",
            filter.Hint);
    }

    [Fact]
    public void Map_AddsDistinctLocalAuthorityOptions_InAlphabeticalOrder()
    {
        // arrange
        SearchResponseToSearchFiltersViewModelMapper mapper = new();

        SearchFiltersMappingContext context =
            MakeContext(
                results:
                [
                    MakeResult(
                        urn: "111111",
                        localAuthorityName: "TestLocalAuthority1"),

                    MakeResult(
                        urn: "222222",
                        localAuthorityName: "TestLocalAuthority2"),

                    MakeResult(
                        urn: "333333",
                        localAuthorityName: "testlocalauthority2")
                ]);

        // act
        SearchFiltersViewModel result = mapper.Map(context);

        // assert
        AutocompleteFilterViewModel filter =
            Assert.IsType<AutocompleteFilterViewModel>(
                result.Filters.Single(
                    filter => filter.Name == "LocalAuthority"));

        Assert.Collection(
            filter.Options,
            option =>
            {
                Assert.Equal("TestLocalAuthority1", option.Text);
                Assert.Equal("TestLocalAuthority1", option.Value);
            },
            option =>
            {
                Assert.Equal("TestLocalAuthority2", option.Text);
                Assert.Equal("TestLocalAuthority2", option.Value);
            });
    }

    [Fact]
    public void Map_SetsSelectedLocalAuthority_FromFilterRequests()
    {
        // arrange
        SearchResponseToSearchFiltersViewModelMapper mapper = new();

        ReadOnlyCollection<FilterRequest> filterRequests =
            new(
            [
                new FilterRequest(
                    "LocalAuthority",
                    ["TestLocalAuthority1"])
            ]);

        SearchFiltersMappingContext context =
            MakeContext(
                results:
                [
                    MakeResult(
                        localAuthorityName: "TestLocalAuthority1")
                ],
                filterRequests: filterRequests);

        // act
        SearchFiltersViewModel result = mapper.Map(context);

        // assert
        AutocompleteFilterViewModel filter =
            Assert.IsType<AutocompleteFilterViewModel>(
                result.Filters.Single(
                    filter => filter.Name == "LocalAuthority"));

        Assert.Equal("TestLocalAuthority1", filter.SelectedValue);
    }

    [Fact]
    public void Map_MatchesLocalAuthorityFilterName_IgnoringCase()
    {
        // arrange
        SearchResponseToSearchFiltersViewModelMapper mapper = new();

        ReadOnlyCollection<FilterRequest> filterRequests =
            new(
            [
                new FilterRequest(
                    "localauthority",
                    ["TestLocalAuthority1"])
            ]);

        SearchFiltersMappingContext context =
            MakeContext(
                results:
                [
                    MakeResult(
                        localAuthorityName: "TestLocalAuthority1")
                ],
                filterRequests: filterRequests);

        // act
        SearchFiltersViewModel result = mapper.Map(context);

        // assert
        AutocompleteFilterViewModel filter =
            Assert.IsType<AutocompleteFilterViewModel>(
                result.Filters.Single(
                    filter => filter.Name == "LocalAuthority"));

        Assert.Equal("TestLocalAuthority1", filter.SelectedValue);
    }

    [Fact]
    public void Map_ConfiguresEstablishmentTypeFilterCorrectly()
    {
        // arrange
        SearchResponseToSearchFiltersViewModelMapper mapper = new();

        SearchFiltersMappingContext context =
            MakeContext(
                results:
                [
                    MakeResult()
                ]);

        // act
        SearchFiltersViewModel result = mapper.Map(context);

        // assert
        CheckboxFilterViewModel filter =
            Assert.IsType<CheckboxFilterViewModel>(
                result.Filters.Single(
                    filter => filter.Name == "EstablishmentType"));

        Assert.Equal(
            "SelectedFacets[EstablishmentType]",
            filter.BindingName);

        Assert.Equal(
            "Establishment type",
            filter.Label);

        Assert.Equal(
            "EstablishmentType",
            filter.Facet.Name);
    }

    [Fact]
    public void Map_GroupsEstablishmentTypes_AndSetsCounts()
    {
        // arrange
        SearchResponseToSearchFiltersViewModelMapper mapper = new();

        SearchFiltersMappingContext context =
            MakeContext(
                results:
                [
                    MakeResult(
                        urn: "111111",
                        establishmentType: "Academy"),

                    MakeResult(
                        urn: "222222",
                        establishmentType: "Academy"),

                    MakeResult(
                        urn: "333333",
                        establishmentType: "Free school")
                ]);

        // act
        SearchFiltersViewModel result = mapper.Map(context);

        // assert
        CheckboxFilterViewModel filter =
            Assert.IsType<CheckboxFilterViewModel>(
                result.Filters.Single(
                    filter => filter.Name == "EstablishmentType"));

        Assert.Collection(
            filter.Facet.Values,
            academy =>
            {
                Assert.Equal("Academy", academy.Value);
                Assert.Equal(2, academy.Count);
            },
            freeSchool =>
            {
                Assert.Equal("Free school", freeSchool.Value);
                Assert.Equal(1, freeSchool.Count);
            });
    }

    [Fact]
    public void Map_GroupsEstablishmentTypes_IgnoringCase()
    {
        // arrange
        SearchResponseToSearchFiltersViewModelMapper mapper = new();

        SearchFiltersMappingContext context =
            MakeContext(
                results:
                [
                    MakeResult(
                        urn: "111111",
                        establishmentType: "Academy"),

                    MakeResult(
                        urn: "222222",
                        establishmentType: "academy")
                ]);

        // act
        SearchFiltersViewModel result = mapper.Map(context);

        // assert
        CheckboxFilterViewModel filter =
            Assert.IsType<CheckboxFilterViewModel>(
                result.Filters.Single(
                    filter => filter.Name == "EstablishmentType"));

        FacetValueViewModel facetValue =
            Assert.Single(filter.Facet.Values);

        Assert.Equal("Academy", facetValue.Value);
        Assert.Equal(2, facetValue.Count);
    }

    [Fact]
    public void Map_MarksSelectedEstablishmentType()
    {
        // arrange
        SearchResponseToSearchFiltersViewModelMapper mapper = new();

        Dictionary<string, List<string>> selectedFacets = new()
        {
            ["EstablishmentType"] =
            [
                "Academy"
            ]
        };

        SearchFiltersMappingContext context =
            MakeContext(
                results:
                [
                    MakeResult(
                        establishmentType: "Academy"),

                    MakeResult(
                        urn: "222222",
                        establishmentType: "Free school")
                ],
                selectedFacets: selectedFacets);

        // act
        SearchFiltersViewModel result = mapper.Map(context);

        // assert
        CheckboxFilterViewModel filter =
            Assert.IsType<CheckboxFilterViewModel>(
                result.Filters.Single(
                    filter => filter.Name == "EstablishmentType"));

        FacetValueViewModel academy =
            filter.Facet.Values.Single(
                value => value.Value == "Academy");

        FacetValueViewModel freeSchool =
            filter.Facet.Values.Single(
                value => value.Value == "Free school");

        Assert.True(academy.IsSelected);
        Assert.False(freeSchool.IsSelected);
    }

    [Fact]
    public void Map_MatchesSelectedEstablishmentType_IgnoringCase()
    {
        // arrange
        SearchResponseToSearchFiltersViewModelMapper mapper = new();

        Dictionary<string, List<string>> selectedFacets = new()
        {
            ["EstablishmentType"] =
            [
                "academy"
            ]
        };

        SearchFiltersMappingContext context =
            MakeContext(
                results:
                [
                    MakeResult(
                        establishmentType: "Academy")
                ],
                selectedFacets: selectedFacets);

        // act
        SearchFiltersViewModel result = mapper.Map(context);

        // assert
        CheckboxFilterViewModel filter =
            Assert.IsType<CheckboxFilterViewModel>(
                result.Filters.Single(
                    filter => filter.Name == "EstablishmentType"));

        FacetValueViewModel academy =
            Assert.Single(filter.Facet.Values);

        Assert.True(academy.IsSelected);
    }

    [Fact]
    public void Map_AddsSelectedEstablishmentType_ToSelectedFilters()
    {
        // arrange
        SearchResponseToSearchFiltersViewModelMapper mapper = new();

        Dictionary<string, List<string>> selectedFacets = new()
        {
            ["EstablishmentType"] =
            [
                "Academy"
            ]
        };

        SearchFiltersMappingContext context =
            MakeContext(
                results:
                [
                    MakeResult(
                        establishmentType: "Academy")
                ],
                selectedFacets: selectedFacets);

        // act
        SearchFiltersViewModel result = mapper.Map(context);

        // assert
        SelectedFilterViewModel selectedFilter =
            Assert.Single(result.SelectedFilters);

        Assert.Equal("Academy", selectedFilter.Label);
        Assert.Equal(
            "SelectedFacets[EstablishmentType]",
            selectedFilter.BindingName);
        Assert.Equal("Academy", selectedFilter.Value);
    }

    [Fact]
    public void Map_AddsSelectedLocalAuthority_ToSelectedFilters()
    {
        // arrange
        SearchResponseToSearchFiltersViewModelMapper mapper = new();

        ReadOnlyCollection<FilterRequest> filterRequests =
            new(
            [
                new FilterRequest(
                    "LocalAuthority",
                    ["TestLocalAuthority1"])
            ]);

        SearchFiltersMappingContext context =
            MakeContext(
                results:
                [
                    MakeResult(
                        localAuthorityName: "TestLocalAuthority1")
                ],
                filterRequests: filterRequests);

        // act
        SearchFiltersViewModel result = mapper.Map(context);

        // assert
        SelectedFilterViewModel selectedFilter =
            Assert.Single(result.SelectedFilters);

        Assert.Equal("TestLocalAuthority1", selectedFilter.Label);
        Assert.Equal(
            "SelectedFacets[LocalAuthority]",
            selectedFilter.BindingName);
        Assert.Equal("TestLocalAuthority1", selectedFilter.Value);
    }

    [Fact]
    public void Map_ReturnsNoSelectedFilters_WhenNothingIsSelected()
    {
        // arrange
        SearchResponseToSearchFiltersViewModelMapper mapper = new();

        SearchFiltersMappingContext context =
            MakeContext(
                results:
                [
                    MakeResult()
                ]);

        // act
        SearchFiltersViewModel result = mapper.Map(context);

        // assert
        Assert.Empty(result.SelectedFilters);
    }
}