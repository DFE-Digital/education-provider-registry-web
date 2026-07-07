using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Mappers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class EstablishmentsMapperTestDouble
{
    public static Mock<IMapper<
        IReadOnlyCollection<EstablishmentSearchResult>,
        List<GovUkTable>>> Mock() => new(MockBehavior.Strict);


    public static Mock<IMapper<
        IReadOnlyCollection<EstablishmentSearchResult>,
        List<GovUkTable>>> MockFor(
            List<EstablishmentSearchResult> establishmentResults,
            List<GovUkTable> establishmentTables)
    {
        var establishmentMapper = Mock();

        establishmentMapper.Setup(mapper =>
            mapper.Map(establishmentResults))
                .Returns(establishmentTables);

        return establishmentMapper;
    }
}
