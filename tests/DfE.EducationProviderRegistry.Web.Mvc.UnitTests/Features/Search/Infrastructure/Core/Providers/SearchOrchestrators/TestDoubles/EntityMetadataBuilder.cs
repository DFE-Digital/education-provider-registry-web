using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Providers.SearchOrchestrators.EFMetadataResolver;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Diagnostics.CodeAnalysis;
using static DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Infrastructure.Core.Providers.SearchOrchestrators.TrigramSearchOrchestratorUnitTests;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Infrastructure.Core.Providers.SearchOrchestrators.TestDoubles;

[ExcludeFromCodeCoverage]
public static class EntityMetadataBuilder
{
    public static EntityMetadata BuildMetadata(DbContext db)
    {
        var modelBuilder = new ModelBuilder();

        modelBuilder.Entity<TestEntity>()
            .ToTable("test_table", "public")
            .HasKey(entity => entity.Id);

        modelBuilder.Entity<TestEntity>()
            .Property(entity => entity.Name)
            .HasColumnName("name");

        IMutableModel? model = modelBuilder.Model;

        IEntityType entityType = (IEntityType)model.FindEntityType(typeof(TestEntity))!;
        IProperty pk = entityType.FindPrimaryKey()!.Properties[0];

        return new EntityMetadata(
            entityType,
            "public",
            "test_table",
            pk,
            "Id");
    }
}
