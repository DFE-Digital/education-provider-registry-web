using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Filtering.LogicalOperators;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Filtering.LogicalOperators.Factories;

/// <summary>
/// Provides a factory implementation over which to derive Azure AI OData logical
/// operator expressions. This factory leverages dependency injection which necessitates
/// setup of a dictionary of <see cref="ILogicalOperator"/> delegates
/// responsible for handling the creation of concrete <see cref="ILogicalOperator"/>
/// instances. Typical container setup/registrations are as follows,
/// <code>
/// services.TryAddSingleton&lt;ILogicalOperatorFactory&gt;(provider =>
/// {
///    var scopedLogicalOperatorExpressionProvider = provider.CreateScope();
///    var logicalOperators =
///      new Dictionary&lt;string, Func&lt;ILogicalOperator&gt;&gt;()
///      {
///          ["AndLogicalOperator"] = () =>
///              scopedLogicalOperatorExpressionProvider
///                .ServiceProvider.GetRequiredService&lt;AndLogicalOperator&gt;(),
///          ["OrLogicalOperator"] = () =>
///              scopedLogicalOperatorExpressionProvider
///                .ServiceProvider.GetRequiredService&lt;OrLogicalOperator&gt;()
///      };
///
///    return new LogicalOperatorFactory(logicalOperators);
///});
/// </code>
/// </summary>
public sealed class LogicalOperatorFactory : ILogicalOperatorFactory
{
    private readonly Dictionary<string, Func<ILogicalOperator>> _logicalOperatorFactory;

    /// <summary>
    /// The <see cref="LogicalOperatorFactory"/> uses a dictionary of delegates injected via the IOC
    /// container which allows management of object lifetime and scope to be managed via this composition root.
    /// </summary>
    /// <param name="logicalOperatorFactory">
    /// Provides a dictionary of delegates used to derive the requested type.
    /// </param>
    public LogicalOperatorFactory(Dictionary<string, Func<ILogicalOperator>> logicalOperatorFactory)
    {
        _logicalOperatorFactory = logicalOperatorFactory;
    }

    /// <summary>
    /// Allows creation of an <see cref="ILogicalOperator"/> instance based on the type name requested.
    /// </summary>
    /// <param name="logicalOperatorName">
    /// The name of the concrete implementation type <see cref="ILogicalOperator"/> requested.
    /// </param>
    /// <returns>
    /// The configured instance of the <see cref="ILogicalOperator"/> type.
    /// </returns>
    /// <exception cref="ArgumentException ">
    /// Exception thrown if an invalid filter name string is provisioned.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Exception thrown if a request is made to derive an unknown
    /// <see cref="ILogicalOperator"/> instance from the underlying IOC managed dictionary.
    /// </exception>
    public ILogicalOperator CreateLogicalOperator(string logicalOperatorName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(logicalOperatorName);

        return (!_logicalOperatorFactory
            .TryGetValue(logicalOperatorName, out var logicalOperator) || logicalOperator is null) ?
                throw new ArgumentOutOfRangeException(
                    $"Logical operator of type {logicalOperatorName} is not registered.") : logicalOperator();
    }
}
