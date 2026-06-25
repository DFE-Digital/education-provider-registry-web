namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Pipeline.Steps;

internal sealed class SearchOrderMapStep : ISearchPipelineStep
{
    public void Execute(SearchPipelineContext context, CancellationToken cancellationToken)
    {
        IReadOnlyList<string> ids = context.Get<IReadOnlyList<string>>();

        Dictionary<string, int> orderMap =
            ids.Select((id, index) =>
                new KeyValuePair<string, int>(id, index))
                    .ToDictionary(kvp =>
                        kvp.Key, kvp => kvp.Value);

        context.Set(orderMap);
    }
}
