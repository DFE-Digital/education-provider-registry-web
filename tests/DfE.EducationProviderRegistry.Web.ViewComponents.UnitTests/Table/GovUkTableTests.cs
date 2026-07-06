using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

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
                    new TableColumn("Column")
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
                    new TableColumn(text)
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
                    new TableColumn("Column 1")
                    {
                        IsRowHeader = true
                    },
                    new TableColumn("Column 2")
                    {
                        IsRowHeader = true
                    }
                ],
                rows:
                [
                    [
                        new TableCell(),
                        new TableCell()
                    ]
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
                    new TableColumn("Column")
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
                    new TableColumn("Column 1"),
                    new TableColumn("Column 2")
                ],
                rows:
                [
                    [
                        new TableCell()
                    ]
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
            new TableColumn("Month")
            {
                IsRowHeader = true
            },
            new TableColumn("Amount")
        ];

        IReadOnlyList<IReadOnlyList<TableCell>> rows =
        [
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
            new TableColumn("Month")
            {
                IsRowHeader = true
            },
            new TableColumn("Amount")
        ];

        IReadOnlyList<IReadOnlyList<TableCell>> rows =
        [
            [
                new TableCell(),
                new TableCell()
            ]
        ];

        // Act
        GovUkTable result = new(
            columns,
            rows);

        // Assert
        Assert.NotNull(result);
    }
}