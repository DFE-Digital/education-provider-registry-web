namespace DfE.EducationProviderRegistry.Web.PageComponents.GovUk;

public sealed class GovUkTable
{
    public string? Caption { get; init; }

    public IReadOnlyList<GovUkTableRow> Rows { get; init; } = [];
}

public sealed class GovUkTableRow
{
    public IReadOnlyList<TextContent> Cells { get; init; } = [];

    public TextContent this[int index] => Cells[index];
}
