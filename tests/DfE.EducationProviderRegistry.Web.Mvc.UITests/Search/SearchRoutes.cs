using System.Text;

namespace DfE.EducationProviderRegistry.Web.MVC.UITests.Search;

internal static class SearchRoutes
{
    public static Uri Search() => new("search", UriKind.Relative);

    public static Uri SearchResults(
        string? identityTerm = null,
        string? locationTerm = null,
        string? sort = null)
    {
        return new Uri(
            "search/results" +
            $"?SearchKeywords={identityTerm ?? string.Empty}" +
            $"&Address={locationTerm ?? string.Empty}" +
            $"&sort={sort}",
            UriKind.Relative);
    }
}
