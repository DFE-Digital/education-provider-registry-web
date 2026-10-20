using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Options;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Tracking;
using Moq;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs.TestDoubles;

[ExcludeFromCodeCoverage]
public static class BreadcrumbTrackerTestDouble
{
    public static Mock<IBreadcrumbTracker> Mock()
    {
        return new Mock<IBreadcrumbTracker>();
    }
}
