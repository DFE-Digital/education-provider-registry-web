using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Controllers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchResultsMapperTestDouble
{
    public static Mock<IMapper<SearchResultsMappingContext, SearchResultsViewModel>> Mock() => new(MockBehavior.Strict);

    public static Mock<IMapper<
        SearchResultsMappingContext,
        SearchResultsViewModel>> MockFor(SearchResultsViewModel response)
    {
        Mock<IMapper<
            SearchResultsMappingContext,
            SearchResultsViewModel>> mock = Mock();

        mock
            .Setup(mapper =>
                mapper.Map(
                    It.IsAny<SearchResultsMappingContext>()))
            .Returns(response)
            .Verifiable();

        return mock;
    }
}
