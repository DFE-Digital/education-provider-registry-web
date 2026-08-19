using DfE.EducationProviderRegistry.Web.ViewComponents.Table;
using static System.Net.Mime.MediaTypeNames;

namespace DfE.EducationProviderRegistry.Web.ViewComponents.UnitTests.Table;

public sealed class GovUkTableTests
{
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenColumnsAreNull()
    {
        // Arrange
        Func<GovUkTable> construct =
            () => new GovUkTable(
                columns: null!,
                rows: []);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenRowsAreNull()
    {
        // Arrange
        Func<GovUkTable> construct =
            () => new GovUkTable(
                columns:
                [
                    new TableColumn { Text = "Column" }
                ],
                rows: null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenNoColumnsProvided()
    {
        // Arrange
        Func<GovUkTable> construct =
            () => new GovUkTable(
                columns: [],
                rows: []);

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(construct);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenColumnIsNull()
    {
        // Arrange
        Func<GovUkTable> construct =
            () => new GovUkTable(
                columns:
                [
                    null!
                ],
                rows: []);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ShouldThrowArgumentException_WhenColumnTextIsInvalid(
        string text)
    {
        // Arrange
        Func<GovUkTable> construct =
            () => new GovUkTable(
                columns:
                [
                    new TableColumn { Text = text }
                ],
                rows: []);

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(construct);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenMoreThanOneRowHeaderColumnProvided()
    {
        // Arrange
        Func<GovUkTable> construct =
            () => new GovUkTable(
                columns:
                [
                    new TableColumn
                    {
                        IsRowHeader = true,
                        Text = "Column 1"
                    },
                    new TableColumn
                    {
                        IsRowHeader = true,
                        Text = "Column 2"
                    }
                ],
                rows:
                [
                    new TableRow { Cells = { new TableCell(), new TableCell() } }
                ]);

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(construct);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenRowIsNull()
    {
        // Arrange
        Func<GovUkTable> construct =
            () => new GovUkTable(
                columns:
                [
                    new TableColumn{ Text = "Column" }
                ],
                rows:
                [
                    null!
                ]);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenRowLengthDoesNotMatchColumnCount()
    {
        // Arrange
        Func<GovUkTable> construct =
            () => new GovUkTable(
                columns:
                [
                    new TableColumn { Text = "Column 1" },
                    new TableColumn { Text = "Column 2" }
                ],
                rows:
                [
                    new TableRow { Cells = { new TableCell() } }
                ]);

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(construct);
    }

    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        // Arrange
        IReadOnlyList<TableColumn> columns =
        [
            new TableColumn
            {
                IsRowHeader = true,
                Text = "Month"
            },
            new TableColumn{ Text = "Amount" }
        ];

        IReadOnlyList<TableRow> rows =
        [
            new TableRow
            {
                Cells =
                [
                    new TableCell
                    {
                        Text = "January"
                    },
                    new TableCell
                    {
                        Text = "100"
                    }
                ]
            }
        ];

        // Act
        GovUkTable result = new(
            columns,
            rows,
            caption: "Payments",
            captionSize: TableCaptionSize.Large);

        // Assert
        Assert.Same(columns, result.Columns);
        Assert.Same(rows, result.Rows);
        Assert.Equal("Payments", result.Caption);
        Assert.Equal(TableCaptionSize.Large, result.CaptionSize);
    }

    [Fact]
    public void Constructor_ShouldAllowSingleRowHeaderColumn()
    {
        // Arrange
        IReadOnlyList<TableColumn> columns =
        [
            new TableColumn
            {
                IsRowHeader = true,
                Text = "Month"
            },
            new TableColumn { Text = "Amount" }
        ];

        IReadOnlyList<TableRow> rows =
        [
            new TableRow { Cells = { new TableCell(), new TableCell() } }
        ];

        // Act
        GovUkTable result = new(
            columns,
            rows);

        // Assert
        Assert.NotNull(result);
    }
}