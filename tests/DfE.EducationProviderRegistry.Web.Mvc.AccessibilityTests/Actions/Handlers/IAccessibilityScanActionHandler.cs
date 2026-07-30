namespace DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Actions;

public interface IAccessibilityScanActionHandler
{
    Task ExecuteAsync(AccessibilityScanContext context);
}
