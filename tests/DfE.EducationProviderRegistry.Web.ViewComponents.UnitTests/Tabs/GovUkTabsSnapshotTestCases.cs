namespace DfE.EducationProviderRegistry.Web.ViewComponents.UnitTests.Tabs;

internal static class GovUkTabsSnapshotTestCases
{
    private const string GovUkTabsViewPath =
        "/Views/Shared/Components/GovUkTabs/Default.cshtml";

    public static IReadOnlyList<ViewComponentSnapshotTestCase> All =>
    [
        new(
            Name: $"Tabs_{nameof(GovUkTabsExamples.PastDayWeekMonthYear)}",
            ViewPath: GovUkTabsViewPath,
            Model: GovUkTabsExamples.PastDayWeekMonthYear())
    ];
}