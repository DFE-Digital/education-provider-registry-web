namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments;

public static class EstablishmentRoutes
{
    public static Uri Details(string urn) => new($"/establishments/{urn}", UriKind.Relative);
}
