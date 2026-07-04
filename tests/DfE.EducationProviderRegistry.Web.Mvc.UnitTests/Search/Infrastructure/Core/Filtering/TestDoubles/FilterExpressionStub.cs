using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.FilterExpressions;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Search.Infrastructure.Core.Filtering.TestDoubles;

[ExcludeFromCodeCoverage]
public sealed class FilterExpressionStub : ISearchFilterExpression
{
    private readonly string _result;

    public FilterExpressionStub(string result)
    {
        _result = result;
    }

    public string GetFilterExpression(SearchFilterRequest request) => _result;
}