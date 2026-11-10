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

        foreach ((string? name, FilterInputViewModel? filter) in input.Filters)
        {
            if (filter.DateRange is { IsEmpty: false } dateRange)
            {
                AddDateRange(filters, name, dateRange);
            }

            if (filter.NumberRange is { IsEmpty: false } ageRange)
            {
                filters.Add(new FilterRequest(
                    filterName: name,
                    filterValues: [ageRange.From!, ageRange.To!]));
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
            bool hasFrom = range.From.TryGetDate(out DateOnly from);
            bool hasTo = range.To.TryGetDate(out DateOnly to);

            if ((!range.From.IsEmpty && !hasFrom) ||
                (!range.To.IsEmpty && !hasTo) ||
                (hasFrom && hasTo && from > to))
            {
                throw new InvalidOperationException($"Invalid date range for filter '{filterName}'.");
            }

            filters.Add(new FilterRequest(
                filterName: filterName,
                filterValues:
                [
                    hasFrom ? from.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null!,
                    hasTo ? to.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null!
                ]));
        }
    }
}
