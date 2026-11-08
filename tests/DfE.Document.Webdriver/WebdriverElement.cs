using DfE.Document.Abstractions;
using OpenQA.Selenium;

namespace DfE.Document.WebDriver;

internal sealed class WebDriverElement : IElement
{
    private readonly IWebElement _element;

    public WebDriverElement(IWebElement element)
    {
        ArgumentNullException.ThrowIfNull(element);

        _element = element;
    }

    public string Text => _element.Text;

    public string? Attribute(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return _element.GetDomAttribute(name);
    }

    public IElement? Find(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);

        IWebElement? descendant = _element
            .FindElements(By.CssSelector(selector))
            .FirstOrDefault();

        return descendant is null
            ? null
            : new WebDriverElement(descendant);
    }

    public IReadOnlyCollection<IElement> FindAll(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);

        return _element
            .FindElements(By.CssSelector(selector))
            .Select(descendant => (IElement)new WebDriverElement(descendant))
            .ToArray();
    }
}
