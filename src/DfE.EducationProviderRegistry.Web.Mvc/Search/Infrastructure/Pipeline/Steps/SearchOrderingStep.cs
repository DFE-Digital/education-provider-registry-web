using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Pipeline.Steps;

/// <summary>
/// Orders establishments according to a precomputed order map.
/// </summary>
/// <remarks>
/// This step performs strict validation:
/// <list type="bullet">
/// <item><description>The pipeline context must contain an ordered list of establishments.</description></item>
/// <item><description>The pipeline context must contain an order map keyed by URN.</description></item>
/// <item><description>Missing URNs in the order map are surfaced with a clear exception.</description></item>
/// </list>
/// </remarks>
internal sealed class SearchOrderingStep : ISearchPipelineStep
{
    /// <summary>
    /// Orders establishments according to the order map stored in the pipeline context.
    /// </summary>
    /// <param name="context">The pipeline context containing establishments and an order map.</param>
    /// <param name="cancellationToken">A token used to observe cancellation.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when required context entries are missing or when an establishment URN is not present in the order map.
    /// </exception>
    public void Execute(SearchPipelineContext context, CancellationToken cancellationToken)
    {
        IReadOnlyList<Establishment> establishments =
            context.Get<IReadOnlyList<Establishment>>()
            ?? throw new InvalidOperationException(
                "PipelineContext does not contain establishments to order.");

        Dictionary<string, int> orderMap =
            context.Get<Dictionary<string, int>>()
            ?? throw new InvalidOperationException(
                "PipelineContext does not contain an order map.");

        List<Establishment> ordered = new(establishments.Count);

        foreach (Establishment establishment in establishments)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string urn = establishment.Urn
                ?? throw new InvalidOperationException(
                    "Establishment has a null URN and cannot be ordered.");

            if (!orderMap.TryGetValue(urn, out int index))
            {
                throw new InvalidOperationException(
                    $"Order map does not contain an entry for URN '{urn}'.");
            }

            ordered.Add(establishment);
        }

        ordered.Sort((establishmentLeft, establishmentRight) =>
            orderMap[establishmentLeft.Urn!]
                .CompareTo(orderMap[establishmentRight.Urn!]));

        context.Set(ordered);
    }
}