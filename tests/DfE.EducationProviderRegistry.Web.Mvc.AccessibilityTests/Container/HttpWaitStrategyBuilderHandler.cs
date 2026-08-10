using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;
using Microsoft.Extensions.Options;

namespace DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Container;

internal sealed class HttpWaitStrategyBuilderHandler : IContainerBuilderHandler<ContainerBuilder>
{
    private readonly ContainerOptions _options;

    public HttpWaitStrategyBuilderHandler(
        IOptionsMonitor<ContainerOptions> options)
    {
        _options = options.Get("epr-web");
    }

    public ValueTask<ContainerBuilder> ApplyAsync(
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