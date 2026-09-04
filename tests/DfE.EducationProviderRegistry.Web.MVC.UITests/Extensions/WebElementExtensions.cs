using DfE.EducationProviderRegistry.Web.MVC.UITests.Components;
using OpenQA.Selenium;

namespace DfE.EducationProviderRegistry.Web.MVC.UITests.Extensions;

internal static class WebElementExtensions
{
    public static GovUkTable ToGovUkTable(this IWebElement table)
    {
        string? caption = table
            .FindElements(By.CssSelector("caption"))
            .SingleOrDefault()?
            .Text
            .Trim();

        IReadOnlyDictionary<string, string> rows =
            table.FindElements(By.CssSelector("tbody tr"))
                 .ToDictionary(
                     (row) => row.FindElement(By.CssSelector("th")).Text.Trim(),
                     (row) => row.FindElement(By.CssSelector("td")).Text.Trim());

        return new GovUkTable
        {
            Caption = caption,
            Rows = rows
        };
    }
}