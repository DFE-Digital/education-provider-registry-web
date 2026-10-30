using System.Collections.ObjectModel;
using System.Globalization;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;

public sealed class SelectedFacetsToFilterRequestsMapper :
    IMapper<Dictionary<string, List<string>>?, ReadOnlyCollection<FilterRequest>>
{
    /// <summary>
    /// Maps the posted facet selections into a read‑only collection of
    /// <see cref="FilterRequest"/> objects.
    /// </summary>
    /// <param name="selectedFacets">
    /// The dictionary of selected facet values posted from the UI. The key is the facet
    /// name, and the value is the list of selected values for that facet.
    /// </param>
    /// <returns>
    /// A read‑only collection of <see cref="FilterRequest"/> instances representing the
    /// selected filters. If <paramref name="selectedFacets"/> is <c>null</c> or empty,
    /// an empty collection is returned.
    /// </returns>
    /// <remarks>
    /// Each <see cref="FilterRequest"/> is created using the facet name and the selected
    /// values cast to <see cref="object"/> to match the search pipeline’s expected type.
    /// 
    /// Example transformation:
    /// <code>
    /// {
    ///     "Phase": ["Primary", "Secondary"],
    ///     "Type": ["Academy"]
    /// }
    /// </code>
    /// becomes:
    /// <code>
    /// [
    ///     new FilterRequest("Phase", ["Primary", "Secondary"]),
    ///     new FilterRequest("Type", ["Academy"])
    /// ]
    /// </code>
    /// </remarks>
    public ReadOnlyCollection<FilterRequest> Map(
        Dictionary<string, List<string>>? selectedFacets)
    {
        if (selectedFacets is null || selectedFacets.Count == 0)
        {
            return [];
        }

        List<FilterRequest> mapped =
        [
            .. selectedFacets.Select(kvp =>
            new FilterRequest(
                filterName: kvp.Key,
                filterValues: [.. kvp.Value.Cast<object>()]))
        ];

        return mapped.AsReadOnly();
    }
}


public sealed class SearchFiltersToFilterRequestsMapper
    : IMapper<SearchRequestViewModel, ReadOnlyCollection<FilterRequest>>
{
    private readonly IMapper<Dictionary<string, List<string>>?, ReadOnlyCollection<FilterRequest>> _facetMapper;

    public SearchFiltersToFilterRequestsMapper(
        IMapper<Dictionary<string, List<string>>?, ReadOnlyCollection<FilterRequest>> facetMapper)
    {
        ArgumentNullException.ThrowIfNull(facetMapper);
        _facetMapper = facetMapper;
    }

    public ReadOnlyCollection<FilterRequest> Map(SearchRequestViewModel input)
    {
        ArgumentNullException.ThrowIfNull(input);
        List<FilterRequest> filters = [.. _facetMapper.Map(input.SelectedFacets)];

        foreach (FilterViewModel filter in input.Filters)
        {
            switch (filter)
            {
                case DateRangeFilterViewModel dateRange:
                    AddDateRange(filters, dateRange.Name, dateRange);
                    break;
                default:
                    throw new NotImplementedException($"Mapping for filter type '{filter.GetType().Name}' is not implemented.");
            }
        }

        return filters.AsReadOnly();
    }

    private static void AddDateRange(
        List<FilterRequest> filters,
        string filterName,
        DateRangeFilterViewModel range)
    {
        if (!range.IsEmpty)
        {
            if (!range.TryGetRange(out DateOnly from, out DateOnly to))
            {
                throw new InvalidOperationException($"Invalid date range for filter '{filterName}'.");
            }

            filters.Add(new FilterRequest(
                filterName: filterName,
                filterValues:
                [
                    from.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    to.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                ]));
        }
    }
}
