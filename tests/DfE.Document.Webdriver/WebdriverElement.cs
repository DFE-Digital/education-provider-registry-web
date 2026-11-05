using DfE.Document.Abstractions;
using OpenQA.Selenium;

namespace DfE.Document.Webdriver;

internal sealed class WebDriverElement : IElement
{
    private readonly IWebElement __element;

    public WebDriverElement(IWebElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        __element = element;
    }

    public string Text => __element.Text;

    public string? Attribute(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return __element.GetDomAttribute(name);
    }

    public IElement? Find(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);

        IWebElement? descendant = __element
            .FindElements(By.CssSelector(selector))
            .FirstOrDefault();

        return descendant is null
            ? null
            : new WebDriverElement(descendant);
    }

    public IReadOnlyCollection<IElement> FindAll(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);

        return __element
            .FindElements(By.CssSelector(selector))
            .Select(descendant => (IElement)new WebDriverElement(descendant))
            .ToArray();
    }
}
