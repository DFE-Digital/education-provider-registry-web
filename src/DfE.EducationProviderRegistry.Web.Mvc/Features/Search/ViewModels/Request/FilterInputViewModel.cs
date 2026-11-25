namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Request;

public sealed class FilterInputViewModel
{
    public DateRangeInputFilterViewModel? DateRange { get; set; }
    public NumberRangeInputFilterViewModel? NumberRange { get; set; }
}
