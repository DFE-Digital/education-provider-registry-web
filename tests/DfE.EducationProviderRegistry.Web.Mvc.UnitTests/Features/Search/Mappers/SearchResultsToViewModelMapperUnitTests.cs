using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Mappers.TestDoubles;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using Moq;

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
                new FacetResult("Primary", 10),
                new FacetResult("Secondary", 20)
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
        var input = UseCaseResponse<SearchResponse>.Failure("error");

        // act + assert
        Assert.Throws<ArgumentException>(() => mapper.Map(input));
    }

    [Fact]
    public void Map_MapsEstablishmentResults_WhenPresent()
    {
        // arrange
        List<EstablishmentSearchResult> establishmentResults = [MakeEstablishment()];
        List<GovUkTable> establishmentTables = [new() { Caption = "School A" }];

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
                null);

        var input = UseCaseResponse<SearchResponse>.Success(response);

        // act
        SearchResultsViewModel vm = mapper.Map(input);

        // assert
        Assert.Single(vm.EstablishmentResults);
        Assert.Equal("School A", vm.EstablishmentResults[0].Caption);
        establishmentMapper.Verify(m => m.Map(establishmentResults), Times.Once);
    }

    [Fact]
    public void Map_MapsFacetResults_WhenPresent()
    {
        // arrange
        List<SearchFacet> facets = [MakeFacet()];

        List<FacetViewModel> facetVMs =
        [
            new FacetViewModel("Phase", [])
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
                new SearchFacets(facets));

        var input = UseCaseResponse<SearchResponse>.Success(response);

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

        var response = new SearchResponse(null!, null);
        var input = UseCaseResponse<SearchResponse>.Success(response);

        // act
        SearchResultsViewModel vm = mapper.Map(input);

        // assert
        Assert.Empty(vm.EstablishmentResults);
        Assert.Empty(vm.Facets!);
        establishmentMapper.Verify(mapper => mapper.Map(It.IsAny<IReadOnlyCollection<EstablishmentSearchResult>>()), Times.Never);
        facetsMapper.Verify(mapper => mapper.Map(It.IsAny<IReadOnlyCollection<SearchFacet>>()), Times.Never);
    }
}
