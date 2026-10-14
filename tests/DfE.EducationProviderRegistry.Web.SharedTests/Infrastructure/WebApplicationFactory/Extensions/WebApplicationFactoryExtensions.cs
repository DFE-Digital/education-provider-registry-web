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
}
