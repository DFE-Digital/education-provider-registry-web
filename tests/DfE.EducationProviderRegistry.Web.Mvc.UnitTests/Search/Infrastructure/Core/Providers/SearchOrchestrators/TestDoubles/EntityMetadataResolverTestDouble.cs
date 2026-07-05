using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers.SearchOrchestrators.EFMetadataResolver;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Diagnostics.CodeAnalysis;
using static DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Search.Infrastructure.Core.Providers.SearchOrchestrators.TrigramSearchOrchestratorTests;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Search.Infrastructure.Core.Providers.SearchOrchestrators.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class EntityMetadataResolverTestDouble
{
    public static Mock<IEntityMetadataResolver<TestEntity>> Mock() => new(MockBehavior.Strict);

    public static Mock<IEntityMetadataResolver<TestEntity>> MockFor(EntityMetadata metadata)
    {
        var resolverMock = Mock();
        resolverMock
            .Setup(resolver =>
                resolver.Resolve(It.IsAny<DbContext>()))
            .Returns(metadata)
            .Verifiable();

        return resolverMock;
    }
}
