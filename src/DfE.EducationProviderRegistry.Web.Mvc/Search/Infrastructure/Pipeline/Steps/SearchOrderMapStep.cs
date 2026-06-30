using System.Collections.ObjectModel;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Pipeline.Steps;

internal sealed class SearchOrderMapStep : ISearchPipelineStep
{
    public void Execute(SearchPipelineContext context, CancellationToken cancellationToken)
    {
        ReadOnlyCollection<string> ids = context.Get<ReadOnlyCollection<string>>();

        Dictionary<string, int> orderMap =
            ids.Select((id, index) =>
                new KeyValuePair<string, int>(id, index))
                    .ToDictionary(kvp =>
                        kvp.Key, kvp => kvp.Value);

        context.Set(orderMap);
    }
}
