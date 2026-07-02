using Microsoft.EntityFrameworkCore.Metadata;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers.SearchOrchestrators.EFMetadataResolver;

public sealed class EntityMetadata
{
    public IEntityType EntityType { get; }
    public string Schema { get; }
    public string TableName { get; }
    public IProperty PrimaryKeyProperty { get; }
    public string PrimaryKeyColumn { get; }

    public EntityMetadata(
        IEntityType entityType,
        string schema,
        string tableName,
        IProperty primaryKeyProperty,
        string primaryKeyColumn)
    {
        EntityType = entityType;
        Schema = schema;
        TableName = tableName;
        PrimaryKeyProperty = primaryKeyProperty;
        PrimaryKeyColumn = primaryKeyColumn;
    }
}
