using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using Moq;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Search.Controllers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchFacetsResultsMapperTestDouble
{
    public static Mock<IMapper<
        Dictionary<string, List<string>>?,
        ReadOnlyCollection<FilterRequest>>> Mock() => new(MockBehavior.Strict);

    public static Mock<IMapper<
        Dictionary<string, List<string>>?,
        ReadOnlyCollection<FilterRequest>>> MockFor(
        ReadOnlyCollection<FilterRequest> response)
    {
        Mock<IMapper<
            Dictionary<string, List<string>>?,
            ReadOnlyCollection<FilterRequest>>> mock = Mock();

        mock
            .Setup(mapper =>
                mapper.Map(
                    It.IsAny<Dictionary<string, List<string>>>()))
            .Returns(response)
            .Verifiable();

        return mock;
    }
}