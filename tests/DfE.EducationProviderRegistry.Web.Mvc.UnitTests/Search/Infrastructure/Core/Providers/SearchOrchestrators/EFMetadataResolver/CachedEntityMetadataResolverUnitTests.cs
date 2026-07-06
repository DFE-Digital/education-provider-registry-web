using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers.SearchOrchestrators.EFMetadataResolver;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Search.Infrastructure.Core.Providers.SearchOrchestrators.EFMetadataResolver;

public sealed class CachedEntityMetadataResolverUnitTests
{
    [Fact]
    public void Resolve_WithNullDbContext_ThrowsArgumentNullException()
    {
        // arrange
        CachedEntityMetadataResolver<TestEntity> resolver = new();

        // act / assert
        Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null!));
    }

    [Fact]
    public void Resolve_WhenEntityNotMapped_ThrowsInvalidOperationException()
    {
        // arrange
        CachedEntityMetadataResolver<string> resolver = new();
        using TestDbContext db = new();

        // act / assert
        InvalidOperationException ex =
            Assert.Throws<InvalidOperationException>(() => resolver.Resolve(db));
        Assert.Contains("is not mapped", ex.Message);
    }

    [Fact]
    public void Resolve_WhenEntityHasCompositeKey_ThrowsNotSupportedException()
    {
        // arrange
        CachedEntityMetadataResolver<CompositeKeyEntity> resolver = new();
        using TestDbContext db = new();

        // act / assert
        NotSupportedException ex =
            Assert.Throws<NotSupportedException>(() => resolver.Resolve(db));
        Assert.Contains("composite primary key", ex.Message);
    }

    [Fact]
    public void Resolve_ReturnsCorrectMetadata()
    {
        // arrange
        CachedEntityMetadataResolver<TestEntity> resolver = new();
        using TestDbContext db = new();

        // act
        EntityMetadata metadata = resolver.Resolve(db);

        // assert
        Assert.Equal("TestTable", metadata.TableName);
        Assert.Equal("customschema", metadata.Schema);
        Assert.Equal("Id", metadata.PrimaryKeyProperty.Name);
        Assert.Equal("Id", metadata.PrimaryKeyColumn);
    }

    [Fact]
    public void Resolve_CachesMetadata_AfterFirstCall()
    {
        // arrange
        CachedEntityMetadataResolver<TestEntity> resolver = new();
        using TestDbContext db = new();

        // act
        EntityMetadata first = resolver.Resolve(db);
        EntityMetadata second = resolver.Resolve(db);

        // assert
        Assert.Same(first, second);
    }

    [Fact]
    public void Resolve_IsThreadSafe()
    {
        // arrange
        CachedEntityMetadataResolver<TestEntity> resolver = new();
        using TestDbContext db = new();

        EntityMetadata[] results = new EntityMetadata[10];

        // act
        Parallel.For(0, 10, i =>
        {
            results[i] = resolver.Resolve(db);
        });

        // assert
        Assert.True(results.All(r => ReferenceEquals(r, results[0])));
    }
}

