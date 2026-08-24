namespace DfE.EducationProviderRegistry.Web.ViewComponents.Table;

public class TableColumn
{
    public string Text { get; init; } = string.Empty;
    public bool IsRowHeader { get; init; }     // renders <th scope="row">
    public bool IsNumeric { get; init; }       // adds govuk-table__cell--numeric

    // Extensibility
    public string? Classes { get; init; }
}