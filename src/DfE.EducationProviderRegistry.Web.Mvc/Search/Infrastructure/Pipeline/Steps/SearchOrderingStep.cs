using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Pipeline.Steps;

internal sealed class SearchOrderingStep : ISearchPipelineStep
{
    public void Execute(SearchPipelineContext context, CancellationToken cancellationToken)
    {
        IReadOnlyList<Establishment> establishments =
            context.Get<IReadOnlyList<Establishment>>();

        Dictionary<string, int> orderMap =
            context.Get<Dictionary<string, int>>();

        List<Establishment> ordered =
            [.. establishments.OrderBy(establishment =>
                orderMap[establishment.Urn!])];

        context.Set(ordered);
    }
}