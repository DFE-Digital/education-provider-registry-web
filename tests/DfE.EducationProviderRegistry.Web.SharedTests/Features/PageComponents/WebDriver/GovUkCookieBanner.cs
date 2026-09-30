using DfE.WebDriver.WebDriver.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace DfE.EducationProviderRegistry.Web.SharedTests.Features.PageComponents.WebDriver;

public sealed class GovUkCookieBanner
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

    public void Accept()
    {
        _defaultWait.Click(By.CssSelector(".govuk-cookie-banner button[value='true']"));
    }
}
