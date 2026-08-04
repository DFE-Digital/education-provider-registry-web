using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Services;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Controllers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchFilterSelectionHandlerStub
{
    public static Mock<ISearchFilterSelectionHandler> Mock() => new(MockBehavior.Strict);
    public static Mock<ISearchFilterSelectionHandler> MockFor()
    {
        Mock<ISearchFilterSelectionHandler> mock = Mock();
        mock
            .Setup(handler =>
                handler.Handle(
                    It.IsAny<SearchRequestViewModel>()))
            .Verifiable();
        return mock;
    }
}
