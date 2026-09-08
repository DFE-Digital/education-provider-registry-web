using DfE.Core.Libraries.IntegrationTests.Abstractions;
using DfE.EducationProviderRegistry.Web.SharedTests.ApplicationContainer;
using DfE.WebDriver.Public.Session;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Web.MVC.UITests;

public class UIBaseTest : IntegrationTestsBase, IAsyncLifetime
{
    public UIBaseTest(IServiceProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        WebDriverBuilder = provider.GetRequiredService<IWebDriverSessionBuilder>();

        // TODO configure defaults ctor?
        WebDriverBuilder
            .WithChrome()
            .WithHeadless(true)
            .WithViewport(1920, 1080)
            .WithStartMaximised(true)
            .WithAllowInsecureLocalConnections(true)
            .Build();

        ApplicationEnvironment = provider.GetRequiredService<ApplicationHostedEnvironment>();
    }

    protected IWebDriverSessionBuilder WebDriverBuilder { get; }

    protected ApplicationHostedEnvironment ApplicationEnvironment { get; }

    public async ValueTask InitializeAsync()
    {
        await StartTestAsync(TestContext.Current.CancellationToken);
    }

    protected async override Task StartTestDependenciesAsync(CancellationToken ct = default)
    {
        await ApplicationEnvironment.InitialiseAsync(TestContext.Current.CancellationToken);
    }
}
