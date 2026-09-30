using OpenQA.Selenium;

namespace DfE.EducationProviderRegistry.Web.SharedTests.Features.PageComponents.WebDriver;

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
