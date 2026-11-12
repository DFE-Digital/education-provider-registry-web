using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Request;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Results;
using DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Mappers.TestDoubles;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Mappers;

public sealed class SearchResultsToViewModelMapperTests
{
    [Fact]
    public void Map_ThrowsArgumentNullException_WhenInputIsNull()
    {
        // Arrange
        Mock<IMapper<
            IReadOnlyCollection<SearchAggregateResult>,
            List<GovUkTable>>> searchAggregateMapper =
                new();

        Mock<IMapper<
            FacetResultsMappingContext,
            IReadOnlyCollection<IFilterViewModel>>> facetsMapper =
                CreateFacetsMapper();

        Mock<IMapper<
            AvailableFiltersMappingContext,
            IReadOnlyCollection<IFilterViewModel>>> availableFiltersMapper =
                CreateAvailableFiltersMapper();

        SearchResultsToViewModelMapper mapper =
            CreateSut(
                searchAggregateMapper,
                facetsMapper,
                availableFiltersMapper);

        // Act
        Action map = () =>
            mapper.Map(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(map);
    }

    [Fact]
    public void Map_MapsEstablishmentResults_WhenPresent()
    {
        // Arrange
        List<SearchAggregateResult> searchAggregateResults =
        [
            MakeSearchAggregate()
        ];

        List<GovUkTable> searchAggregateTables =
        [
            new(
                [new TableColumn { Text = "testColumn" }],
                [
                    new TableRow
                    {
                        Cells =
                        [
                            new TableCell
                            {
                                Text = "Test Cell"
                            }
                        ]
                    }
                ],
                "School A")
        ];

        Mock<IMapper<
            IReadOnlyCollection<SearchAggregateResult>,
            List<GovUkTable>>> searchAggregateMapper =
                SearchAggregatesMapperTestDouble.MockFor(
                    searchAggregateResults,
                    searchAggregateTables);

        Mock<IMapper<
            FacetResultsMappingContext,
            IReadOnlyCollection<IFilterViewModel>>> facetsMapper =
                CreateFacetsMapper();

        Mock<IMapper<
            AvailableFiltersMappingContext,
            IReadOnlyCollection<IFilterViewModel>>> availableFiltersMapper =
                CreateAvailableFiltersMapper();

        SearchResultsToViewModelMapper mapper =
            CreateSut(
                searchAggregateMapper,
                facetsMapper,
                availableFiltersMapper);

        SearchResponse response = new(
            new SearchAggregateResults(
                searchAggregateResults),
            null,
            1,
            []);

        SearchResultsMappingContext input = new(
            new SearchRequestViewModel(),
            UseCaseResponse<SearchResponse>.Success(
                response));

        // Act
        SearchResultsViewModel result =
            mapper.Map(input);

        // Assert
        Assert.Single(
            result.SearchAggregateResults);

        Assert.Equal(
            "School A",
            result.SearchAggregateResults[0].Caption);

        Assert.Equal(
            "testColumn",
            result.SearchAggregateResults[0]
                .Columns[0]
                .Text);

        Assert.Equal(
            "Test Cell",
            result.SearchAggregateResults[0]
                .Rows[0]
                .Cells[0]
                .Text);

        searchAggregateMapper.Verify(
            mapper =>
                mapper.Map(searchAggregateResults),
            Times.Once);
    }

    [Fact]
    public void Map_MapsFacetResults_WhenPresent()
    {
        // Arrange
        List<SearchFacet> facets =
        [
            MakeFacet()
        ];

        IReadOnlyCollection<IFilterViewModel> facetFilters =
        [
            new CheckboxFilterViewModel(
                "Phase",
                "Phase",
                [])
        ];

        Mock<IMapper<
            IReadOnlyCollection<SearchAggregateResult>,
            List<GovUkTable>>> searchAggregateMapper =
                new();

        Mock<IMapper<
            FacetResultsMappingContext,
            IReadOnlyCollection<IFilterViewModel>>> facetsMapper =
                CreateFacetsMapper();

        Mock<IMapper<
            AvailableFiltersMappingContext,
            IReadOnlyCollection<IFilterViewModel>>> availableFiltersMapper =
                CreateAvailableFiltersMapper();

        FacetResultsMappingContext? capturedContext =
            null;

        facetsMapper
            .Setup(mapper =>
                mapper.Map(
                    It.IsAny<FacetResultsMappingContext>()))
            .Callback<FacetResultsMappingContext>(
                context =>
                    capturedContext = context)
            .Returns(facetFilters);

        SearchResultsToViewModelMapper mapper =
            CreateSut(
                searchAggregateMapper,
                facetsMapper,
                availableFiltersMapper);

        SearchRequestViewModel searchRequest =
            new();

        SearchResponse response = new(
            null!,
            new SearchFacets(facets),
            1,
            []);

        SearchResultsMappingContext input = new(
            searchRequest,
            UseCaseResponse<SearchResponse>.Success(
                response));

        // Act
        SearchResultsViewModel result =
            mapper.Map(input);

        // Assert
        IFilterViewModel filter =
            Assert.Single(result.Filters);

        CheckboxFilterViewModel checkboxFilter =
            Assert.IsType<CheckboxFilterViewModel>(
                filter);

        Assert.Equal(
            "Phase",
            checkboxFilter.Name);

        Assert.NotNull(capturedContext);

        Assert.Equal(
            facets,
            capturedContext.Facets);

        Assert.Equal(
            searchRequest.SelectedFacets,
            capturedContext.SelectedFacets);

        facetsMapper.Verify(
            mapper =>
                mapper.Map(
                    It.IsAny<FacetResultsMappingContext>()),
            Times.Once);
    }

    [Fact]
    public void Map_MapsAvailableFilters_WhenPresent()
    {
        // Arrange
        List<string> availableFilters =
        [
            "opened_date",
            "closed_date",
            "age_range"
        ];

        IReadOnlyCollection<IFilterViewModel> mappedFilters =
        [
            new DateRangeFilterViewModel(
                "opened_date",
                "Opened date",
                new DateRangeInputFilterViewModel()),

            new DateRangeFilterViewModel(
                "closed_date",
                "Closed date",
                new DateRangeInputFilterViewModel()),

            new NumberRangeFilterViewModel(
                "age_range",
                "Age range",
                new NumberRangeInputFilterViewModel())
        ];

        Mock<IMapper<
            IReadOnlyCollection<SearchAggregateResult>,
            List<GovUkTable>>> searchAggregateMapper =
                new();

        Mock<IMapper<
            FacetResultsMappingContext,
            IReadOnlyCollection<IFilterViewModel>>> facetsMapper =
                CreateFacetsMapper();

        Mock<IMapper<
            AvailableFiltersMappingContext,
            IReadOnlyCollection<IFilterViewModel>>> availableFiltersMapper =
                CreateAvailableFiltersMapper();

        AvailableFiltersMappingContext? capturedContext =
            null;

        availableFiltersMapper
            .Setup(mapper =>
                mapper.Map(
                    It.IsAny<AvailableFiltersMappingContext>()))
            .Callback<AvailableFiltersMappingContext>(
                context =>
                    capturedContext = context)
            .Returns(mappedFilters);

        SearchResultsToViewModelMapper mapper =
            CreateSut(
                searchAggregateMapper,
                facetsMapper,
                availableFiltersMapper);

        SearchRequestViewModel searchRequest =
            new();

        SearchResponse response = new(
            null!,
            null,
            0,
            availableFilters);

        SearchResultsMappingContext input = new(
            searchRequest,
            UseCaseResponse<SearchResponse>.Success(
                response));

        // Act
        SearchResultsViewModel result =
            mapper.Map(input);

        // Assert
        Assert.Equal(
            3,
            result.Filters.Count);

        Assert.Equal(
            mappedFilters,
            result.Filters);

        Assert.NotNull(capturedContext);

        Assert.Equal(
            availableFilters,
            capturedContext.AvailableFilters);

        Assert.Same(
            searchRequest,
            capturedContext.SearchRequest);

        availableFiltersMapper.Verify(
            mapper =>
                mapper.Map(
                    It.IsAny<AvailableFiltersMappingContext>()),
            Times.Once);
    }

    [Fact]
    public void Map_CombinesFacetAndAvailableFilters()
    {
        // Arrange
        List<SearchFacet> facets =
        [
            MakeFacet()
        ];

        IReadOnlyCollection<IFilterViewModel> facetFilters =
        [
            new CheckboxFilterViewModel(
                "Phase",
                "Phase",
                [])
        ];

        IReadOnlyCollection<IFilterViewModel> availableFilters =
        [
            new NumberRangeFilterViewModel(
                "age_range",
                "Age range",
                new NumberRangeInputFilterViewModel())
        ];

        Mock<IMapper<
            IReadOnlyCollection<SearchAggregateResult>,
            List<GovUkTable>>> searchAggregateMapper =
                new();

        Mock<IMapper<
            FacetResultsMappingContext,
            IReadOnlyCollection<IFilterViewModel>>> facetsMapper =
                CreateFacetsMapper();

        Mock<IMapper<
            AvailableFiltersMappingContext,
            IReadOnlyCollection<IFilterViewModel>>> availableFiltersMapper =
                CreateAvailableFiltersMapper();

        facetsMapper
            .Setup(mapper =>
                mapper.Map(
                    It.IsAny<FacetResultsMappingContext>()))
            .Returns(facetFilters);

        availableFiltersMapper
            .Setup(mapper =>
                mapper.Map(
                    It.IsAny<AvailableFiltersMappingContext>()))
            .Returns(availableFilters);

        SearchResultsToViewModelMapper mapper =
            CreateSut(
                searchAggregateMapper,
                facetsMapper,
                availableFiltersMapper);

        SearchResponse response = new(
            null!,
            new SearchFacets(facets),
            0,
            ["age_range"]);

        SearchResultsMappingContext input = new(
            new SearchRequestViewModel(),
            UseCaseResponse<SearchResponse>.Success(
                response));

        // Act
        SearchResultsViewModel result =
            mapper.Map(input);

        // Assert
        Assert.Equal(
            2,
            result.Filters.Count);

        Assert.Same(
            facetFilters.First(),
            result.Filters[0]);

        Assert.Same(
            availableFilters.First(),
            result.Filters[1]);
    }

    [Fact]
    public void Map_ReturnsEmptyLists_WhenSectionsAreMissing()
    {
        // Arrange
        Mock<IMapper<
            IReadOnlyCollection<SearchAggregateResult>,
            List<GovUkTable>>> searchAggregateMapper =
                new();

        Mock<IMapper<
            FacetResultsMappingContext,
            IReadOnlyCollection<IFilterViewModel>>> facetsMapper =
                CreateFacetsMapper();

        Mock<IMapper<
            AvailableFiltersMappingContext,
            IReadOnlyCollection<IFilterViewModel>>> availableFiltersMapper =
                CreateAvailableFiltersMapper();

        SearchResultsToViewModelMapper mapper =
            CreateSut(
                searchAggregateMapper,
                facetsMapper,
                availableFiltersMapper);

        SearchResponse response =
            new(
                searchProviderResults: null,
                facetedResults: null,
                totalNumberOfResults: 0,
                availableFilters: []);

        SearchResultsMappingContext input =
            new(
                new SearchRequestViewModel(),
                UseCaseResponse<SearchResponse>.Success(
                    response));

        // Act
        SearchResultsViewModel result =
            mapper.Map(input);

        // Assert
        Assert.Empty(
            result.SearchAggregateResults);

        Assert.Empty(
            result.Filters);

        searchAggregateMapper.Verify(
            mapper =>
                mapper.Map(
                    It.IsAny<
                        IReadOnlyCollection<SearchAggregateResult>>()),
            Times.Never);

        facetsMapper.Verify(
            mapper =>
                mapper.Map(
                    It.Is<FacetResultsMappingContext>(
                        context =>
                            context.Facets.Count == 0)),
            Times.Once);

        availableFiltersMapper.Verify(
            mapper =>
                mapper.Map(
                    It.Is<AvailableFiltersMappingContext>(
                        context =>
                            context.AvailableFilters.Count == 0)),
            Times.Once);
    }

    private static SearchAggregateResult MakeSearchAggregate(
    string identifier = "111111",
    string name = "School A")
    {
        return SearchAggregateResult.Create(
            new ProviderIdentifier(identifier),
            new Name(name),
            new SearchAddress(
                "Street, Street 2, Town, County, AB1 2CD"),
            new SearchType("Academy"),
            new GroupDetail("Group Name", "1234"),
            new SearchLocalAuthority("LA Name"),
            new SearchCategory("Establishment"),
            SearchProviderStatus.Open,
            0);
    }

    private static SearchFacet MakeFacet(
        string name = "Phase")
    {
        return new SearchFacet(
            name,
            [
                new FacetResult(
                    "1",
                    "Primary",
                    10),

                new FacetResult(
                    "2",
                    "Secondary",
                    20)
            ]);
    }

    private static SearchResultsToViewModelMapper CreateSut(
        Mock<IMapper<
            IReadOnlyCollection<SearchAggregateResult>,
            List<GovUkTable>>> searchAggregateMapper,
        Mock<IMapper<
            FacetResultsMappingContext,
            IReadOnlyCollection<IFilterViewModel>>> facetsMapper,
        Mock<IMapper<
            AvailableFiltersMappingContext,
            IReadOnlyCollection<IFilterViewModel>>> availableFiltersMapper)
    {
        return new SearchResultsToViewModelMapper(
            searchAggregateMapper.Object,
            facetsMapper.Object,
            availableFiltersMapper.Object);
    }

    private static Mock<IMapper<
        FacetResultsMappingContext,
        IReadOnlyCollection<IFilterViewModel>>> CreateFacetsMapper()
    {
        Mock<IMapper<
            FacetResultsMappingContext,
            IReadOnlyCollection<IFilterViewModel>>> mapper =
                new();

        mapper
            .Setup(mapper =>
                mapper.Map(
                    It.IsAny<FacetResultsMappingContext>()))
            .Returns([]);

        return mapper;
    }

    private static Mock<IMapper<
        AvailableFiltersMappingContext,
        IReadOnlyCollection<IFilterViewModel>>> CreateAvailableFiltersMapper()
    {
        Mock<IMapper<
            AvailableFiltersMappingContext,
            IReadOnlyCollection<IFilterViewModel>>> mapper =
                new();

        mapper
            .Setup(mapper =>
                mapper.Map(
                    It.IsAny<AvailableFiltersMappingContext>()))
            .Returns([]);

        return mapper;
    }

}
