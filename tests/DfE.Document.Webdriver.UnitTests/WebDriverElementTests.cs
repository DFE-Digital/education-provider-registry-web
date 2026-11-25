using DfE.Document.Abstractions;
using DfE.Document.WebDriver;
using DfE.Document.WebDriver.UnitTests;
using OpenQA.Selenium;

namespace DfE.Document.Webdriver.UnitTests;

public sealed class WebDriverElementTests
{
    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenElementIsNull()
    {
        // Arrange
        Func<WebDriverElement> construct = () => new(null!);

        // Act Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Text_ReturnsElementText()
    {
        // Arrange
        IWebElement webElement =
            WebDriverElementBuilder.Create()
                .WithText("There is a problem")
                .Build();

        WebDriverElement element = new(webElement);

        // Act
        string text = element.Text;

        // Assert
        Assert.Equal("There is a problem", text);
    }

    [Fact]
    public void Attribute_ThrowsArgumentNullException_WhenNameIsNull()
    {
        // Arrange
        IWebElement webElement =
            WebDriverElementBuilder.Create()
                .Build();

        WebDriverElement element = new(webElement);

        // Act
        Func<string?> attribute = () => element.Attribute(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(attribute);
    }

    [Fact]
    public void Attribute_ReturnsAttributeValue_WhenAttributeExists()
    {
        // Arrange
        IWebElement webElement =
            WebDriverElementBuilder.Create()
                .WithAttribute("name", "Email")
                .Build();

        WebDriverElement element = new(webElement);

        // Act
        string? attribute = element.Attribute("name");

        // Assert
        Assert.Equal("Email", attribute);
    }

    [Fact]
    public void Attribute_ReturnsNull_WhenAttributeDoesNotExist()
    {
        // Arrange
        IWebElement webElement =
            WebDriverElementBuilder.Create()
                .Build();

        WebDriverElement element = new(webElement);

        // Act
        string? attribute = element.Attribute("name");

        // Assert
        Assert.Null(attribute);
    }

    [Fact]
    public void Find_ReturnsDescendant_WhenSelectorMatches()
    {
        // Arrange
        IWebElement descendant = WebDriverElementBuilder.Create()
            .WithText("Enter an email address")
            .WithAttribute("href", "#email")
            .Build();

        IWebElement webElement = WebDriverElementBuilder.Create()
            .WithDescendant("a", descendant)
            .Build();

        WebDriverElement element = new(webElement);

        // Act
        IElement? result = element.Find("a");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Enter an email address", result.Text);
        Assert.Equal("#email", result.Attribute("href"));
    }

    [Fact]
    public void Find_ReturnsNull_WhenDescendantDoesNotMatch()
    {
        // Arrange
        IWebElement webElement = WebDriverElementBuilder.Create().Build();

        WebDriverElement element = new(webElement);

        // Act
        IElement? result = element.Find("a");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void FindAll_ReturnsMatchingDescendants()
    {
        // Arrange
        IWebElement first = WebDriverElementBuilder.Create()
            .WithText("First")
            .Build();

        IWebElement second = WebDriverElementBuilder.Create()
            .WithText("Second")
            .Build();

        IWebElement third = WebDriverElementBuilder.Create()
            .WithText("Third")
            .Build();

        IWebElement webElement = WebDriverElementBuilder.Create()
            .WithDescendants("li", first, second, third)
            .Build();

        WebDriverElement element = new(webElement);

        // Act
        IReadOnlyCollection<IElement> results = element.FindAll("li");

        // Assert
        Assert.Equal(3, results.Count);
        Assert.Collection(
            results,
            element => Assert.Equal("First", element.Text),
            element => Assert.Equal("Second", element.Text),
            element => Assert.Equal("Third", element.Text));
    }

    [Fact]
    public void FindAll_ReturnsEmptyCollection_WhenNoDescendantsMatch()
    {
        // Arrange
        IWebElement webElement = WebDriverElementBuilder.Create().Build();

        WebDriverElement element = new(webElement);

        // Act
        IReadOnlyCollection<IElement> results = element.FindAll("a");

        // Assert
        Assert.Empty(results);
    }
}
