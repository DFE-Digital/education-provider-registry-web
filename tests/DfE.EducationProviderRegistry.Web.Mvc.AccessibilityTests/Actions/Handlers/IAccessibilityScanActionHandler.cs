namespace DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Actions.Handlers;

public interface IAccessibilityScanActionHandler
{
    Task ExecuteAsync(AccessibilityScanContext context);
}
