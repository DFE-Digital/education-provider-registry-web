using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace DfE.EducationProviderRegistry.Web.SharedTests.Infrastructure.WebApplicationFactory.Extensions;

public static class WebApplicationFactoryExtensions
{
    public static WebApplicationFactory<TProgram> UseProduction<TProgram>(this WebApplicationFactory<TProgram> factory) where TProgram : class
    {
        WebApplicationFactory<TProgram> updatedFactory =
            factory.WithWebHostBuilder((builder) =>
                builder.UseEnvironment(Environments.Production));

        return updatedFactory;
    }

    public static WebApplicationFactory<TProgram> UseClientOptions<TProgram>(
        this WebApplicationFactory<TProgram> factory,
        Action<WebApplicationFactoryClientOptions>? configure = null) where TProgram : class
    {
        factory.ClientOptions.BaseAddress = new("https://localhost");
        factory.ClientOptions.AllowAutoRedirect = false;

        configure?.Invoke(factory.ClientOptions);

        return factory;
    }
}
