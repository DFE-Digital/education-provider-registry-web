using DfE.EducationProviderRegistry.Web.Mvc.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Search.Controllers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchResultsViewModelStub
{
    public static SearchResultsViewModel Empty() =>
        new()
        {
            EstablishmentResults = [],
            Facets = null
        };

    public static SearchResultsViewModel WithEstablishmentResults(
        List<GovUkTable> results) =>
        new()
        {
            EstablishmentResults = results,
            Facets = null
        };

    public static SearchResultsViewModel WithPrimarySearchTerms(
        string searchTerms,
        List<GovUkTable> results) =>
        new()
        {
            EstablishmentResults = results,
            Facets = null,
            PrimarySearchTerms = searchTerms
        };
}

