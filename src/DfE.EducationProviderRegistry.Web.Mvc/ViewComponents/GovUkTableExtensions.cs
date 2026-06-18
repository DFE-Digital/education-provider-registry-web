namespace DfE.EducationProviderRegistry.Web.Mvc.ViewComponents
{
    /// <summary>
    /// Provides extension methods for building <see cref="GovUkTable"/> instances
    /// in a consistent and reusable way across the web application.
    /// </summary>
    public static class GovUkTableExtensions
    {
        /// <summary>
        /// Adds a new row to the specified <see cref="GovUkTable"/> using a
        /// standard two‑column layout where the first cell is a bold label
        /// and the second cell contains the corresponding value.
        /// </summary>
        /// <param name="table">
        /// The <see cref="GovUkTable"/> instance to which the row will be added.
        /// </param>
        /// <param name="label">
        /// The text displayed in the first cell of the row. This cell is rendered
        /// in bold to indicate a field name or descriptor.
        /// </param>
        /// <param name="value">
        /// The text displayed in the second cell of the row. Represents the value
        /// associated with the label.
        /// </param>
        /// <param name="linkUrl">
        /// An optional URL that, when provided, renders the value cell as a link
        /// instead of plain text. If <c>null</c>, the value is rendered as text.
        /// </param>
        public static void AddRow(
            this GovUkTable table,
            string label,
            string value,
            string? linkUrl = null)
        {
            table.Rows.Add(new GovUkTableRow
            {
                Cells =
                [
                    new GovUkTableCell { Text = label, IsBold = true },
                    new GovUkTableCell { Text = value, LinkUrl = linkUrl }
                ]
            });
        }
    }
}
