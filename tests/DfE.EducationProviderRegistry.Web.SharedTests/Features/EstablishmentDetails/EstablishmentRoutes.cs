namespace DfE.EducationProviderRegistry.Web.SharedTests.Features.EstablishmentDetails;

public static class EstablishmentRoutes
{
    public static Uri Details(string urn) => new($"establishments/{urn}", UriKind.Relative);
}
