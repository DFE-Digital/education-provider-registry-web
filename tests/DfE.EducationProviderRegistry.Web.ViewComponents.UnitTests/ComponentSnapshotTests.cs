using DfE.EducationProviderRegistry.Web.ViewComponents.Table;
using DfE.EducationProviderRegistry.Web.ViewComponents.UnitTests.Table;

namespace DfE.EducationProviderRegistry.Web.ViewComponents.UnitTests;

public sealed class ComponentSnapshotTests
{
    [Fact]
    public async Task Render_ShouldMatchSnapshot()
    {
        // Arrange
        GovUkTable model = GovUkTablesExamples.MonthsAndRates();
        using ViewComponentRenderer renderer = new();

        // Act
        string html =
            await renderer.RenderAsync(
                viewPath: "/Views/Shared/Components/SharedGovUkTable/Default.cshtml", model);

        // Assert
        await VerifyHtml.Verify(html);
    }
}