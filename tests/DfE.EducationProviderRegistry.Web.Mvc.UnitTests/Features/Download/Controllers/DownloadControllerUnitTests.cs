using DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Download.Controllers;

public sealed class DownloadControllerUnitTests
{
    [Fact]
    public void Index_ReturnsCorrectViewAndModel()
    {
        // arrange

        DownloadDatasetsController sut = new();

        // act
        IActionResult result = sut.Index();

        // assert
        ViewResult view = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", view.ViewName);
    }
}
