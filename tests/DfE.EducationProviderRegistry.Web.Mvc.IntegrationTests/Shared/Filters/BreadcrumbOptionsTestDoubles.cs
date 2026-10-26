using System;
using System.Collections.Generic;
using System.Text;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Shared.Filters;

public static class BreadcrumbOptionsTestDoubles
{
    public static IReadOnlyDictionary<string, string?> Stub()
    {
        return new Dictionary<string, string?>
        {
            ["Breadcrumbs:Rules:0:PathPattern"] = "/search/results",
            ["Breadcrumbs:Rules:0:Title"] = "Search Results",
            ["Breadcrumbs:Rules:0:JourneyResetMode"] = "DirectNavigationOnly",

            ["Breadcrumbs:Rules:0:BaseJourney:0:Title"] = "Home",
            ["Breadcrumbs:Rules:0:BaseJourney:0:Url"] = "/",

            ["Breadcrumbs:Rules:0:BaseJourney:1:Title"] = "Search",
            ["Breadcrumbs:Rules:0:BaseJourney:1:Url"] = "/search",

            ["Breadcrumbs:Rules:1:PathPattern"] = "/establishments",
            ["Breadcrumbs:Rules:1:TitleModelProperty"] = "Heading",
            ["Breadcrumbs:Rules:1:JourneyResetMode"] = "Always",

            ["Breadcrumbs:Rules:2:PathPattern"] = "/groups",
            ["Breadcrumbs:Rules:2:TitleModelProperty"] = "Heading"
        };
    }
}
