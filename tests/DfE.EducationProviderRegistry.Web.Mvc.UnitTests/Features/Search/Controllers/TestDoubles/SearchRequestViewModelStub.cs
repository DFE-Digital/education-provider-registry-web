using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Controllers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchRequestViewModelStub
{
    public static SearchRequestViewModel AcademyWithFacet() =>
        new()
        {
            SearchKeywords = "academy",
            SelectedFacets = new Dictionary<string, List<SelectedFacetValueViewModel>>
            {
                { "establishment_type_id", new List<SelectedFacetValueViewModel> { new SelectedFacetValueViewModel("01", "Academy"), new SelectedFacetValueViewModel("02", "Foundation") } }
            }
        };

    public static SearchRequestViewModel AcademyWithoutFacet() =>
        new()
        {
            SearchKeywords = "academy",
            SelectedFacets = []
        };
}