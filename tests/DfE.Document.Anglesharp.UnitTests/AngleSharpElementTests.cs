using DfE.Document.Abstractions;

namespace DfE.Document.Anglesharp.UnitTests;

public sealed class AngleSharpElementTests
{
    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenElementIsNull()
    {
        Func<AngleSharpElement> construct = () => new AngleSharpElement(null!);

        // Act Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Text_ReturnsElementTextContent()
    {
        // Arrange
        AngleSharpElement element = AnglesSharpElementTestDoubles.CreateElement(
            """
            <div class="govuk-error-summary">
                There is a problem
            </div>
            """,
            ".govuk-error-summary");

        // Act
        string text = element.Text;

        // Assert
        Assert.Contains("There is a problem", text);
    }

    [Fact]
    public void Attribute_ThrowsArgumentException_WhenNameIsNull()
    {
        // Arrange
        AngleSharpElement element = AnglesSharpElementTestDoubles.CreateElement(
            "<input />",
            "input");

        // Act
        Func<string?> attribute =
            () => element.Attribute(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(attribute);
    }

    [Fact]
    public void Attribute_ReturnsAttributeValue_WhenAttributeExists()
    {
        // Arrange
        AngleSharpElement element = AnglesSharpElementTestDoubles.CreateElement(
            """
            <input id="email" name="Email" type="email" />
            """,
            "#email");

        // Act
        string? attribute =
            element.Attribute("name");

        // Assert
        Assert.Equal("Email", attribute);
    }

    [Fact]
    public void Attribute_ReturnsNull_WhenAttributeDoesNotExist()
    {
        // Arrange
        AngleSharpElement element = AnglesSharpElementTestDoubles.CreateElement(
            """
            <input id="email" type="email" />
            """,
            "#email");

        // Act
        string? attribute =
            element.Attribute("name");

        // Assert
        Assert.Null(attribute);
    }

    [Fact]
    public void Find_ReturnsDescendant_WhenSelectorMatches()
    {
        // Arrange
        AngleSharpElement element = AnglesSharpElementTestDoubles.CreateElement(
            """
            <div class="govuk-error-summary">
                <div class="govuk-error-summary__body">
                    <a href="#email">Enter an email address</a>
                </div>
            </div>
            """,
            ".govuk-error-summary");

        // Act
        IElement? result =
            element.Find("a");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Enter an email address", result.Text);
        Assert.Equal("#email", result.Attribute("href"));
    }

    [Fact]
    public void Find_ReturnsNull_WhenDescendantDoesNotMatch()
    {
        // Arrange
        AngleSharpElement element = AnglesSharpElementTestDoubles.CreateElement(
            """
            <div class="govuk-error-summary">
                <span>Error</span>
            </div>
            """,
            ".govuk-error-summary");

        // Act
        IElement? result =
            element.Find("a");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void FindAll_ReturnsMatchingDescendants()
    {
        // Arrange
        AngleSharpElement element = AnglesSharpElementTestDoubles.CreateElement(
            """
            <ul class="govuk-list">
                <li>First</li>
                <li>Second</li>
                <li>Third</li>
            </ul>
            """,
            ".govuk-list");

        // Act
        IReadOnlyCollection<IElement> results =
            element.FindAll("li");

        // Assert
        Assert.Equal(3, results.Count);
        Assert.Collection(
            results,
            element => Assert.Equal("First", element.Text),
            element => Assert.Equal("Second", element.Text),
            element => Assert.Equal("Third", element.Text)
        );
    }

    [Fact]
    public void FindAll_ReturnsEmptyCollection_WhenNoDescendantsMatch()
    {
        // Arrange
        AngleSharpElement element = AnglesSharpElementTestDoubles.CreateElement(
            """
            <div class="govuk-error-summary">
                <span>Error</span>
            </div>
            """,
            ".govuk-error-summary");

        // Act
        IReadOnlyCollection<IElement> results =
            element.FindAll("a");

        // Assert
        Assert.Empty(results);
    }
}
