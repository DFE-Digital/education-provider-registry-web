namespace DfE.EducationProviderRegistry.Web.ViewComponents.UnitTests.Table;

internal static class GovUkTableSnapshotTestCases
{
    private const string DefaultView =
        "/Views/Shared/Components/SharedGovUkTable/Default.cshtml";

    public static IReadOnlyList<ViewComponentSnapshotTestCase> All =>
    [
        new(
            Name: $"Table_{nameof(GovUkTablesExamples.MonthsAndRates)}",
            ViewPath: DefaultView,
            Model: GovUkTablesExamples.MonthsAndRates()),

        new(
            Name: $"Table_{nameof(GovUkTablesExamples.DatesAndAmounts)}",
            ViewPath: DefaultView,
            Model: GovUkTablesExamples.DatesAndAmounts()),

        new(
            Name: $"Table_{nameof(GovUkTablesExamples.CaseStatistics)}",
            ViewPath: DefaultView,
            Model: GovUkTablesExamples.CaseStatistics()),

        new(
            Name: $"Table_{nameof(GovUkTablesExamples.NoRowHeaders)}",
            ViewPath: DefaultView,
            Model: GovUkTablesExamples.NoRowHeaders()),

        new(
            Name: $"Table_{nameof(GovUkTablesExamples.NoCaption)}",
            ViewPath: DefaultView,
            Model: GovUkTablesExamples.NoCaption()),

        new(
            Name: $"Table_{nameof(GovUkTablesExamples.LargeCaption)}",
            ViewPath: DefaultView,
            Model: GovUkTablesExamples.LargeCaption())
    ];
}