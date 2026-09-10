using DfE.EducationProviderRegistry.Web.Mvc.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Web.SharedTests.WebApplicationFactory.Extensions;

public static class WebApplicationFactoryFeatureExtensions
{
    public static IServiceCollection WithClarity(this IServiceCollection services)
    {
        // Valid clarity
        services.PostConfigure<ClaritySettings>((opts) =>
        {
            opts.Enabled = true;
            opts.ProjectId = "STUB-PROJECTID";
        });
        return services;
    }

    public static IServiceCollection WithDisabledClarity(this IServiceCollection services)
    {
        // Valid clarity
        services.PostConfigure<ClaritySettings>((opts) =>
        {
            opts.Enabled = false;
        });

        return services;
    }

    public static IServiceCollection WithGoogleTagManager(this IServiceCollection services)
    {
        // Valid GTM
        services.PostConfigure<GoogleAnalyticsSettings>((opts) =>
        {
            opts.ContainerId = "STUB-GTM-CONTAINERID";
        });
        return services;
    }

    public static IServiceCollection WithDisabledGoogleTagManager(this IServiceCollection services)
    {
        // Valid GTM
        services.PostConfigure<GoogleAnalyticsSettings>((opts) =>
        {
            opts.ContainerId = string.Empty;
        });
        return services;
    }
}
