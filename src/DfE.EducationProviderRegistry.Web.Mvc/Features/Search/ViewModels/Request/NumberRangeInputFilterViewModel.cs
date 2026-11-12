using System.ComponentModel.DataAnnotations;
namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Request;

public sealed class NumberRangeInputFilterViewModel
{
    public string? From { get; set; }

    public string? To { get; set; }

    public bool IsEmpty =>
        string.IsNullOrWhiteSpace(From) &&
            string.IsNullOrWhiteSpace(To);

    public string? SelectedLabel =>
        (Parse(From), Parse(To)) switch
        {
            (int from, int to) => $"{from} to {to}",
            (int from, null) => $"{from} and over",
            (null, int to) => $"Up to {to}",
            _ => null
        };

    private static int? Parse(string? value)
    {
        return int.TryParse(value, out int result) ? result : null;
    }
}
