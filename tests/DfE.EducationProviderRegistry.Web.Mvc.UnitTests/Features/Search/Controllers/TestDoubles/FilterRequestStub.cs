using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Controllers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class FilterRequestStub
{
    public static ReadOnlyCollection<FilterRequest> EstablishmentTypeFacet() =>
        new([
            new("establishment_type_id", ["01", "02"])
        ]);
}
