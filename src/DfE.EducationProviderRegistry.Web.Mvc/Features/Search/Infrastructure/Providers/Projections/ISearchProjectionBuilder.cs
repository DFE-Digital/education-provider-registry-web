using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Providers.Projections;

/// <summary>
/// Defines a component responsible for constructing an <see cref="IQueryable{TProjection}"/>
/// that represents the base query used by the search pipeline.
/// </summary>
/// <typeparam name="TProjection">
/// The projection type produced by the builder. This may be an EF Core entity,
/// a lightweight DTO, or any type that can be materialised from the underlying
/// <see cref="DbContext"/>.
/// </typeparam>
/// <remarks>
/// Implementations of this interface encapsulate the logic required to build
/// the initial query shape used by search orchestration. This may include
/// eager‑loading navigation properties, applying default filters, or projecting
/// entities into search‑specific models. The returned <see cref="IQueryable{T}"/>
/// must be EF‑translatable so that downstream components can safely append
/// additional filters, ordering, and pagination.
/// </remarks>
public interface ISearchProjectionBuilder<TProjection>
{
    /// <summary>
    /// Builds the base <see cref="IQueryable{TProjection}"/> used by the search pipeline.
    /// </summary>
    /// <param name="db">
    /// The <see cref="DbContext"/> instance from which the query will be constructed.
    /// Implementations must not execute the query; they should only compose and return
    /// the <see cref="IQueryable{TProjection}"/>.
    /// </param>
    /// <returns>
    /// An EF‑translatable <see cref="IQueryable{TProjection}"/> representing the initial
    /// query shape for search operations.
    /// </returns>
    IQueryable<TProjection> Build(DbContext db);
}
