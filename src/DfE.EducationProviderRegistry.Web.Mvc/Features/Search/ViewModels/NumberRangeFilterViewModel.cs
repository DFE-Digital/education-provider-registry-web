using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

[ExcludeFromCodeCoverage]
public sealed class NumberRangeFilterViewModel
{
    public int? From { get; set; }
    public int? To { get; set; }
}
