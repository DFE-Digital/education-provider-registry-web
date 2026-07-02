using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.Search.ViewModels;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Search.Controllers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchResultsMapperTestDouble
{
    public static Mock<IMapper<
        UseCaseResponse<
            SearchResponse>, SearchResultsViewModel>> Mock() => new(MockBehavior.Strict);

    public static Mock<IMapper<
        UseCaseResponse<
        SearchResponse>, SearchResultsViewModel>> MockFor(SearchResultsViewModel response)
    {
        Mock<IMapper<
            UseCaseResponse<
            SearchResponse>, SearchResultsViewModel>> mock = Mock();

        mock
            .Setup(mapper =>
                mapper.Map(
                    It.IsAny<UseCaseResponse<SearchResponse>>()))
            .Returns(response)
            .Verifiable();

        return mock;
    }
}
