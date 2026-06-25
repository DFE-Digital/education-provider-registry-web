using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Pipeline.Steps;

internal sealed class FacetQueryDispatchStep : ISearchPipelineStep
{
    private readonly IFacetProvider<Establishment> _facetProvider;

    public FacetQueryDispatchStep(
        IFacetProvider<Establishment> facetProvider)
    {
        _facetProvider = facetProvider;
    }

    public void Execute(SearchPipelineContext context, CancellationToken cancellationToken)
    {
        IReadOnlyList<string> ids = context.Get<IReadOnlyList<string>>();
        List<string> facetNames = context.Get<List<string>>();

        List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)> tasks =
            [.. facetNames.Select(name =>
                (name, _facetProvider.GetFacetsAsync(ids, name, cancellationToken)))];

        context.Set(tasks);
    }
}