using DfE.EducationProviderRegistry.Web.Mvc.Search.ViewModels;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Search.Controllers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchRequestViewModelStub
{
    public static SearchRequestViewModel AcademyWithFacet() =>
        new()
        {
            SearchKeywords = "academy",
            SelectedFacets = new Dictionary<string, List<string>>
            {
                { "establishment_type_id", new List<string> { "01", "02" } }
            }
        };

    public static SearchRequestViewModel AcademyWithoutFacet() =>
        new()
        {
            SearchKeywords = "academy",
            SelectedFacets = null
        };
}