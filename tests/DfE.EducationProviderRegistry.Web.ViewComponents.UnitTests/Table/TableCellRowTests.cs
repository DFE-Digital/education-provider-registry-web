using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.ViewComponents.UnitTests.Table;

public sealed class TableCellRowTests
{
    [Fact]
    public void Constructor_InitializesLabelAndValue()
    {
        // Act
        TableCellRow row = new();

        // Assert
        Assert.NotNull(row.Label);
        Assert.NotNull(row.Value);
        Assert.NotSame(row.Label, row.Value);
    }

    [Fact]
    public void Properties_CanBeInitialized()
    {
        // Arrange
        TableCell label = new()
        {
            Text = "testLabel"
        };

        TableCell value = new()
        {
            Text = "testValue"
        };

        // Act
        TableCellRow row = new()
        {
            Label = label,
            Value = value
        };

        // Assert
        Assert.Same(label, row.Label);
        Assert.Same(value, row.Value);
        Assert.Equal("testLabel", row.Label.Text);
        Assert.Equal("testValue", row.Value.Text);
    }
}