using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Controllers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchResultsViewModelStub
{
    public static SearchResultsViewModel Empty() =>
        new()
        {
            SearchAggregateResults = [],
            Facets = null
        };

    public static SearchResultsViewModel WithEstablishmentResults(
        List<GovUkTable> results) =>
        new()
        {
            SearchAggregateResults = results,
            Facets = null
        };

    public static SearchResultsViewModel WithPrimarySearchTerms(
        string searchTerms,
        List<GovUkTable> results) =>
        new()
        {
            SearchAggregateResults = results,
            Facets = null,
            PrimarySearchTerms = searchTerms
        };
}

