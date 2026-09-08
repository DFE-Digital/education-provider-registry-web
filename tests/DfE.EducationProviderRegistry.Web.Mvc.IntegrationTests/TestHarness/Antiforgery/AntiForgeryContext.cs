namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness.Antiforgery;

public sealed record AntiForgeryContext(
    string FormFieldName, // e.g __RequestVerification
    string RequestToken, // e.g. VALUE
    string CookieHeader); // e.g. .AspNetCore.Antiforgery=