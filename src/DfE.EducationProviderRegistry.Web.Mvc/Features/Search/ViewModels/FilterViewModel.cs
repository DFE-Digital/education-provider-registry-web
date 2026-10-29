using System.Globalization;
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


public sealed class DateRangeFilterViewModel : FilterViewModel
{
    public DateInputViewModel From { get; set; } = new();
    public DateInputViewModel To { get; set; } = new();

    public bool IsEmpty => From.IsEmpty && To.IsEmpty;


    public bool TryGetRange(out DateOnly from, out DateOnly to)
    {
        bool validFrom = From.TryGetDate(out from);
        bool validateTo = To.TryGetDate(out to);
        return validFrom && validateTo && from <= to;
    }
}


public sealed class DateInputViewModel
{
    public string? Day { get; set; }
    public string? Month { get; set; }
    public string? Year { get; set; }

    public bool IsEmpty => string.IsNullOrWhiteSpace(Day) &&
        string.IsNullOrWhiteSpace(Month) &&
        string.IsNullOrWhiteSpace(Year);

    public bool TryGetDate(out DateOnly date)
    {
        date = default;

        if (Year?.Trim().Length != 4
            || !int.TryParse(Year?.Trim(), NumberStyles.None, CultureInfo.CurrentCulture, out int year)
            || !int.TryParse(Month?.Trim(), NumberStyles.None, CultureInfo.CurrentCulture, out int month)
            || !int.TryParse(Day?.Trim(), NumberStyles.None, CultureInfo.CurrentCulture, out int day)
            || year is < 1 or > 9999
            || month is < 1 or > 12
            || day is < 1
            || day > DateTime.DaysInMonth(year, month))
        {
            return false;
        }
        date = new DateOnly(year, month, day);
        return true;
    }
}



public static class SearchFilterViewModelFactory
{
    public static List<FilterViewModel> Create(SearchRequestViewModel request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return
        [
            CreateDateRange(request, "opened_date", "Open date")
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
