using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options.Container;
using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry.BuilderHandler;
using DotNet.Testcontainers.Builders;
using Microsoft.Extensions.Options;

namespace DfE.EducationProviderRegistry.Web.SharedTests.ApplicationContainer;

internal sealed class HttpWaitStrategyBuilderHandler : IConfigureContainerBuilderHandler<ContainerBuilder>
{
    private readonly ContainerOptions _options;

    public HttpWaitStrategyBuilderHandler(
        IOptionsMonitor<ContainerOptions> options)
    {
        _options = options.Get("epr-web");
    }

    public ValueTask<ContainerBuilder> HandleAsync(
        ContainerBuilder builder,
        CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(
            builder.WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilHttpRequestIsSucceeded(
                        request =>
                            request.ForPort(
                                (ushort)_options
                                    .PortMappings!
                                    .First()
                                    .ContainerPort))));
    }
}