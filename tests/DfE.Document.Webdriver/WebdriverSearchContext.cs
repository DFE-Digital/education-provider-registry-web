using DfE.Document.Abstractions;
using OpenQA.Selenium;

namespace DfE.Document.WebDriver;

public sealed class WebDriverSearchContext : Abstractions.ISearchContext
{
    private readonly OpenQA.Selenium.ISearchContext _context;

    public WebDriverSearchContext(OpenQA.Selenium.ISearchContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    public IElement? Find(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);

        IWebElement? element = _context
            .FindElements(By.CssSelector(selector))
            .FirstOrDefault();

        return element is null
            ? null
            : new WebDriverElement(element);
    }

    public IReadOnlyCollection<IElement> FindAll(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);

        return [.._context
                    .FindElements(By.CssSelector(selector))
                    .Select((element) =>
                        new WebDriverElement(element))];
    }
}
