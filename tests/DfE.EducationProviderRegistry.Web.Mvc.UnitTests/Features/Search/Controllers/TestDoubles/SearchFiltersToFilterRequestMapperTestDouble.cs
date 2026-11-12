using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Request;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Controllers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchFiltersToFilterRequestMapperTestDouble
{
    public static Mock<IMapper<SearchRequestViewModel, ReadOnlyCollection<FilterRequest>>> Mock() =>
        new(MockBehavior.Strict);

    public static Mock<IMapper<SearchRequestViewModel, ReadOnlyCollection<FilterRequest>>> MockFor(
        ReadOnlyCollection<FilterRequest> response)
    {
        Mock<IMapper<
            SearchRequestViewModel,
            ReadOnlyCollection<FilterRequest>>> mock = Mock();

        mock
            .Setup(mapper =>
                mapper.Map(
                    It.IsAny<SearchRequestViewModel>()))
            .Returns(response)
            .Verifiable();

        return mock;
    }

}
