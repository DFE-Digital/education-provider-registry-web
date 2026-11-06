namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

public sealed class FilterInputViewModel
{
    public DateRangeFilterViewModel? DateRange { get; set; }
    public NumberRangeFilterViewModel? NumberRange { get; set; }
}
