namespace DfE.EducationProviderRegistry.Web.MVC.UITests.Components;

internal sealed class GovUkTable
{
    public string? Caption { get; init; }

    public IReadOnlyDictionary<string, string> Rows { get; init; } = new Dictionary<string, string>();

    public string? this[string key]
        => Rows.TryGetValue(key, out string? value) ?
            value : null;

    public string? this[int key]
        => Rows.ElementAtOrDefault(key).Value;
}