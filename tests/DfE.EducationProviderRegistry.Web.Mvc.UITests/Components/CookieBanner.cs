using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace DfE.EducationProviderRegistry.Web.Mvc.UITests.Components;

internal sealed class CookieBanner
{
    private readonly WebDriverWait _defaultWait;

    public CookieBanner(IWebDriver driver)
    {
        _defaultWait = new(driver, TimeSpan.FromSeconds(15));
    }

    public void Reject()
    {
        _defaultWait.ClickOn(By.CssSelector(".govuk-cookie-banner button[value='false']"));
    }

    internal void Accept()
    {
        _defaultWait.ClickOn(By.CssSelector(".govuk-cookie-banner button[value='true']"));
    }
}