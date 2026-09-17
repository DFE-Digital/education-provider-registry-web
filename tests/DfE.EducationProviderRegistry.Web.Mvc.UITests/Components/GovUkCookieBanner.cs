using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace DfE.EducationProviderRegistry.Web.MVC.UITests.Components;

internal sealed class GovUkCookieBanner
{
    private readonly WebDriverWait _defaultWait;

    public GovUkCookieBanner(IWebDriver driver)
    {
        _defaultWait = new(driver, TimeSpan.FromSeconds(15));
    }

    public void Reject()
    {
        _defaultWait.Click(By.CssSelector(".govuk-cookie-banner button[value='false']"));
    }

    internal void Accept()
    {
        _defaultWait.Click(By.CssSelector(".govuk-cookie-banner button[value='true']"));
    }
}