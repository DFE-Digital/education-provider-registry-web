using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.ViewModels;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Controllers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchFiltersViewModelMapperStub
{
    public static Mock<IMapper<
        SearchFiltersMappingContext,
        SearchFiltersViewModel>> Mock() => new(MockBehavior.Strict);

    public static Mock<IMapper<SearchFiltersMappingContext, SearchFiltersViewModel>> MockFor(SearchFiltersViewModel response)
    {
        Mock<IMapper<SearchFiltersMappingContext, SearchFiltersViewModel>> mock = Mock();

        mock
            .Setup(mapper =>
                mapper.Map(
                    It.IsAny<SearchFiltersMappingContext>()))
            .Returns(response)
            .Verifiable();

        return mock;
    }
}