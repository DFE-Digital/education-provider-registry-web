namespace DfE.EducationProviderRegistry.Web.ViewComponents.Table
{
    public sealed record TableCellRow
    {
        public TableCell Label { get; init; } = new();

        public TableCell Value { get; init; } = new();
    }
}