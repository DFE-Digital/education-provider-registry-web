namespace DfE.EducationProviderRegistry.Web.ViewComponents.Table;

public class TableColumn
{
    public TableColumn(string text)
    {
        Text = text ?? string.Empty;
    }

    public string Text { get; }
    public bool IsRowHeader { get; init; }     // renders <th scope="row">
    public bool IsNumeric { get; init; }       // adds govuk-table__cell--numeric

    // Extensibility
    public string? Classes { get; init; }
}