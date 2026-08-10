using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using Moq;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Controllers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchFacetsResultsMapperTestDouble
{
    public static Mock<IMapper<
        Dictionary<string, List<SelectedFacetValueViewModel>>?,
        ReadOnlyCollection<FilterRequest>>> Mock() => new(MockBehavior.Strict);

    public static Mock<IMapper<
        Dictionary<string, List<SelectedFacetValueViewModel>>?,
        ReadOnlyCollection<FilterRequest>>> MockFor(
        ReadOnlyCollection<FilterRequest> response)
    {
        Mock<IMapper<
            Dictionary<string, List<SelectedFacetValueViewModel>>?,
            ReadOnlyCollection<FilterRequest>>> mock = Mock();

        mock
            .Setup(mapper =>
                mapper.Map(
                    It.IsAny<Dictionary<string, List<SelectedFacetValueViewModel>>>()))
            .Returns(response)
            .Verifiable();

        return mock;
    }
}