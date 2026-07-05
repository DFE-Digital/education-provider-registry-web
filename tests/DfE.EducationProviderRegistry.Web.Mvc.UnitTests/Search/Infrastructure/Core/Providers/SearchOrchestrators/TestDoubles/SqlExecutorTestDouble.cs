using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers.SearchOrchestrators;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Diagnostics.CodeAnalysis;
using static DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Search.Infrastructure.Core.Providers.SearchOrchestrators.TrigramSearchOrchestratorTests;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Search.Infrastructure.Core.Providers.SearchOrchestrators.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SqlExecutorTestDouble
{
    public static Mock<ISqlExecutor<TestEntity>> Mock() => new(MockBehavior.Strict);

    public static Mock<ISqlExecutor<TestEntity>> MockFor(IEnumerable<TestEntity> sqlResults)
    {
        var sqlExecutorMock = Mock();

        sqlExecutorMock
            .Setup(executor =>
                executor.ExecuteIdsAsync(
                    It.IsAny<DbContext>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                .. sqlResults.Select(entity => (object?)entity.Id!)
            ]);

        return sqlExecutorMock;
    }
}
