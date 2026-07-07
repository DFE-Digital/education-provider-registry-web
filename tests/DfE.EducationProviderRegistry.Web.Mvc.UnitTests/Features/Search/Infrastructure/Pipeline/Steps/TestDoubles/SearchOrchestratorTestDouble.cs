using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Providers.SearchOrchestrators;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Infrastructure.Pipeline.Steps.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchOrchestratorTestDouble
{
    public static Mock<ISearchOrchestrator<Establishment>> Mock() =>
        new(MockBehavior.Strict);
}
