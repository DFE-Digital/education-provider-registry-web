using OpenQA.Selenium;

namespace DfE.EducationProviderRegistry.Web.SharedTests.Features.PageComponents.WebDriver;

public static class WebElementExtensions
{
    public static GovUkTable ToGovUkTable(this IWebElement table)
    {
        string? caption = table
            .FindElements(By.CssSelector("caption"))
            .SingleOrDefault()?
            .Text
            .Trim();

        IReadOnlyDictionary<string, TextContent> rows =
            table.FindElements(By.CssSelector("tbody tr"))
                 .ToDictionary(
                     (row) => row.FindElement(By.CssSelector("th")).Text.Trim(),
                     (row) =>
                     {
                         IWebElement td = row.FindElement(By.CssSelector("td"));


                         IWebElement? a = td.TryFind(By.CssSelector("a"));

                         TextContent content = new()
                         {
                             Text = td.Text.Trim(),
                             Link = a is not null ?
                                new Link(
                                    url: a.GetAttribute("href") ?? null,
                                    securityAttributes: a.GetAttribute("rel")?.Split(" ", StringSplitOptions.RemoveEmptyEntries) ?? [],
                                    opensInNewWindow: a.GetAttribute("target") == "_blank")
                                : null
                         };

                         return content;
                     });

        return new GovUkTable
        {
            Caption = caption,
            Rows = rows
        };
    }

    public static IWebElement? TryFind(this IWebElement element, By by)
    {
        try
        {
            return element.FindElement(by);
        }
        catch (NoSuchElementException)
        {
            return null!;
        }
    }
}
