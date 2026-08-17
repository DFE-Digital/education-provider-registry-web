using DfE.EducationProviderRegistry.Web.SharedTests;
using DfE.WebDriver.Public.Session;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace DfE.EducationProviderRegistry.Web.MVC.UITests;

public sealed class SearchTests
{
    private readonly CancellationToken _ct;
    private readonly ApplicationHostedEnvironment _hostedEnvironment;
    private readonly IWebDriverSessionBuilder _webDriverSessionBuilder;

    public SearchTests(
        ApplicationHostedEnvironment hostedEnvironment,
        IWebDriverSessionBuilder webDriverSessionBuilder)
    {

        ArgumentNullException.ThrowIfNull(hostedEnvironment);
        ArgumentNullException.ThrowIfNull(webDriverSessionBuilder);

        _hostedEnvironment = hostedEnvironment;
        _webDriverSessionBuilder = webDriverSessionBuilder;
        _ct = TestContext.Current.CancellationToken;

        _webDriverSessionBuilder
            .WithChrome()
            .WithHeadless(false)
            .WithViewport(1920, 1080)
            .WithStartMaximised(true)
            .WithAllowInsecureLocalConnections(true);
    }

    [Fact]
    public async Task Filter_SearchResults()
    {
        await _hostedEnvironment.InitialiseAsync(_ct);

        using IWebDriver driver =
            await _webDriverSessionBuilder
                .Build()
                .StartDriverAsync(_ct);

        WebDriverWait wait = new(driver, TimeSpan.FromSeconds(15));

        Uri uri = new(_hostedEnvironment.GetApplicationUrl(), "/search");

        await driver.Navigate().GoToUrlAsync(uri);

        // TODO evaluation handlers over context (with decorated WebDriver through WebDriverWait)

        wait.Until((t) =>
        {
            IWebElement element = t.FindElement(By.CssSelector("#SearchKeywords"));
            element.SendKeys("sch");
            return t;
        });

        wait.Until((t) =>
        {
            IWebElement element = t.FindElement(By.CssSelector(".search-panel [type=submit]"));
            element.Click();
            return t;
        });

        // TODO implicitly assumptions on data returned

        wait.Until(t => t.FindElements(By.CssSelector("#establishments caption")).Count == 9);

        wait.Until((t) =>
        {
            IWebElement element = t.FindElement(By.CssSelector(".filter-section"));
            element.Click();
            return driver;
        });
        
        // TODO capture filter label and value here?

        wait.Until((t) =>
        {
            IWebElement element = t.FindElement(By.CssSelector(".filter-section [type=checkbox]"));
            element.Click();
            return t;
        });

        wait.Until((t) =>
        {
            IWebElement element = t.FindElement(By.CssSelector(".filter-panel [type=submit]"));
            element.Click();
            return t;
        });

        // TODO capture network request here with devtools to verify request submitted

        IReadOnlyCollection<IWebElement> elements = wait.Until((t) =>
        {
            return t.FindElements(By.CssSelector(".app-selected-filter"));
        });

        Assert.Single(elements);

        // TODO a filter has been applied
        // Assert.True(elements.Count < 9);
    }
}
