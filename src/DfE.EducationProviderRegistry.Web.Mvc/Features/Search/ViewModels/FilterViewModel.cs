using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

public abstract class FilterViewModel
{
    [BindNever]
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string ViewName { get; set; } = string.Empty;
    public string BindingName { get; set; } = string.Empty;
    public string SelectedLabel { get; set; } = string.Empty;
    public bool IsSelected => !string.IsNullOrWhiteSpace(SelectedLabel);
}



public static class SearchFilterViewModelFactory
{
    public static List<FilterViewModel> Create(SearchRequestViewModel request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return
        [
            CreateDateRange(request, "opened_date", "Open date"),
            CreateDateRange(request, "closed_date", "Close date")
        ];
    }

    private static DateRangeFilterViewModel CreateDateRange(
        SearchRequestViewModel request,
        string name,
        string label)
    {
        request.DateRanges.TryGetValue(name, out DateRangeFilterViewModel? dateRange);

        DateRangeFilterViewModel filter = new()
        {
            Name = name,
            Label = label,
            ViewName = "_DateRangeFilter",
            BindingName = name,
            From = dateRange?.From ?? new DateInputViewModel(),
            To = dateRange?.To ?? new DateInputViewModel()
        };

        if (filter.TryGetRange(out DateOnly from, out DateOnly to))
        {
            filter.SelectedLabel = $"{from:dd/MM/yyyy} - {to:dd/MM/yyyy}";
        }

        return filter;
    }

}
