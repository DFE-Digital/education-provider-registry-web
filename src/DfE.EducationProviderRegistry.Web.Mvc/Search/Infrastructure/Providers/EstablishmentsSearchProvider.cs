using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Providers.Projections;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Providers;

public sealed class EstablishmentsSearchProvider : ISearchProvider<Establishment>
{
    private readonly IDbContextFactory<EducationProviderRegistryDbContext> _factory;
    private readonly ISearchFilterExpressionsBuilder _searchFilterExpressionsBuilder;
    private readonly string _sqlTemplate;

    public EstablishmentsSearchProvider(
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
                e.urn::int AS "Id",
                e.{COLUMN} AS "SearchColumn",
                e.name AS "Name",
                s.address_line_1 AS "AddressLine1",
                s.address_line_2 AS "AddressLine2",
                s.town AS "Town",
                s.county AS "County",
                s.postcode AS "Postcode",
                t.name AS "EstablishmentType",
                a.authority_name AS "AuthorityName",
                a.authority_code AS "AuthorityCode",
                gt.code AS "GroupCode",
                gt.name AS "GroupName"
            FROM {SCHEMA}.{TABLE} e
            INNER JOIN core.site s 
                ON s.establishment_id = e.establishment_id
            INNER JOIN ref.establishment_type t 
                ON t.establishment_type_id = e.establishment_type_id
            INNER JOIN core.establishment_group_membership gm
                ON gm.establishment_id = e.establishment_id
            INNER JOIN ref.group_type gt
                ON gt.group_type_id = gm.group_id
            INNER JOIN core.establishment_authority a 
                ON a.establishment_id = e.establishment_id
            WHERE e.{COLUMN} % @p0
            {FILTERS}
            ORDER BY similarity(e.{COLUMN}, @p0) DESC
            LIMIT @p1 OFFSET @p2
            """
            .Replace("{SCHEMA}", QuoteIdentifier(schemaName))
            .Replace("{TABLE}", QuoteIdentifier(tableName))
            .Replace("{COLUMN}", QuoteIdentifier(searchColumn));
    }

    public async Task<IReadOnlyList<SearchResultProjection>> GetMatchingIdsAsync(
        string searchTerm,
        int pageSize,
        int offset,
        IReadOnlyList<SearchFilterRequest> searchFilterRequests,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(searchTerm);

        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        return await db.Database
            .SqlQueryRaw<SearchResultProjection>(
                sql: _sqlTemplate.Replace("{FILTERS}", BuildFilterClause(searchFilterRequests)),
                parameters: [searchTerm, pageSize, offset])
            .ToListAsync(cancellationToken);
    }

    private string BuildFilterClause(IReadOnlyList<SearchFilterRequest> searchFilterRequests)
    {
        string filter = _searchFilterExpressionsBuilder.BuildSearchFilterExpressions(searchFilterRequests);

        return string.IsNullOrWhiteSpace(filter)
            ? string.Empty
            : $" AND {filter}";
    }

    private static string QuoteIdentifier(string identifier)
        => "\"" + identifier.Replace("\"", "\"\"") + "\"";
}
