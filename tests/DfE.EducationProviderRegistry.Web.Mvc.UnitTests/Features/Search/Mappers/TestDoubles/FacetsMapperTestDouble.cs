using System.Diagnostics.CodeAnalysis;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Mappers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class FacetsMapperTestDouble
{
    public static Mock<IMapper<
        IReadOnlyCollection<SearchFacet>,
        List<FacetViewModel>>> Mock() => new(MockBehavior.Strict);

    public static Mock<IMapper<
        IReadOnlyCollection<SearchFacet>,
        List<FacetViewModel>>> MockFor(
        List<SearchFacet> facets, List<FacetViewModel> results)
    {
        Mock<IMapper<IReadOnlyCollection<SearchFacet>, List<FacetViewModel>>> facetsMapper = Mock();

        facetsMapper.Setup(mapper =>
            mapper.Map(facets)).Returns(results);

        return facetsMapper;
    }

}
