using DfE.EducationProviderRegistry.Web.Mvc.Features.Download.Datasets.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Download.Views;

public sealed class DownloadDatasetsViewLocationExpanderTests
{
    [Fact]
    public void ExpandViewLocations_PrependsCustomFeatureLocations_BasedOnControllerRoute()
    {
        // arrange
        ActionDescriptor actionDescriptor =
            new()
            {
                RouteValues = new Dictionary<string, string?>
                {
                    { "controller", "Datasets" }
                }
            };

        ViewLocationExpanderContext expanderContext = new(
            actionContext: new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                actionDescriptor),
            viewName: "Index",
            controllerName: "Datasets",
            areaName: null,
            pageName: null,
            isMainPage: true
        );

        string[] existingLocations =
        [
            "/Views/Shared/{0}.cshtml",
            "/Views/Home/{0}.cshtml"
        ];

        // act
        List<string> result =
            [
                .. new DownloadDatasetsViewLocationExpander()
                    .ExpandViewLocations(expanderContext, existingLocations)
            ];

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Count);
        Assert.Equal("/Features/Download/Datasets/Views/Datasets/{0}.cshtml", result[0]);
        Assert.Equal("/Features/Download/Datasets/Views/Shared/{0}.cshtml", result[1]);
        Assert.Equal("/Views/Shared/{0}.cshtml", result[2]);
        Assert.Equal("/Views/Home/{0}.cshtml", result[3]);
    }

    [Fact]
    public void PopulateValues_DoesNotThrow()
    {
        // arrange
        ViewLocationExpanderContext expanderContext =
            new(
                actionContext: new ActionContext(),
                viewName: "Index",
                controllerName: null,
                areaName: null,
                pageName: null,
                isMainPage: true
            );

        // act/assert
        Exception? exception = Record.Exception(() =>
            new DownloadDatasetsViewLocationExpander()
                .PopulateValues(expanderContext));

        Assert.Null(exception);
    }
}
