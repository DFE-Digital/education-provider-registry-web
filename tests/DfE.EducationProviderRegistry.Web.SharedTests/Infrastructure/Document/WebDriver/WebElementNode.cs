using DfE.EducationProviderRegistry.Web.SharedTests.Infrastructure.Document.Abstractions;
using OpenQA.Selenium;

namespace DfE.EducationProviderRegistry.Web.SharedTests.Infrastructure.Document.WebDriver;

internal class WebElementNode : IElement
{
    private readonly IWebElement _element;

    public WebElementNode(IWebElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        _element = element;
    }

    public string Text => _element.Text;

    public string? Attribute(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return _element.GetAttribute(name);
    }

    public IElement? Find(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);

        IWebElement? element = _element
            .FindElements(By.CssSelector(selector))
            .FirstOrDefault();

        return element is null
            ? null
            : new WebElementNode(element);
    }

    public IReadOnlyCollection<IElement> FindAll(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);

        return _element
            .FindElements(
                By.CssSelector(selector))
            .Select(static (element) => (IElement)new WebElementNode(element))
            .ToList();
    }
}
