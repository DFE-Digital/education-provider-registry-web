using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Web.Mvc.Search.ViewModels;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Search.Mappers.TestDoubles;

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
        var facetsMapper = Mock();

        facetsMapper.Setup(mapper =>
            mapper.Map(facets)).Returns(results);

        return facetsMapper;
    }
    
}
