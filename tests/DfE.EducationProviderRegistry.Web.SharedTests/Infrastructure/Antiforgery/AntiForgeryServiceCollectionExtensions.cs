using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Web.SharedTests.Infrastructure.Antiforgery;

public static class AntiForgeryServiceCollectionExtensions
{
    public static IServiceCollection AddTestAntiForgeryTokenServices(this IServiceCollection services)
    {
        services.AddControllersWithViews()
                .PartManager
                .ApplicationParts
                .Add(
                    new AssemblyPart(
                        typeof(AntiforgeryTokenController).Assembly));
        return services;
    }
}
