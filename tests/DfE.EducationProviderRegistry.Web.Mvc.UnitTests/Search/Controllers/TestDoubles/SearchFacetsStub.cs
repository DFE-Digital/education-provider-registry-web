using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Search.Controllers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchFacetsStub
{
    public static SearchFacets Empty() => new([]);
}
