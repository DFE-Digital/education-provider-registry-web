using DfE.EducationProviderRegistry.Web.ViewComponents.Tabs;

namespace DfE.EducationProviderRegistry.Web.ViewComponents.UnitTests.Tabs;

public sealed class GovUkTabsTests
{
    [Fact]
    public void Constructor_WhenTabsIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        IEnumerable<Tab>? tabs = null;

        Func<GovUkTabs> construct = () => new GovUkTabs(tabs!);

        // Act / Assert
        Assert.ThrowsAny<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_WhenTabsIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        IEnumerable<Tab> tabs = [];

        Func<GovUkTabs> construct = () => new GovUkTabs(tabs);

        // Act / Assert
        Assert.ThrowsAny<ArgumentException>(construct);
    }

    [Fact]
    public void Constructor_WhenTabsProvided_SetsTabsProperty()
    {
        // Arrange
        IEnumerable<Tab> tabs =
        [
            new(
                "tab-1",
                "Tab 1",
                [
                    new TabContent(viewComponentName: "STUB")
                ])
        ];

        // Act
        GovUkTabs result = new(tabs);

        // Assert
        Assert.Same(tabs, result.Tabs);
    }
}