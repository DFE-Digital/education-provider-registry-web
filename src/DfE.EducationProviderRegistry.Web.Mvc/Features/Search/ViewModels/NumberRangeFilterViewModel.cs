using System.ComponentModel.DataAnnotations;
namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

public sealed class NumberRangeFilterViewModel
{
    public int? From { get; set; }
    public int? To { get; set; }

    public bool IsEmpty => From is null && To is null;

    public string? SelectedLabel => (From, To) switch
    {
        (int from, int to) => $"{from} to {to}",
        (int from, null) => $"{from} and over",
        (null, int to) => $"up to {to}",
        _ => null
    };
}
