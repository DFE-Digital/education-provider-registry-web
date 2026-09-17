using OpenQA.Selenium;

namespace DfE.EducationProviderRegistry.Web.MVC.UITests.Components;

internal class GovUkComponents
{
    public GovUkComponents(IWebDriver driver)
    {
        Checkbox = new(driver);
        Details = new(driver);
    }

    public virtual GovUkCheckboxComponent Checkbox { get; }
    public virtual GovUkDetailsComponent Details { get; }
}
