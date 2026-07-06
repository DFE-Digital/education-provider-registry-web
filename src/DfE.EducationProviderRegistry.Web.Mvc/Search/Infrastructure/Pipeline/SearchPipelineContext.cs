namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Pipeline;

/// <summary>
/// Represents the shared state passed between search pipeline steps.
/// Each step may add, read, or transform values stored in the context,
/// enabling a composable and deterministic search pipeline.
/// </summary>
/// <remarks>
/// Values are stored and retrieved strictly by their concrete <see cref="Type"/>.
/// This means the type used in <see cref="Set{TContextState}(TContextState)"/>
/// must match exactly the type used in <see cref="Get{TContextState}"/> or
/// <see cref="TryGet{TContextState}(out TContextState)"/>.
/// 
/// If a requested type has not been stored, <see cref="Get{TContextState}"/>
/// throws an <see cref="InvalidOperationException"/>.
/// </remarks>
public sealed class SearchPipelineContext
{
    /// <summary>
    /// Internal dictionary storing pipeline state values keyed by their concrete type.
    /// </summary>
    private readonly Dictionary<Type, object> _items = [];

    /// <summary>
    /// Stores a value in the pipeline context under its concrete type.
    /// </summary>
    /// <typeparam name="TContextState">
    /// The type used as the key for storing and retrieving the value.
    /// </typeparam>
    /// <param name="value">
    /// The value to store. Must be retrieved later using the same type.
    /// </param>
    /// <remarks>
    /// If a value of the same type already exists, it is overwritten.
    /// </remarks>
    public void Set<TContextState>(TContextState value)
    {
        _items[typeof(TContextState)] = value!;
    }

    /// <summary>
    /// Retrieves a value previously stored in the context using <see cref="Set{TContextState}(TContextState)"/>.
    /// </summary>
    /// <typeparam name="TContextState">
    /// The type used when the value was stored.
    /// </typeparam>
    /// <returns>
    /// The stored value cast to <typeparamref name="TContextState"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no value has been stored for the requested type.
    /// </exception>
    public TContextState Get<TContextState>()
    {
        if (_items.TryGetValue(typeof(TContextState), out object? value))
        {
            return (TContextState)value;
        }

        throw new InvalidOperationException(
            "PipelineContext does not contain a value of type " + typeof(TContextState).Name);
    }

    /// <summary>
    /// Attempts to retrieve a value previously stored in the context.
    /// </summary>
    /// <typeparam name="TContextState">
    /// The type used when the value was stored.
    /// </typeparam>
    /// <param name="value">
    /// When this method returns, contains the stored value if found; otherwise the default value.
    /// </param>
    /// <returns>
    /// <c>true</c> if a value was found for the requested type; otherwise <c>false</c>.
    /// </returns>
    public bool TryGet<TContextState>(out TContextState value)
    {
        if (_items.TryGetValue(typeof(TContextState), out object? obj))
        {
            value = (TContextState)obj;
            return true;
        }

        value = default!;
        return false;
    }
}
