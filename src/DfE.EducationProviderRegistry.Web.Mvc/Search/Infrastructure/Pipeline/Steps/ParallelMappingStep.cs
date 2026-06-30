using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Providers.Projections;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Pipeline.Steps;

internal sealed class ParallelMappingStep : ISearchPipelineStep
{
    private readonly IMapper<SearchResultProjection, EstablishmentSearchResult> _mapper;

    public ParallelMappingStep(
        IMapper<SearchResultProjection, EstablishmentSearchResult> mapper)
    {
        _mapper = mapper;
    }

    public void Execute(SearchPipelineContext context, CancellationToken cancellationToken)
    {
        List<SearchResultProjection> ordered =
            context.Get<List<SearchResultProjection>>();

        EstablishmentSearchResult[] results =
            new EstablishmentSearchResult[ordered.Count];

        ParallelOptions options = new()
        {
            CancellationToken = cancellationToken
        };

        Parallel.ForEach(
            ordered.Select((establishment, index) =>
                new KeyValuePair<int, SearchResultProjection>(index, establishment)),
            options,
            pair =>
            {
                options.CancellationToken.ThrowIfCancellationRequested();
                EstablishmentSearchResult mapped = _mapper.Map(pair.Value);
                results[pair.Key] = mapped;
            });

        context.Set(results);
    }
}
