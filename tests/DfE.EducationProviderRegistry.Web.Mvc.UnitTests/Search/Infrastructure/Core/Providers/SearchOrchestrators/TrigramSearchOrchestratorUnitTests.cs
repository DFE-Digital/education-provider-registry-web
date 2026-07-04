using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers.SearchOrchestrators;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers.SearchOrchestrators.Context;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers.SearchOrchestrators.EFMetadataResolver;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Search.Infrastructure.Core.Providers.SearchOrchestrators;

public sealed class TrigramSearchOrchestratorTests
{
    public sealed class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private static DbContext BuildFakeDbContext()
    {
        return new Mock<DbContext>(new DbContextOptions<DbContext>()).Object;
    }

    private static EntityMetadata BuildMetadata(DbContext db)
    {
        var modelBuilder = new ModelBuilder();

        modelBuilder.Entity<TestEntity>()
            .ToTable("test_table", "public")
            .HasKey(e => e.Id);

        modelBuilder.Entity<TestEntity>()
            .Property(e => e.Name)
            .HasColumnName("name");

        var model = modelBuilder.Model;

        IEntityType entityType = (IEntityType)model.FindEntityType(typeof(TestEntity))!;
        IProperty pk = entityType.FindPrimaryKey()!.Properties[0];

        return new EntityMetadata(
            entityType,
            "public",
            "test_table",
            pk,
            "Id");
    }

    private static Mock<IEntityMetadataResolver<TestEntity>> BuildResolverMock(EntityMetadata metadata)
    {
        var resolverMock = new Mock<IEntityMetadataResolver<TestEntity>>();
        resolverMock
            .Setup(r => r.Resolve(It.IsAny<DbContext>()))
            .Returns(metadata);
        return resolverMock;
    }

    private static Mock<ISqlExecutor<TestEntity>> BuildSqlExecutorMock(IEnumerable<TestEntity> sqlResults)
    {
        var sqlExecutorMock = new Mock<ISqlExecutor<TestEntity>>();

        sqlExecutorMock
            .Setup(x => x.ExecuteIdsAsync(
                It.IsAny<DbContext>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(sqlResults.Select(e => (object)e.Id).ToList());

        return sqlExecutorMock;
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsMatchingEntities_WhenSearchMatches()
    {
        var db = BuildFakeDbContext();
        var metadata = BuildMetadata(db);

        var resolverMock = BuildResolverMock(metadata);

        var sqlExecutorMock = BuildSqlExecutorMock(new[]
        {
            new TestEntity { Id = 1, Name = "alpha" }
        });

        var orchestrator = new TrigramSearchOrchestrator<TestEntity>(
            resolverMock.Object,
            sqlExecutorMock.Object);

        IQueryable<TestEntity> baseQuery = new[]
        {
            new TestEntity { Id = 1, Name = "alpha" },
            new TestEntity { Id = 2, Name = "beta" }
        }
        .AsQueryable();

        var context = new SearchOrchestratorContext
        {
            SearchTerm = "alpha",
            SearchColumn = "name",
            PageSize = 10,
            Offset = 0
        };

        var result =
            await orchestrator.ExecuteAsync(
                db,
                baseQuery,
                context,
                "",
                CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsEmptyList_WhenNoMatches()
    {
        var db = BuildFakeDbContext();
        var metadata = BuildMetadata(db);

        var resolverMock = BuildResolverMock(metadata);
        var sqlExecutorMock = BuildSqlExecutorMock(Array.Empty<TestEntity>());

        var orchestrator = new TrigramSearchOrchestrator<TestEntity>(
            resolverMock.Object,
            sqlExecutorMock.Object);

        IQueryable<TestEntity> baseQuery = new[]
        {
            new TestEntity { Id = 1, Name = "alpha" },
            new TestEntity { Id = 2, Name = "beta" }
        }.AsQueryable();

        var context = new SearchOrchestratorContext
        {
            SearchTerm = "zzz",
            SearchColumn = "name",
            PageSize = 10,
            Offset = 0
        };

        IReadOnlyList<TestEntity> result =
            await orchestrator.ExecuteAsync(
                db,
                baseQuery,
                context,
                "",
                CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task ExecuteAsync_Throws_WhenColumnDoesNotExist()
    {
        var db = BuildFakeDbContext();
        var metadata = BuildMetadata(db);

        var resolverMock = BuildResolverMock(metadata);
        var sqlExecutorMock = BuildSqlExecutorMock(Array.Empty<TestEntity>());

        var orchestrator = new TrigramSearchOrchestrator<TestEntity>(
            resolverMock.Object,
            sqlExecutorMock.Object);

        IQueryable<TestEntity> baseQuery = Array.Empty<TestEntity>().AsQueryable();

        var context = new SearchOrchestratorContext
        {
            SearchTerm = "alpha",
            SearchColumn = "nonexistent_column",
            PageSize = 10,
            Offset = 0
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            orchestrator.ExecuteAsync(
                db,
                baseQuery,
                context,
                "",
                CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_Throws_WhenDbContextIsNull()
    {
        var metadata = BuildMetadata(BuildFakeDbContext());

        var resolverMock = BuildResolverMock(metadata);
        var sqlExecutorMock = BuildSqlExecutorMock(Array.Empty<TestEntity>());

        var orchestrator = new TrigramSearchOrchestrator<TestEntity>(
            resolverMock.Object,
            sqlExecutorMock.Object);

        IQueryable<TestEntity> baseQuery = Array.Empty<TestEntity>().AsQueryable();

        var context = new SearchOrchestratorContext
        {
            SearchTerm = "alpha",
            SearchColumn = "name",
            PageSize = 10,
            Offset = 0
        };

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            orchestrator.ExecuteAsync(
                null!,
                baseQuery,
                context,
                "",
                CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_Throws_WhenContextIsNull()
    {
        var db = BuildFakeDbContext();
        var metadata = BuildMetadata(db);

        var resolverMock = BuildResolverMock(metadata);
        var sqlExecutorMock = BuildSqlExecutorMock(Array.Empty<TestEntity>());

        var orchestrator = new TrigramSearchOrchestrator<TestEntity>(
            resolverMock.Object,
            sqlExecutorMock.Object);

        IQueryable<TestEntity> baseQuery = Array.Empty<TestEntity>().AsQueryable();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            orchestrator.ExecuteAsync(
                db,
                baseQuery,
                null!,
                "",
                CancellationToken.None));
    }
}