using DfE.Document.Abstractions;
using OpenQA.Selenium;
using ISearchContext = DfE.Document.Abstractions.ISearchContext;

namespace DfE.Document.Webdriver;

internal sealed class WebDriverSearchContext : ISearchContext
{
    private readonly OpenQA.Selenium.ISearchContext _searchContext;

    public WebDriverSearchContext(OpenQA.Selenium.ISearchContext searchContext)
    {
        ArgumentNullException.ThrowIfNull(searchContext);
        _searchContext = searchContext;
    }

    public IElement? Find(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);

        IWebElement? element = _searchContext
            .FindElements(By.CssSelector(selector))
            .FirstOrDefault();

        return element is null
            ? null
            : new WebDriverElement(element);
    }

    public IReadOnlyCollection<IElement> FindAll(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);

        return _searchContext
            .FindElements(By.CssSelector(selector))
            .Select(element => new WebDriverElement(element))
            .ToArray();
    }
}
