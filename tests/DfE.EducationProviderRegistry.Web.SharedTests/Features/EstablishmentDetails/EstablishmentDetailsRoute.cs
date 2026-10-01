namespace DfE.EducationProviderRegistry.Web.SharedTests.Features.EstablishmentDetails;

public static class EstablishmentDetailsRoute
{
    public static Uri ForUrn(string urn) => new($"establishments/{urn}", UriKind.Relative);
}
