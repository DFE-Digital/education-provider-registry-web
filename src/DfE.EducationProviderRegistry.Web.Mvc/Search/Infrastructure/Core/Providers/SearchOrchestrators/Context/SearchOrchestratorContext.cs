using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers.SearchOrchestrators.Context;

public sealed record SearchOrchestratorContext
{
    public required string SearchTerm { get; init; }
    public string SearchColumn { get; init; } = string.Empty;
    public int PageSize { get; init; }
    public int Offset { get; init; }
    public IReadOnlyList<SearchFilterRequest> Filters { get; init; } = [];
}

