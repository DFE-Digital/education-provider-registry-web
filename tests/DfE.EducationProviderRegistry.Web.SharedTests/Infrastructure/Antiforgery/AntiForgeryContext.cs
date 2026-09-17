namespace DfE.EducationProviderRegistry.Web.SharedTests.Infrastructure.Antiforgery;

public sealed record AntiForgeryContext(
    string FormFieldName, // e.g __RequestVerification
    string RequestToken, // e.g. VALUE
    string CookieHeader); // e.g. .AspNetCore.Antiforgery=