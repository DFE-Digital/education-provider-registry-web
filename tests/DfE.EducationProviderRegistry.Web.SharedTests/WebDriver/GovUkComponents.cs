using OpenQA.Selenium;

namespace DfE.EducationProviderRegistry.Web.SharedTests.WebDriver;

public class GovUkComponents
{
    public GovUkComponents(IWebDriver driver)
    {
        Checkbox = new(driver);
        Details = new(driver);
    }

    public virtual GovUkCheckboxComponent Checkbox { get; }
    public virtual GovUkDetailsComponent Details { get; }
}
