using DfE.EducationProviderRegistry.Web.ViewComponents.UnitTests.SummaryList;
using DfE.EducationProviderRegistry.Web.ViewComponents.UnitTests.Table;
using DfE.EducationProviderRegistry.Web.ViewComponents.UnitTests.Tabs;

namespace DfE.EducationProviderRegistry.Web.ViewComponents.UnitTests;

public sealed class ViewComponentSnapshotTests
{
    [Theory]
    [MemberData(nameof(SnapshotTestCases))]
    public async Task Render_ShouldMatchSnapshot(
        ViewComponentSnapshotTestCase test)
    {
        // Assert
        await HtmlMarkupVerify.VerifyAsync(
            testName: test.Name,
            viewPath: test.ViewPath,
            viewModel: test.Model);
    }


    public static TheoryData<ViewComponentSnapshotTestCase> SnapshotTestCases
    {
        get
        {
            TheoryData<ViewComponentSnapshotTestCase> data =
            [
                .. GovUkTableSnapshotTestCases.All
                    .Concat(GovUkSummaryListSnapshotTestCases.All)
                    .Concat(GovUkTabsSnapshotTestCases.All),
            ];

            return data;
        }
    }

}

public sealed record ViewComponentSnapshotTestCase(
    string Name,
    string ViewPath,
    object? Model,
    string? SnapshotDirectory = null);