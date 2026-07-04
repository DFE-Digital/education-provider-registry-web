using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Search.Infrastructure.Core.Filtering.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchFilterRequestStub
{
    public static SearchFilterRequest Create(string key, params object[] values)
        => new(key, values);
}

