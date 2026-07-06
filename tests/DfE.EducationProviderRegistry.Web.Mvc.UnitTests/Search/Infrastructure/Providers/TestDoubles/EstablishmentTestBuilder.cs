using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Search.Infrastructure.Providers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class EstablishmentTestBuilder
{
    private static long _nextId = 1;

    public static Establishment Create(
        string urn,
        string name,
        EstablishmentType type)
    {
        return new Establishment
        {
            EstablishmentId = _nextId++,
            Urn = urn,
            Name = name,
            EstablishmentType = type
        };
    }
}
