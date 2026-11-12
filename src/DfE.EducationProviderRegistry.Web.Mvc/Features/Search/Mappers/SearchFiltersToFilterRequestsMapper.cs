using System.Collections.ObjectModel;
using System.Globalization;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Request;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;

public sealed class SearchFiltersToFilterRequestsMapper : IMapper<SearchRequestViewModel, ReadOnlyCollection<FilterRequest>>
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
        DateRangeInputFilterViewModel range)
    {
        filters.Add(new FilterRequest(
            filterName: filterName,
            filterValues: [MapDateValue(range.From)!, MapDateValue(range.To)!]));
    }

    private static string? MapDateValue(DateInputViewModel input)
    {
        if (input.IsEmpty)
        {
            return null;
        }

        if (input.TryGetDate(out DateOnly date))
        {
            return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        return $"{input.Year}-{input.Month}-{input.Day}";
    }
}
