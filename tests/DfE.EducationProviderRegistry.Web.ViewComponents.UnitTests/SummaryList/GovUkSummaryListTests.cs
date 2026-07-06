using DfE.EducationProviderRegistry.Web.ViewComponents.SummaryList;

namespace DfE.EducationProviderRegistry.Web.ViewComponents.UnitTests.SummaryList;

public sealed class GovUkSummaryListTests
{
    [Fact]
    public void Constructor_WhenRowsIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        IReadOnlyList<SummaryListRow>? rows = null;

        Func<GovUkSummaryList> construct = () => new GovUkSummaryList(rows!);

        // Act / Assert
        Assert.ThrowsAny<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_WhenRowsProvided_SetsRowsProperty()
    {
        // Arrange
        IReadOnlyList<SummaryListRow> rows =
        [
            new(
                "Key",
                new SummaryListValue("Value"))
        ];

        // Act
        GovUkSummaryList result = new(rows);

        // Assert
        Assert.Same(rows, result.Rows);
    }
}