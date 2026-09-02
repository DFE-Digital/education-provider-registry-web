using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Mappers.TestDoubles;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;
using Moq;
using static System.Net.Mime.MediaTypeNames;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Mappers;

public sealed class SearchResultsToViewModelMapperTests
{
    private static EstablishmentSearchResult MakeEstablishment(string urn = "111111", string name = "School A")
    {
        return new EstablishmentSearchResult(
            new UniqueReferenceNumber(urn),
            new Name(name),
            new Address("Street", "Town", "County", "AB1 2CD"),
            new EstablishmentType("Academy"),
            new GroupDetail("Group Name", "G123"),
            new LocalAuthority("LA Name", "123"));
    }

    private static SearchFacet MakeFacet(string name = "Phase")
    {
        return new SearchFacet(
            name,
            [
                new FacetResult("1", "Primary", 10),
                new FacetResult("2", "Secondary", 20)
            ]);
    }

    [Fact]
    public void Map_ThrowsArgumentNullException_WhenInputIsNull()
    {
        // arrange
        Mock<IMapper<
            IReadOnlyCollection<EstablishmentSearchResult>,
            List<GovUkTable>>> establishmentMapper =
                EstablishmentsMapperTestDouble.Mock();

        Mock<IMapper<
            IReadOnlyCollection<SearchFacet>,
            List<FacetViewModel>>> facetsMapper =
                FacetsMapperTestDouble.Mock();

        SearchResultsToViewModelMapper mapper =
            new(establishmentMapper.Object, facetsMapper.Object);

        // act/assert
        Assert.Throws<ArgumentNullException>(() => mapper.Map(null!));
    }

    [Fact]
    public void Map_ThrowsArgumentException_WhenModelIsNull()
    {
        // arrange
        Mock<IMapper<
            IReadOnlyCollection<EstablishmentSearchResult>,
            List<GovUkTable>>> establishmentMapper =
                EstablishmentsMapperTestDouble.Mock();

        Mock<IMapper<
            IReadOnlyCollection<SearchFacet>,
            List<FacetViewModel>>> facetsMapper =
                FacetsMapperTestDouble.Mock();

        SearchResultsToViewModelMapper mapper =
            new(establishmentMapper.Object, facetsMapper.Object);

        // UseCaseResponse<T>.Failure creates a response with Model = null
        SearchResultsMappingContext input = new(new SearchRequestViewModel(), UseCaseResponse<SearchResponse>.Failure("error"));

        // act + assert
        Assert.Throws<ArgumentException>(() => mapper.Map(input));
    }

    [Fact]
    public void Map_MapsEstablishmentResults_WhenPresent()
    {
        // arrange
        List<EstablishmentSearchResult> establishmentResults = [MakeEstablishment()];
        List<GovUkTable> establishmentTables = [new([new TableColumn { Text = "testColumn" }], [new TableRow { Cells = [new TableCell { Text = "Test Cell" }] }], "School A")];

        Mock<IMapper<
            IReadOnlyCollection<EstablishmentSearchResult>,
            List<GovUkTable>>> establishmentMapper =
                EstablishmentsMapperTestDouble.MockFor(establishmentResults, establishmentTables);

        Mock<IMapper<
            IReadOnlyCollection<SearchFacet>,
            List<FacetViewModel>>> facetsMapper =
                FacetsMapperTestDouble.Mock();

        SearchResultsToViewModelMapper mapper =
            new(establishmentMapper.Object, facetsMapper.Object);

        SearchResponse response =
            new(
                new EstablishmentSearchResults(establishmentResults),
                null, 1);

        SearchResultsMappingContext input = new(new SearchRequestViewModel(), UseCaseResponse<SearchResponse>.Success(response));

        // act
        SearchResultsViewModel vm = mapper.Map(input);

        // assert
        Assert.Single(vm.EstablishmentResults);
        Assert.Equal("School A", vm.EstablishmentResults[0].Caption);
        Assert.Equal("testColumn", vm.EstablishmentResults[0].Columns[0].Text);
        Assert.Equal("Test Cell", vm.EstablishmentResults[0].Rows[0].Cells[0].Text);
        establishmentMapper.Verify(m => m.Map(establishmentResults), Times.Once);
    }

    [Fact]
    public void Map_MapsFacetResults_WhenPresent()
    {
        // arrange
        List<SearchFacet> facets = [MakeFacet()];

        List<FacetViewModel> facetVMs =
        [
            new FacetViewModel("Phase", "Phase", [])
        ];

        Mock<IMapper<
            IReadOnlyCollection<EstablishmentSearchResult>,
            List<GovUkTable>>> establishmentMapper =
                EstablishmentsMapperTestDouble.Mock();

        Mock<IMapper<
            IReadOnlyCollection<SearchFacet>,
            List<FacetViewModel>>> facetsMapper =
                FacetsMapperTestDouble.MockFor(facets, facetVMs);

        SearchResultsToViewModelMapper mapper =
            new(establishmentMapper.Object, facetsMapper.Object);

        SearchResponse response =
            new(
                null!,
                new SearchFacets(facets), 1);

        SearchResultsMappingContext input = new(new SearchRequestViewModel(), UseCaseResponse<SearchResponse>.Success(response));

        // act
        SearchResultsViewModel vm = mapper.Map(input);

        // assert
        Assert.Single(vm.Facets!);
        Assert.Equal("Phase", vm.Facets![0].Name);
        facetsMapper.Verify(mapper => mapper.Map(facets), Times.Once);
    }

