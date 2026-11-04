namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

public sealed class DateRangeFilterViewModel
{
    public DateInputViewModel From { get; set; } = new();
    public DateInputViewModel To { get; set; } = new();

    public bool IsEmpty => From.IsEmpty && To.IsEmpty;

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
                (true, true) => $"{from:dd/MM/yyyy} to {to:dd/MM/yyyy}",
                (true, false) => $"From {from:dd/MM/yyyy}",
                (false, true) => $"To {to:dd/MM/yyyy}",
                _ => null
            };
        }
    }
}
