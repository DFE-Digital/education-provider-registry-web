using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers.SearchOrchestrators.EFMetadataResolver;

public sealed class CachedEntityMetadataResolver<T>
    where T : class
{
    private static readonly object _lock = new();
    private static EntityMetadata? _cached;

    public EntityMetadata Resolve(DbContext db)
    {
        ArgumentNullException.ThrowIfNull(db);

        if (_cached != null)
        {
            return _cached;
        }

        lock (_lock)
        {
            if (_cached != null)
            {
                return _cached;
            }

            IEntityType? entityType = db.Model.FindEntityType(typeof(T));
            
            if (entityType == null)
            {
                throw new InvalidOperationException(
                    $"Entity type {typeof(T).Name} is not mapped in the DbContext.");
            }

            string? tableName = entityType.GetTableName();

            if (tableName == null)
            {
                throw new InvalidOperationException(
                    $"Entity {typeof(T).Name} has no table mapping.");
            }

            string schema = entityType.GetSchema() ?? "public";

            IKey? key = entityType.FindPrimaryKey();

            if (key == null)
            {
                throw new InvalidOperationException(
                    $"Entity {typeof(T).Name} has no primary key defined.");
            }

            if (key.Properties.Count != 1)
            {
                throw new NotSupportedException(
                    $"Entity {typeof(T).Name} has a composite primary key, which is not supported.");
            }

            IProperty primaryKeyProperty = key.Properties.Single();
            string primaryKeyColumn = primaryKeyProperty.GetColumnName();

            _cached = new EntityMetadata(
                entityType,
                schema,
                tableName,
                primaryKeyProperty,
                primaryKeyColumn);

            return _cached;
        }
    }
}
