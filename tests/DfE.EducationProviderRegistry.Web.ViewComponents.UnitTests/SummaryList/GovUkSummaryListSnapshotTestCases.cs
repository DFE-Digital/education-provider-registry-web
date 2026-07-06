namespace DfE.EducationProviderRegistry.Web.ViewComponents.UnitTests.SummaryList;

internal static class GovUkSummaryListSnapshotTestCases
{
    private const string DefaultViewPath =
        "/Views/Shared/Components/GovUkSummaryList/Default.cshtml";

    public static IReadOnlyList<ViewComponentSnapshotTestCase> All =>
    [
        new(
            Name: $"SummaryList_{nameof(GovUkSummaryListExamples.Basic)}",
            ViewPath: DefaultViewPath,
            Model: GovUkSummaryListExamples.Basic()),

        new(
            Name: $"SummaryList_{nameof(GovUkSummaryListExamples.WithActions)}",
            ViewPath: DefaultViewPath,
            Model: GovUkSummaryListExamples.WithActions()),

        new(
            Name: $"SummaryList_{nameof(GovUkSummaryListExamples.MixedActions)}",
            ViewPath: DefaultViewPath,
            Model: GovUkSummaryListExamples.MixedActions()),

        new(
            Name: $"SummaryList_{nameof(GovUkSummaryListExamples.ContactDetails)}",
            ViewPath: DefaultViewPath,
            Model: GovUkSummaryListExamples.ContactDetails())
    ];
}