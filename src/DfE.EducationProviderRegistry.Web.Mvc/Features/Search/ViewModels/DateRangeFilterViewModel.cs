namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

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
