using System.Globalization;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Request;

public sealed class DateRangeInputFilterViewModel
{
    private const string DateFormat = "dd/MM/yyyy";
    private static readonly CultureInfo _ukCulture = new("en-GB");

    public DateInputViewModel From { get; set; } = new();

    public DateInputViewModel To { get; set; } = new();

    public bool IsEmpty => From.IsEmpty && To.IsEmpty;

    public string? SelectedLabel
    {
        get
        {
            bool hasFrom =
                From.TryGetDate(out DateOnly from);

            bool hasTo =
                To.TryGetDate(out DateOnly to);

            return (hasFrom, hasTo) switch
            {
                (true, true) =>
                    $"{FormatDate(from)} to {FormatDate(to)}",

                (true, false) =>
                    $"From {FormatDate(from)}",

                (false, true) =>
                    $"To {FormatDate(to)}",

                _ =>
                    null
            };
        }
    }

    private static string FormatDate(DateOnly date)
    {
        return date.ToString(
            DateFormat,
            _ukCulture);
    }
}
