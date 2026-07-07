using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Filtering;
using Moq;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Infrastructure.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class FilterResultsMapperTestDouble
{
    public static Mock<IMapper<
        ReadOnlyCollection<FilterRequest>,
        ReadOnlyCollection<SearchFilterRequest>>> Mock() => new(MockBehavior.Strict);

    public static Mock<IMapper<
        ReadOnlyCollection<FilterRequest>,
        ReadOnlyCollection<SearchFilterRequest>>> MockFor(
            ReadOnlyCollection<SearchFilterRequest> searchFilterRequests)
    {
        var filtersMapper = Mock();

        filtersMapper
            .Setup(mapper =>
                mapper.Map(It.IsAny<ReadOnlyCollection<FilterRequest>>()))
        .Returns(searchFilterRequests)
        .Verifiable();

        return filtersMapper;
    }
}
