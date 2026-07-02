using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Search.Controllers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class FilterRequestStub
{
    public static ReadOnlyCollection<FilterRequest> EstablishmentTypeFacet() =>
        new([
            new("establishment_type_id", ["01", "02"])
        ]);
}
