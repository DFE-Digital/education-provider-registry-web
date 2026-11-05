using DfE.Document.Abstractions;
using DfE.Document.WebDriver.UnitTests;
using OpenQA.Selenium;

namespace DfE.Document.Webdriver.UnitTests;

public sealed class WebDriverSearchContextTests
{
    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenSearchContextIsNull()
    {
        // Arrange
        Func<WebDriverSearchContext> construct = () => new(null!);

        // Act Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Find_ThrowsArgumentNullException_WhenSelectorIsNull()
    {
        // Arrange
        OpenQA.Selenium.ISearchContext seleniumSearchContext =
            WebDriverSearchContextBuilder.Create().Build();

        WebDriverSearchContext searchContext = new(seleniumSearchContext);

        // Act Assert
        Assert.Throws<ArgumentNullException>(
            () => searchContext.Find(null!));
    }

    [Fact]
    public void Find_ThrowsArgumentException_WhenSelectorIsEmpty()
    {
        // Arrange
        OpenQA.Selenium.ISearchContext seleniumSearchContext =
            WebDriverSearchContextBuilder.Create().Build();

        WebDriverSearchContext searchContext = new(seleniumSearchContext);

        // Act
        Func<IElement?> find =
            () => searchContext.Find(string.Empty);

        // Assert
        Assert.Throws<ArgumentException>(find);
    }

    [Fact]
    public void Find_ThrowsArgumentException_WhenSelectorIsWhitespace()
    {
        // Arrange
        OpenQA.Selenium.ISearchContext seleniumSearchContext =
            WebDriverSearchContextBuilder.Create().Build();

        WebDriverSearchContext searchContext = new(seleniumSearchContext);

        // Act
        Func<IElement?> find =
            () => searchContext.Find(" ");

        // Assert
        Assert.Throws<ArgumentException>(find);
    }

    [Fact]
    public void Find_ReturnsElement_WhenSelectorMatches()
    {
        // Arrange
        IWebElement webElement = WebDriverElementBuilder.Create()
            .WithText("Test heading")
            .Build();

        OpenQA.Selenium.ISearchContext seleniumSearchContext =
            WebDriverSearchContextBuilder.Create()
                .WithElement(".govuk-heading-xl", webElement)
                .Build();

        WebDriverSearchContext searchContext = new(seleniumSearchContext);

        // Act
        IElement? element =
            searchContext.Find(".govuk-heading-xl");

        // Assert
        Assert.NotNull(element);
        Assert.Equal("Test heading", element.Text);
    }

    [Fact]
    public void Find_ReturnsFirstElement_WhenSelectorMatchesMultipleElements()
    {
        // Arrange
        IWebElement first =
            WebDriverElementBuilder.Create()
                .WithText("First")
                .Build();

        IWebElement second = WebDriverElementBuilder.Create()
            .WithText("Second")
            .Build();

        OpenQA.Selenium.ISearchContext seleniumSearchContext =
            WebDriverSearchContextBuilder.Create()
                .WithElements(
                    ".govuk-heading-xl",
                    first,
                    second)
                .Build();

        WebDriverSearchContext searchContext = new(seleniumSearchContext);

        // Act
        IElement? element =
            searchContext.Find(".govuk-heading-xl");

        // Assert
        Assert.NotNull(element);
        Assert.Equal("First", element.Text);
    }

    [Fact]
    public void Find_ReturnsNull_WhenSelectorDoesNotMatch()
    {
        // Arrange
        OpenQA.Selenium.ISearchContext seleniumSearchContext =
            WebDriverSearchContextBuilder.Create().Build();

        WebDriverSearchContext searchContext = new(seleniumSearchContext);

        // Act
        IElement? element =
            searchContext.Find(".govuk-heading-xl");

        // Assert
        Assert.Null(element);
    }

    [Fact]
    public void FindAll_ReturnsElements_WhenSelectorMatchesMultipleElements()
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

        OpenQA.Selenium.ISearchContext seleniumSearchContext =
            WebDriverSearchContextBuilder.Create()
                .WithElements(".govuk-link", first, second, third)
                .Build();

        WebDriverSearchContext searchContext = new(seleniumSearchContext);

        // Act
        IReadOnlyCollection<IElement> elements =
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
        OpenQA.Selenium.ISearchContext seleniumSearchContext =
            WebDriverSearchContextBuilder.Create().Build();

        WebDriverSearchContext searchContext = new(seleniumSearchContext);

        // Act
        IReadOnlyCollection<IElement> elements =
            searchContext.FindAll(".govuk-link");

        // Assert
        Assert.Empty(elements);
    }
}
