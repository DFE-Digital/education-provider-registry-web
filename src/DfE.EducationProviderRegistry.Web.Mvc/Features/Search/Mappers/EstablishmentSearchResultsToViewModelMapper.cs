using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;

public sealed class EstablishmentSearchResultsToViewModelMapper :
    IMapper<IReadOnlyCollection<EstablishmentSearchResult>, List<GovUkTable>>
{
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

    private static GovUkTable MapItem(EstablishmentSearchResult input)
    {
        ArgumentNullException.ThrowIfNull(input);

        GovUkTable table = BuildTable(input);
        AddRows(table, input);

        return table;
    }

    private static GovUkTable BuildTable(EstablishmentSearchResult input)
    {
        GovUkTable table = new()
        {
            Caption = input.Name.Value,
            CaptionLinkUrl = "establishment/" + input.Urn.Value
        };

        return table;
    }

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
