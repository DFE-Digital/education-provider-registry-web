using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

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
            tables.Add(MapItem(result));
        }

        return tables;
    }

    private static GovUkTable MapItem(EstablishmentSearchResult input)
    {
        ArgumentNullException.ThrowIfNull(input);

        TableColumn[] columns =
        [
            new() { Text = "Name", IsRowHeader = true },
            new() { Text = "Value" }
        ];

        GovUkTableBuilder builder = GovUkTableBuilder
            .Create()
            .WithCaption(
                input.Name.Value,
                "/establishments/" + input.Urn.Value)
            .WithColumns(columns);

        AddRows(builder, input);

        return builder.Build();
    }

    private static void AddRows(
        GovUkTableBuilder builder,
        EstablishmentSearchResult input)
    {
        builder.AddRow(
            new TableCell { Text = "URN" },
            new TableCell { Text = input.Urn.Value });

        builder.AddRow(
            new TableCell { Text = "Type" },
            new TableCell { Text = input.Type?.Value });

        builder.AddRow(
            new TableCell { Text = "Address" },
            new TableCell { Text = MappingHelpers.CombineAddress(input.Address, input.Name.Value) });

        builder.AddRow(
            new TableCell { Text = "Local authority" },
            new TableCell
            {
                Text = input.LocalAuthority?.Name,
                Href = MappingHelpers.CreateLinkUrl("/la/", input.LocalAuthority?.Code)
            });

        builder.AddRow(
            new TableCell { Text = "Part of a group" },
            new TableCell
            {
                Text = input.Group?.PartOfName,
                Href = MappingHelpers.CreateLinkUrl("/groups/", input.Group?.PartOfCode)
            });
    }
}