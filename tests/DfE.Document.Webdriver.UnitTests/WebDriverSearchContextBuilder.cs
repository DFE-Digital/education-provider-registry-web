using System.Collections.ObjectModel;
using Moq;
using OpenQA.Selenium;

namespace DfE.Document.WebDriver.UnitTests;

internal sealed class WebDriverSearchContextBuilder
{
    private readonly Dictionary<string, IReadOnlyCollection<IWebElement>> _elements;

    public WebDriverSearchContextBuilder()
    {
        _elements = [];
    }

    public static WebDriverSearchContextBuilder Create() => new();
    public WebDriverSearchContextBuilder WithElement(
        string selector,
        IWebElement element)
    {
        _elements[selector] = [element];

        return this;
    }

    public WebDriverSearchContextBuilder WithElements(
        string selector,
        params IWebElement[] matchingElements)
    {
        _elements[selector] = matchingElements;

        return this;
    }

    public ISearchContext Build()
    {
        Mock<ISearchContext> searchContext = new();

        searchContext
            .Setup(instance => instance.FindElements(It.IsAny<By>()))
            .Returns(
                (By by) =>
                {
                    string? selector = Selector(by);

                    IReadOnlyCollection<IWebElement> matches =
                        selector is not null &&
                        _elements.TryGetValue(selector, out IReadOnlyCollection<IWebElement>? configured)
                            ? configured
                            : [];

                    return new ReadOnlyCollection<IWebElement>(
                        matches.ToArray());
                });

        return searchContext.Object;
    }

    private static string? Selector(By by)
    {
        const string prefix = "By.CssSelector: ";

        string value = by.ToString();

        return value.StartsWith(prefix, StringComparison.Ordinal)
            ? value[prefix.Length..]
            : null;
    }
}
