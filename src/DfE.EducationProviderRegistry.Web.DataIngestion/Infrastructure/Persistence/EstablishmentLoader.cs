using DfE.EducationProviderRegistry.Web.DataPipeline.Application.Models;
using DfE.EducationProviderRegistry.Web.DataPipeline.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Web.DataPipeline.Infrastructure.Persistence;

/// <summary>
/// Infrastructure implementation of <see cref="IEstablishmentLoader"/> that
/// persists canonical establishment records into the PostgreSQL read model store.
/// </summary>
/// <remarks>
/// This loader is responsible for:
/// <list type="bullet">
/// <item><description>Ensuring the database is ready for ETL operations.</description></item>
/// <item><description>Upserting establishment records in an idempotent manner.</description></item>
/// <item><description>Removing records that no longer exist upstream.</description></item>
/// </list>
/// </remarks>
public sealed class EstablishmentLoader : IEstablishmentLoader
{
    private readonly EstablishmentDbContext _dbContext;

    /// <summary>
    /// Creates a new instance of the <see cref="EstablishmentLoader"/> class.
    /// </summary>
    /// <param name="dbContext">
    /// The EF Core database context used to persist establishment records.
    /// </param>
    public EstablishmentLoader(EstablishmentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Ensures that the persistence layer is ready to receive establishment data.
    /// This may include applying migrations or verifying schema readiness.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token that may be used to cancel the operation.
    /// </param>
    public async Task EnsureReadyAsync(CancellationToken cancellationToken)
    {
        await _dbContext.Database.MigrateAsync(cancellationToken);
    }

    /// <summary>
    /// Inserts or updates the specified establishment record in the database.
    /// Implementations must guarantee idempotency so repeated ETL runs do not
    /// produce inconsistent state.
    /// </summary>
    /// <param name="establishment">
    /// The canonical establishment record to be persisted.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that may be used to cancel the operation.
    /// </param>
    public async Task UpsertAsync(
        EstablishmentReadModel establishment,
        CancellationToken cancellationToken)
    {
        EstablishmentReadModel? existing =
            await _dbContext.Establishments
                .FirstOrDefaultAsync(establishment =>
                    establishment.Urn == establishment.Urn, cancellationToken);

        if (existing == null){
            _dbContext.Establishments.Add(establishment);
        }
        else{
            _dbContext.Entry(existing).CurrentValues.SetValues(establishment);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Removes any establishments from the database whose identifiers are not
    /// present in the supplied collection. This ensures the read model remains
    /// consistent with the upstream source after an ETL run.
    /// </summary>
    /// <param name="validEstablishmentIds">
    /// A collection of establishment identifiers that were successfully extracted
    /// during the current ETL run.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that may be used to cancel the operation.
    /// </param>
    public async Task CleanupMissingAsync(
        IEnumerable<int> validEstablishmentIds,
        CancellationToken cancellationToken)
    {
        HashSet<int> validIds = [.. validEstablishmentIds];

        List<EstablishmentReadModel> toDelete =
            await _dbContext.Establishments
                .Where(establishment => !validIds.Contains(establishment.Urn))
                .ToListAsync(cancellationToken);

        if (toDelete.Count > 0)
        {
            _dbContext.Establishments.RemoveRange(toDelete);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
