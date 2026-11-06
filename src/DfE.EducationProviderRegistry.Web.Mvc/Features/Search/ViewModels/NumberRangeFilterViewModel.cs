using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

public sealed class NumberRangeFilterViewModel
{
    public int? From { get; set; }
    public int? To { get; set; }

    public bool IsEmpty => From is null && To is null;

    public string? SelectedLabel => (From, To) switch
    {
        (int from, int to) => $"Age range {from} to {to}",
        (int from, null) => $"Age range from {from}",
        (null, int to) => $"Age range up to {to}",
        _ => null
    };


}
