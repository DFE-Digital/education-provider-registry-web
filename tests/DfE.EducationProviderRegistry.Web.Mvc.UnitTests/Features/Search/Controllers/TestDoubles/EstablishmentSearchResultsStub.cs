using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Controllers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class EstablishmentSearchResultsStub
{
    public static EstablishmentSearchResults Empty() => new([]);
}