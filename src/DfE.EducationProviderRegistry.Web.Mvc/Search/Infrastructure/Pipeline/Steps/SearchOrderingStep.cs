using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Providers.Projections;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Pipeline.Steps;

internal sealed class SearchOrderingStep : ISearchPipelineStep
{
    public void Execute(SearchPipelineContext context, CancellationToken cancellationToken)
    {
        IReadOnlyList<SearchResultProjection> establishments =
            context.Get<IReadOnlyList<SearchResultProjection>>();

        Dictionary<string, int> orderMap =
            context.Get<Dictionary<string, int>>();

        List<SearchResultProjection> ordered =
            [.. establishments.OrderBy(establishment =>
                orderMap[establishment.Id.ToString()!])];

        context.Set(ordered);
    }
}