    [Fact]
    public void Map_ReturnsEmptyLists_WhenSectionsAreMissing()
    {
        // arrange
        Mock<IMapper<
            IReadOnlyCollection<EstablishmentSearchResult>,
            List<GovUkTable>>> establishmentMapper =
                EstablishmentsMapperTestDouble.Mock();

        Mock<IMapper<
            IReadOnlyCollection<SearchFacet>,
            List<FacetViewModel>>> facetsMapper =
                FacetsMapperTestDouble.Mock();

        SearchResultsToViewModelMapper mapper =
            new(establishmentMapper.Object, facetsMapper.Object);

        var response = new SearchResponse(null!, null, 1);
        SearchResultsMappingContext input = new(new SearchRequestViewModel(), UseCaseResponse<SearchResponse>.Success(response));

        // act
        SearchResultsViewModel vm = mapper.Map(input);

        // assert
        Assert.Empty(vm.EstablishmentResults);
        Assert.Empty(vm.Facets!);
        establishmentMapper.Verify(mapper => mapper.Map(It.IsAny<IReadOnlyCollection<EstablishmentSearchResult>>()), Times.Never);
        facetsMapper.Verify(mapper => mapper.Map(It.IsAny<IReadOnlyCollection<SearchFacet>>()), Times.Never);
    }

    [Fact]
    public void Map_MarksSelectedFacetValuesAsSelected()
    {
        // arrange
        List<SearchFacet> facets =
        [
            MakeFacet("EstablishmentType")
        ];

        List<FacetViewModel> facetViewModels =
        [
            new FacetViewModel(
            "EstablishmentType",
            "Establishment Type",
            [
                new FacetValueViewModel(
                    "1",
                    "Primary",
                    10,
                    false),

                new FacetValueViewModel(
                    "2",
                    "Secondary",
                    20,
                    false)
            ])
        ];

        Mock<IMapper<
            IReadOnlyCollection<EstablishmentSearchResult>,
            List<GovUkTable>>> establishmentMapper =
                EstablishmentsMapperTestDouble.Mock();

        Mock<IMapper<
            IReadOnlyCollection<SearchFacet>,
            List<FacetViewModel>>> facetsMapper =
                FacetsMapperTestDouble.MockFor(
                    facets,
                    facetViewModels);

        SearchResultsToViewModelMapper mapper =
            new(
                establishmentMapper.Object,
                facetsMapper.Object);

        SearchRequestViewModel searchRequest = new()
        {
            SelectedFacets = new Dictionary<string, List<string>>
            {
                ["EstablishmentType"] =
                [
                    "1"
                ]
            }
        };

        SearchResponse response =
            new(
                null!,
                new SearchFacets(facets), 1);

        SearchResultsMappingContext input =
            new(
                searchRequest,
                UseCaseResponse<SearchResponse>.Success(response));

        // act
        SearchResultsViewModel result = mapper.Map(input);

        // assert
        FacetViewModel phaseFacet =
            Assert.Single(result.Facets!);

        FacetValueViewModel primary =
            Assert.Single(
                phaseFacet.Values,
                value => value.Label == "Primary");

        FacetValueViewModel secondary =
            Assert.Single(
                phaseFacet.Values,
                value => value.Label == "Secondary");

        Assert.True(primary.IsSelected);
        Assert.False(secondary.IsSelected);
    }

    [Fact]
    public void Map_LeavesFacetValuesUnselected_WhenFacetHasNotBeenSelected()
    {
        // arrange
        List<SearchFacet> facets =
        [
            MakeFacet("Phase")
        ];

        List<FacetViewModel> facetViewModels =
        [
            new FacetViewModel(
            "Phase",
            "Phase",
            [
                new FacetValueViewModel(
                    "1",
                    "Primary",
                    10,
                    false),

                new FacetValueViewModel(
                    "2",
                    "Secondary",
                    20,
                    false)
            ])
        ];

        Mock<IMapper<
            IReadOnlyCollection<EstablishmentSearchResult>,
            List<GovUkTable>>> establishmentMapper =
                EstablishmentsMapperTestDouble.Mock();

        Mock<IMapper<
            IReadOnlyCollection<SearchFacet>,
            List<FacetViewModel>>> facetsMapper =
                FacetsMapperTestDouble.MockFor(
                    facets,
                    facetViewModels);

        SearchResultsToViewModelMapper mapper =
            new(
                establishmentMapper.Object,
                facetsMapper.Object);

        SearchRequestViewModel searchRequest = new()
        {
            SelectedFacets = new Dictionary<string, List<string>>
            {
                ["EstablishmentType"] =
                [
                    "1"
                ]
            }
        };

        SearchResponse response =
            new(
                null!,
                new SearchFacets(facets), 1);

        SearchResultsMappingContext input =
            new(
                searchRequest,
                UseCaseResponse<SearchResponse>.Success(response));

        // act
        SearchResultsViewModel result = mapper.Map(input);

        // assert
        FacetViewModel phaseFacet =
            Assert.Single(result.Facets!);

        Assert.All(
            phaseFacet.Values,
            value => Assert.False(value.IsSelected));
    }
}
