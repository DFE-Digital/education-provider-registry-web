using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Providers;

public sealed class EstablishmentsUrnSearchProvider : IIdSearchProvider<Establishment>
{
    private readonly IDbContextFactory<EducationProviderRegistryDbContext> _factory;
    private readonly ISearchFilterExpressionsBuilder _searchFilterExpressionsBuilder;
    private readonly string _sqlTemplate;

    public EstablishmentsUrnSearchProvider(
        IDbContextFactory<EducationProviderRegistryDbContext> factory,
        ISearchFilterExpressionsBuilder searchFilterExpressionsBuilder,
        string schemaName,
        string tableName,
        string searchColumn)
    {
        _factory = factory ??
            throw new ArgumentNullException(nameof(factory));
        _searchFilterExpressionsBuilder = searchFilterExpressionsBuilder ??
            throw new ArgumentNullException(nameof(searchFilterExpressionsBuilder));

        _sqlTemplate =
            """
            SELECT 
                urn::int AS "Id",
                {COLUMN} AS "SearchColumn"
            FROM {SCHEMA}.{TABLE}
            WHERE {COLUMN} % @p0
            {FILTERS}
            ORDER BY similarity({COLUMN}, @p0) DESC
            LIMIT @p1 OFFSET @p2
            """
            .Replace("{SCHEMA}", QuoteIdentifier(schemaName))
            .Replace("{TABLE}", QuoteIdentifier(tableName))
            .Replace("{COLUMN}", QuoteIdentifier(searchColumn));
    }

    public async Task<IReadOnlyList<string>> GetMatchingIdsAsync(
        string searchTerm,
        int pageSize,
        int offset,
        IReadOnlyList<SearchFilterRequest> searchFilterRequests,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(searchTerm);

        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        List<SearchResultRow> searchResultRows =
            await db.Database
                .SqlQueryRaw<SearchResultRow>(
                    sql: _sqlTemplate.Replace("{FILTERS}", BuildFilterClause(searchFilterRequests)),
                    parameters: [searchTerm, pageSize, offset])
                .ToListAsync(cancellationToken);

        return [
            .. searchResultRows.Select(searchResultRow =>
                searchResultRow.Id.ToString())];
    }

    private string BuildFilterClause(
        IReadOnlyList<SearchFilterRequest> searchFilterRequests)
    {
        string filter =
            _searchFilterExpressionsBuilder
                .BuildSearchFilterExpressions(searchFilterRequests);

        return string.IsNullOrWhiteSpace(filter)
            ? string.Empty
            : $" AND {filter}";
    }


    private static string QuoteIdentifier(
        string identifier) => "\"" + identifier.Replace("\"", "\"\"") + "\"";

    private sealed class SearchResultRow
    {
        public int Id { get; set; }
        public string SearchColumn { get; set; } = string.Empty;
    }
}
