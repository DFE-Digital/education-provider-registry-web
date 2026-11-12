using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Filters;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;

public sealed record AvailableFiltersMappingContext(IReadOnlyCollection<string> AvailableFilters, SearchRequestViewModel SearchRequest);

public sealed class AvailableFiltersToViewModelMapper : IMapper<AvailableFiltersMappingContext, IReadOnlyCollection<IFilterViewModel>>
{
    public IReadOnlyCollection<IFilterViewModel> Map(AvailableFiltersMappingContext input)
    {
        ArgumentNullException.ThrowIfNull(input);

        List<IFilterViewModel> filters = new(input.AvailableFilters.Count);

        foreach (string filterKey in input.AvailableFilters)
        {
            filters.Add(
                MapFilter(
                    filterKey,
                    input.SearchRequest));
        }

        return filters;
    }

    private static IFilterViewModel MapFilter(
        string filterKey,
        SearchRequestViewModel searchRequest)
    {
        return filterKey.ToLowerInvariant() switch
        {
            "opened_date" =>
                new DateRangeFilterViewModel(
                    Name: filterKey,
                    Label: "Opened date",
                    Range: DateRange(
                        searchRequest,
                        filterKey)),

            "closed_date" =>
                new DateRangeFilterViewModel(
                    Name: filterKey,
                    Label: "Closed date",
                    Range: DateRange(
                        searchRequest,
                        filterKey)),

            "age_range" =>
                new NumberRangeFilterViewModel(
                    Name: filterKey,
                    Label: "Age range",
                    Range: NumberRange(
                        searchRequest,
                        filterKey)),

            _ => throw new InvalidOperationException($"No Web filter presentation is configured for '{filterKey}'.")
        };
    }

    private static DateRangeInputFilterViewModel DateRange(
        SearchRequestViewModel searchRequest,
        string filterKey)
    {
        return searchRequest.Filters
            .GetValueOrDefault(filterKey)?.DateRange ?? new DateRangeInputFilterViewModel();
    }

    private static NumberRangeInputFilterViewModel NumberRange(
        SearchRequestViewModel searchRequest,
        string filterKey)
    {
        return searchRequest.Filters
            .GetValueOrDefault(filterKey)? .NumberRange ?? new NumberRangeInputFilterViewModel();
    }
}
