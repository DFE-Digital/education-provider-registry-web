namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering.FilterExpressions.Factories;

/// <summary>
/// Provides a factory implementation over which to derive Azure AI OData filter
/// expressions. This factory leverages dependency injection which necessitates
/// setup of a dictionary of <see cref="ISearchFilterExpression"/> delegates
/// responsible for handling the creation of concrete <see cref="ISearchFilterExpression"/>
/// instances. Typical container setup/registrations are as follows,
/// <code>
/// services.TryAddSingleton&lt;ISearchFilterExpressionFactory&gt;(provider =>
/// {
///    var scopedSearchFilterExpressionProvider = provider.CreateScope();
///    var searchFilterExpressions =
///    new Dictionary&lt;string, Func&lt;ISearchFilterExpression&gt;&gt;()
///    {
///        ["SearchInFilterExpression"] = () =>
///            scopedSearchFilterExpressionProvider
///                .ServiceProvider.GetRequiredService&lt;SearchInFilterExpression&gt;(),
///        ["LessThanOrEqualToExpression"] = () =>
///            scopedSearchFilterExpressionProvider
///                .ServiceProvider.GetRequiredService&lt;LessThanOrEqualToExpression&gt;(),
///        ["SearchGeoLocationFilterExpression"] = () =>
///            scopedSearchFilterExpressionProvider.
///                ServiceProvider.GetRequiredService&lt;SearchGeoLocationFilterExpression&gt;()
///    };
///    return new SearchFilterExpressionFactory(searchFilterExpressions);
///});
/// </code>
/// </summary>
public sealed class SearchFilterExpressionFactory : ISearchFilterExpressionFactory
{
    private readonly Dictionary<string, Func<ISearchFilterExpression>> _filterExpressionfactory;

    /// <summary>
    /// The <see cref="SearchFilterExpressionFactory"/> uses a dictionary of delegates injected via the IOC
    /// container which allows management of object lifetime and scope to be managed via this composition root.
    /// </summary>
    /// <param name="filterExpressionFactory">
    /// Provides a dictionary of delegates used to derive the requested type.
    /// </param>
    public SearchFilterExpressionFactory(Dictionary<string, Func<ISearchFilterExpression>> filterExpressionFactory)
    {
        _filterExpressionfactory = filterExpressionFactory;
    }

    /// <summary>
    /// Allows creation of an <see cref="ISearchFilterExpression"/> instance based on the generic type specified.
    /// </summary>
    /// <typeparam name="TSearchFilterExpression">
    /// The concrete type of <see cref="ISearchFilterExpression"/> requested.
    /// </typeparam>
    /// <returns>
    /// The configured instance of the <see cref="ISearchFilterExpression"/> type.
    /// </returns>
    public ISearchFilterExpression CreateFilter<TSearchFilterExpression>()
        where TSearchFilterExpression : ISearchFilterExpression => CreateFilter(typeof(TSearchFilterExpression));

    /// <summary>
    /// Allows creation of an <see cref="ISearchFilterExpression"/> instance based on the type requested.
    /// </summary>
    /// <param name="filterType">
    /// The concrete implementation type of <see cref="ISearchFilterExpression"/> requested.
    /// </param>
    /// <returns>
    /// The configured instance of the <see cref="ISearchFilterExpression"/> type.
    /// </returns>
    public ISearchFilterExpression CreateFilter(Type filterType) => CreateFilter(filterName: filterType.Name);

    /// <summary>
    /// Allows creation of an <see cref="ISearchFilterExpression"/> instance based on the type name requested.
    /// </summary>
    /// <param name="filterName">
    /// The name of the concrete implementation type <see cref="ISearchFilterExpression"/> requested.
    /// </param>
    /// <returns>
    /// The configured instance of the <see cref="ISearchFilterExpression"/> type.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Exception thrown if an invalid filter name string is provisioned.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Exception thrown if a request is made to derive an unknown
    /// <see cref="ISearchFilterExpression"/> instance from the underlying IOC managed dictionary.
    /// </exception>
    public ISearchFilterExpression CreateFilter(string filterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filterName);

        return (!_filterExpressionfactory
            .TryGetValue(filterName, out var searchExpressionFilter) || searchExpressionFilter is null) ?
                throw new ArgumentOutOfRangeException(
                    $"Search expression filter of type {filterName} is not registered.") : searchExpressionFilter();
    }
}