using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers.SearchOrchestrators;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers.SearchOrchestrators.Context;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Providers.Projections;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Providers;

/// <summary>
/// Provides establishment search functionality by composing a projection,
/// applying filters, and delegating execution to a configured search orchestrator.
/// </summary>
/// <remarks>
/// This provider acts as the entry point for establishment search within the MVC layer.
/// It is responsible for:
/// <list type="bullet">
/// <item><description>Creating a database context via <see cref="IDbContextFactory{TContext}"/>.</description></item>
/// <item><description>Building the base <see cref="IQueryable{Establishment}"/> using the configured projection builder.</description></item>
/// <item><description>Constructing a <see cref="SearchOrchestratorContext"/> containing search parameters.</description></item>
/// <item><description>Generating a SQL filter clause using <see cref="ISearchFilterExpressionsBuilder"/>.</description></item>
/// <item><description>Delegating execution to <see cref="ISearchOrchestrator{TProjection}"/> which performs the actual search.</description></item>
/// </list>
/// 
/// The provider itself does not execute SQL or apply ordering/paging logic; these
/// responsibilities are handled by the orchestrator.
/// </remarks>
public sealed class EstablishmentsSearchProvider : ISearchProvider<Establishment>
{
    private readonly IDbContextFactory<EducationProviderRegistryDbContext> _factory;
    private readonly ISearchOrchestrator<Establishment> _orchestrator;
    private readonly ISearchProjectionBuilder<Establishment> _projectionBuilder;
    private readonly ISearchFilterExpressionsBuilder _searchFilterExpressionsBuilder;
    private readonly string _searchColumn;

    /// <summary>
    /// Initializes a new instance of the <see cref="EstablishmentsSearchProvider"/> class.
    /// </summary>
    /// <param name="factory">Factory used to create <see cref="EducationProviderRegistryDbContext"/> instances.</param>
    /// <param name="orchestrator">The orchestrator responsible for executing the search pipeline.</param>
    /// <param name="projectionBuilder">Builds the base establishment projection used for searching.</param>
    /// <param name="searchFilterExpressionsBuilder">Generates SQL filter expressions from filter requests.</param>
    /// <param name="searchColumn">
    /// The database column used for text search (e.g., <c>name</c>, <c>urn</c>, etc.).
    /// This value is passed to the orchestrator via <see cref="SearchOrchestratorContext"/>.
    /// </param>
    public EstablishmentsSearchProvider(
        IDbContextFactory<EducationProviderRegistryDbContext> factory,
        ISearchOrchestrator<Establishment> orchestrator,
        ISearchProjectionBuilder<Establishment> projectionBuilder,
        ISearchFilterExpressionsBuilder searchFilterExpressionsBuilder,
        string searchColumn)
    {
        _factory = factory;
        _orchestrator = orchestrator;
        _projectionBuilder = projectionBuilder;
        _searchFilterExpressionsBuilder = searchFilterExpressionsBuilder;
        _searchColumn = searchColumn;
    }

    /// <summary>
    /// Executes an establishment search using the configured projection, filters,
    /// and orchestrator.
    /// </summary>
    /// <param name="searchTerm">The text search term provided by the user.</param>
    /// <param name="pageSize">The number of records to return.</param>
    /// <param name="offset">The number of records to skip (for paging).</param>
    /// <param name="filters">A collection of filter requests to apply to the search.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// <returns>
    /// A read‑only list of <see cref="Establishment"/> results matching the search criteria.
    /// </returns>
    /// <remarks>
    /// The method performs the following steps:
    /// <list type="number">
    /// <item><description>Create a database context.</description></item>
    /// <item><description>Build the base establishment query using the projection builder.</description></item>
    /// <item><description>Create a <see cref="SearchOrchestratorContext"/> containing search parameters.</description></item>
    /// <item><description>Generate a SQL filter clause using <see cref="BuildFilterClause"/>.</description></item>
    /// <item><description>Delegate execution to the orchestrator.</description></item>
    /// </list>
    /// 
    /// The orchestrator is responsible for applying text search, ordering, paging,
    /// and executing the final SQL query.
    /// </remarks>
    public async Task<IReadOnlyList<Establishment>> GetMatchingIdsAsync(
        string searchTerm,
        int pageSize,
        int offset,
        IReadOnlyList<SearchFilterRequest> filters,
        CancellationToken cancellationToken = default)
    {
        await using EducationProviderRegistryDbContext db =
            await _factory.CreateDbContextAsync(cancellationToken);

        IQueryable<Establishment> baseQuery = _projectionBuilder.Build(db);

        SearchOrchestratorContext context =
            new()
            {
                SearchColumn = _searchColumn,
                SearchTerm = searchTerm,
                PageSize = pageSize,
                Offset = offset,
                Filters = filters
            };

        return await _orchestrator.ExecuteAsync(
            db,
            baseQuery,
            context,
            BuildFilterClause(filters),
            cancellationToken);
    }

    /// <summary>
    /// Builds a SQL filter clause from the provided filter requests.
    /// </summary>
    /// <param name="searchFilterRequests">The filter requests to convert into SQL.</param>
    /// <returns>
    /// A SQL fragment beginning with <c>AND</c> when filters are present; otherwise an empty string.
    /// </returns>
    /// <remarks>
    /// The returned SQL fragment is appended directly to the orchestrator's query template.
    /// The filter builder is responsible for producing safe, parameterized SQL expressions.
    /// </remarks>
    private string BuildFilterClause(IReadOnlyList<SearchFilterRequest> searchFilterRequests)
    {
        string filter = _searchFilterExpressionsBuilder.BuildSearchFilterExpressions(searchFilterRequests);

        return string.IsNullOrWhiteSpace(filter)
            ? string.Empty
            : $" AND {filter}";
    }
}
