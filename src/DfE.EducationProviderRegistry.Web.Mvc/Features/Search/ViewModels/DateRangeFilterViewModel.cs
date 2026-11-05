using System.Globalization;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

public sealed class DateRangeFilterViewModel
{
    public DateInputViewModel From { get; set; } = new();
    public DateInputViewModel To { get; set; } = new();

    public bool IsEmpty => From.IsEmpty && To.IsEmpty;

    private const string DateFormat = "dd/MM/yyyy";
    private static readonly CultureInfo UkCulture = new("en-GB");

    public string? SelectedLabel
    {
        get
        {
            bool hasFrom = From.TryGetDate(out DateOnly from);
            bool hasTo = To.TryGetDate(out DateOnly to);

            if ((!From.IsEmpty && !hasFrom) ||
                (!To.IsEmpty && !hasTo) ||
                (hasFrom && hasTo && from > to))
            {
                return null;
            }

            return (hasFrom, hasTo) switch
            {
                (true, true) => $"{FormatDate(from)} to {FormatDate(to)}",
                (true, false) => $"From {FormatDate(from)}",
                (false, true) => $"To {FormatDate(to)}",
                _ => null
            };
        }
    }

    private static string FormatDate(DateOnly date) => date.ToString(DateFormat, UkCulture);
}
