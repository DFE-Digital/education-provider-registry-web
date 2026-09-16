using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Mappers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchAggregatesMapperTestDouble
{
    public static Mock<IMapper<
        IReadOnlyCollection<SearchAggregateResult>,
        List<GovUkTable>>> Mock() => new(MockBehavior.Strict);


    public static Mock<IMapper<
        IReadOnlyCollection<SearchAggregateResult>,
        List<GovUkTable>>> MockFor(
            List<SearchAggregateResult> aggregateResults,
            List<GovUkTable> aggregateTables)
    {
        Mock<IMapper<IReadOnlyCollection<SearchAggregateResult>, List<GovUkTable>>> aggregateMapper = Mock();

        aggregateMapper.Setup(mapper =>
            mapper.Map(aggregateResults))
                .Returns(aggregateTables);

        return aggregateMapper;
    }
}
