using System.Collections.ObjectModel;
using Moq;
using OpenQA.Selenium;

namespace DfE.Document.WebDriver.UnitTests;

internal sealed class WebDriverElementBuilder
{
    private string _text;
    private readonly Dictionary<string, string?> _attributes;
    private readonly Dictionary<string, IReadOnlyCollection<IWebElement>> _descendants;

    public WebDriverElementBuilder()
    {
        _text = string.Empty;
        _attributes = [];
        _descendants = [];
    }

    public static WebDriverElementBuilder Create() => new();

    public WebDriverElementBuilder WithText(string value)
    {
        _text = value;

        return this;
    }

    public WebDriverElementBuilder WithAttribute(
        string name,
        string? value)
    {
        _attributes[name] = value;

        return this;
    }

    public WebDriverElementBuilder WithDescendant(
        string selector,
        IWebElement element)
    {
        _descendants[selector] = [element];

        return this;
    }

    public WebDriverElementBuilder WithDescendants(
        string selector,
        params IWebElement[] elements)
    {
        _descendants[selector] = elements;

        return this;
    }

    public IWebElement Build()
    {
        Mock<IWebElement> element = new();

        element
            .Setup(instance => instance.Text)
            .Returns(_text);

        element
            .Setup(instance => instance.GetDomAttribute(It.IsAny<string>()))
            .Returns(
                (string name) =>
                    _attributes.TryGetValue(name, out string? value)
                        ? value
                        : null);

        element
            .Setup(instance => instance.FindElements(It.IsAny<By>()))
            .Returns(
                (By by) =>
                {
                    string? selector = Selector(by);

                    IReadOnlyCollection<IWebElement> elements =
                        selector is not null &&
                        _descendants.TryGetValue(selector, out IReadOnlyCollection<IWebElement>? matches)
                            ? matches
                            : [];

                    return new ReadOnlyCollection<IWebElement>(
                        elements.ToArray());
                });

        return element.Object;
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
