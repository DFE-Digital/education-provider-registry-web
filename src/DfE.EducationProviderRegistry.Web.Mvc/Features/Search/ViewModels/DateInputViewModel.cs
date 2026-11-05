using System.Globalization;
namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

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

        string? dayInput = Day?.Trim();
        string? monthInput = Month?.Trim();
        string? yearInput = Year?.Trim();

        if (yearInput is not { Length: 4 }
            || !int.TryParse(yearInput, NumberStyles.None, CultureInfo.CurrentCulture, out int year)
            || !int.TryParse(monthInput, NumberStyles.None, CultureInfo.CurrentCulture, out int month)
            || !int.TryParse(dayInput, NumberStyles.None, CultureInfo.CurrentCulture, out int day)
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
