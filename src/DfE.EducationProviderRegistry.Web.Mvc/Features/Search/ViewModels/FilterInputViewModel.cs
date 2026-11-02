using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

public sealed class FilterInputViewModel
{
    public DateRangeFilterViewModel? DateRange { get; set; }
    // NumberRangeFilterViewModel? NumberRange { get; set; }
    // MultiSelectFilterViewModel? MultiSelect { get; set; }
}
