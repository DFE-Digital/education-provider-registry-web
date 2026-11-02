using System.Collections.ObjectModel;
using System.Globalization;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;

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

        foreach (var (name, range) in input.DateRanges)
            AddDateRange(filters, name, range);

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
