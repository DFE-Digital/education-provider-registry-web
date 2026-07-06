using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Mappers;

/// <summary>
/// Maps a collection of <see cref="EstablishmentSearchResult"/> instances into a
/// collection of <see cref="GovUkTable"/> view models suitable for rendering in
/// GOV.UK‑styled search result pages.
/// </summary>
/// <remarks>
/// This mapper is responsible only for transforming search result data into
/// presentational table components. It does not perform filtering, sorting,
/// or enrichment; those responsibilities belong to the search pipeline.
/// 
/// Each <see cref="EstablishmentSearchResult"/> is mapped to a single
/// <see cref="GovUkTable"/> containing a caption and a fixed set of rows
/// describing the establishment.
/// </remarks>
public sealed class EstablishmentSearchResultsToViewModelMapper :
    IMapper<IReadOnlyCollection<EstablishmentSearchResult>, List<GovUkTable>>
{
    /// <summary>
    /// Maps a read‑only collection of establishment search results into a list
    /// of <see cref="GovUkTable"/> view models.
    /// </summary>
    /// <param name="input">
    /// The collection of <see cref="EstablishmentSearchResult"/> instances to map.
    /// </param>
    /// <returns>
    /// A list of <see cref="GovUkTable"/> components, one per establishment.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="input"/> is <c>null</c>.
    /// </exception>
    public List<GovUkTable> Map(IReadOnlyCollection<EstablishmentSearchResult> input)
    {
        ArgumentNullException.ThrowIfNull(input);

        List<GovUkTable> tables = new(input.Count);

        foreach (EstablishmentSearchResult result in input)
        {
            GovUkTable table = MapItem(result);
            tables.Add(table);
        }

        return tables;
    }

    /// <summary>
    /// Maps a single <see cref="EstablishmentSearchResult"/> into a
    /// <see cref="GovUkTable"/> view model.
    /// </summary>
    /// <param name="input">The establishment search result to map.</param>
    /// <returns>A populated <see cref="GovUkTable"/> instance.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="input"/> is <c>null</c>.
    /// </exception>
    private static GovUkTable MapItem(EstablishmentSearchResult input)
    {
        ArgumentNullException.ThrowIfNull(input);

        GovUkTable table = BuildTable(input);
        AddRows(table, input);

        return table;
    }

    /// <summary>
    /// Creates the base <see cref="GovUkTable"/> for an establishment,
    /// including caption and caption link.
    /// </summary>
    /// <param name="input">The establishment search result.</param>
    /// <returns>
    /// A <see cref="GovUkTable"/> with caption metadata populated.
    /// </returns>
    private static GovUkTable BuildTable(EstablishmentSearchResult input)
    {
        GovUkTable table = new()
        {
            Caption = input.Name.Value,
            CaptionLinkUrl = "establishment/" + input.Urn.Value
        };

        return table;
    }

    /// <summary>
    /// Adds the standard set of rows describing the establishment to the table.
    /// </summary>
    /// <param name="table">The table to populate.</param>
    /// <param name="input">The establishment search result.</param>
    /// <remarks>
    /// Rows added:
    /// <list type="bullet">
    /// <item><description>URN</description></item>
    /// <item><description>Type</description></item>
    /// <item><description>Address</description></item>
    /// <item><description>Local authority</description></item>
    /// <item><description>Part of (group)</description></item>
    /// </list>
    /// </remarks>
    private static void AddRows(GovUkTable table, EstablishmentSearchResult input)
    {
        table.AddRow("URN", input.Urn.Value);
        table.AddRow("Type", input.Type.Value);

        table.AddRow(
            "Address",
            input.Address.Street + " " +
            input.Address.County + " " +
            input.Address.Postcode
        );

        table.AddRow(
            "Local authority",
            input.LocalAuthority.Name,
            "/la/" + input.LocalAuthority.Code
        );

        table.AddRow(
            "Part of",
            input.Group.PartOfName,
            "/group/" + input.Group.PartOfCode
        );
    }
}
