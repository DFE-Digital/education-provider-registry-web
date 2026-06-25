using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Pipeline.Steps;

internal sealed class ParallelMappingStep : ISearchPipelineStep
{
    private readonly IMapper<Establishment, EstablishmentSearchResult> _mapper;

    public ParallelMappingStep(
        IMapper<Establishment, EstablishmentSearchResult> mapper)
    {
        _mapper = mapper;
    }

    public void Execute(SearchPipelineContext context, CancellationToken cancellationToken)
    {
        List<Establishment> ordered =
            context.Get<List<Establishment>>();

        EstablishmentSearchResult[] results =
            new EstablishmentSearchResult[ordered.Count];

        ParallelOptions options = new()
        {
            CancellationToken = cancellationToken
        };

        Parallel.ForEach(
            ordered.Select((establishment, index) =>
                new KeyValuePair<int, Establishment>(index, establishment)),
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
