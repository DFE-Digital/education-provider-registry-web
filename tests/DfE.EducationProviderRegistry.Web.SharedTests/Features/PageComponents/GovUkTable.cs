namespace DfE.EducationProviderRegistry.Web.SharedTests.Features.PageComponents;

public sealed class GovUkTable
{
    public string? Caption { get; init; }

    public IReadOnlyDictionary<string, TextContent> Rows { get; init; } = new Dictionary<string, TextContent>();

    public TextContent? this[int key]
        => Rows.ElementAtOrDefault(key).Value;
}
