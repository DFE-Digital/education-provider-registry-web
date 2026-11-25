using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace DfE.Document.Anglesharp.UnitTests;

public sealed class AngleSharpSearchContextTests
{
    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenSearchContextIsNull()
    {
        // Arrange
        Func<AngleSharpSearchContext> construct = () => new(null!);

        // Act Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Find_ThrowsArgumentException_WhenSelectorIsNull()
    {
        // Arrange
        AngleSharpSearchContext searchContext = CreateSearchContext(
            """
            <html>
                <body></body>
            </html>
            """);

        // Act Assert
        Assert.Throws<ArgumentNullException>(() => searchContext.Find(null!));
    }

    [Fact]
    public void Find_ThrowsArgumentException_WhenSelectorIsEmpty()
    {
        // Arrange
        AngleSharpSearchContext searchContext = CreateSearchContext(
            """
            <html>
                <body></body>
            </html>
            """);

        // Act
        Func<Abstractions.IElement?> find =
            () => searchContext.Find(string.Empty);

        // Assert
        Assert.Throws<ArgumentException>(find);
    }

    [Fact]
    public void Find_ThrowsArgumentException_WhenSelectorIsWhitespace()
    {
        // Arrange
        AngleSharpSearchContext searchContext = CreateSearchContext(
            """
            <html>
                <body></body>
            </html>
            """);

        // Act
        Func<Abstractions.IElement?> find =
            () => searchContext.Find(" ");

        // Assert
        Assert.Throws<ArgumentException>(find);
    }

    [Fact]
    public void Find_ReturnsElement_WhenSelectorMatches()
    {
        // Arrange
        AngleSharpSearchContext searchContext = CreateSearchContext(
            """
            <html>
                <body>
                    <h1 class="govuk-heading-xl">Test heading</h1>
                </body>
            </html>
            """);

        // Act
        Abstractions.IElement? element =
            searchContext.Find(".govuk-heading-xl");

        // Assert
        Assert.NotNull(element);
        Assert.Equal("Test heading", element.Text);
    }

    [Fact]
    public void Find_ReturnsNull_WhenSelectorDoesNotMatch()
    {
        // Arrange
        AngleSharpSearchContext searchContext = CreateSearchContext(
            """
            <html>
                <body></body>
            </html>
            """);

        // Act
        Abstractions.IElement? element =
            searchContext.Find(".govuk-heading-xl");

        // Assert
        Assert.Null(element);
    }

    [Fact]
    public void FindAll_ReturnsElements_WhenSelectorMatchesMultipleElements()
    {
        // Arrange
        AngleSharpSearchContext searchContext = CreateSearchContext(
            """
            <html>
                <body>
                    <a class="govuk-link">First</a>
                    <a class="govuk-link">Second</a>
                    <a class="govuk-link">Third</a>
                </body>
            </html>
            """);

        // Act
        IReadOnlyCollection<Abstractions.IElement> elements =
            searchContext.FindAll(".govuk-link");

        // Assert
        Assert.Equal(3, elements.Count);
        Assert.Collection(
            elements,
            element => Assert.Equal("First", element.Text),
            element => Assert.Equal("Second", element.Text),
            element => Assert.Equal("Third", element.Text));
    }

    [Fact]
    public void FindAll_ReturnsEmptyCollection_WhenSelectorDoesNotMatch()
    {
        // Arrange
        AngleSharpSearchContext searchContext = CreateSearchContext(
            """
            <html>
                <body></body>
            </html>
            """);

        // Act
        IReadOnlyCollection<Abstractions.IElement> elements =
            searchContext.FindAll(".govuk-link");

        // Assert
        Assert.Empty(elements);
    }

    private static AngleSharpSearchContext CreateSearchContext(string html)
    {
        HtmlParser parser = new();
        IDocument document = parser.ParseDocument(html);

        return new AngleSharpSearchContext(document);
    }
}
