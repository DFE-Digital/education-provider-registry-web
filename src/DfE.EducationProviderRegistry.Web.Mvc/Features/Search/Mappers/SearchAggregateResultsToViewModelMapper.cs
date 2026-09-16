using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;

public sealed class SearchAggregateResultsToViewModelMapper
    : IMapper<IReadOnlyCollection<SearchAggregateResult>, List<GovUkTable>>
{
    public List<GovUkTable> Map(IReadOnlyCollection<SearchAggregateResult> input)
    {
        ArgumentNullException.ThrowIfNull(input);

        List<GovUkTable> tables = new(input.Count);

        foreach (SearchAggregateResult result in input)
        {
            tables.Add(MapItem(result));
        }

        return tables;
    }

    private static GovUkTable MapItem(SearchAggregateResult input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return input.ProviderCategory.Category switch
        {
            "Establishment" => MapEstablishment(input),
            "Group" => MapGroup(input),
            _ => throw new ArgumentOutOfRangeException(nameof(input))
        };
    }

    private static GovUkTable MapEstablishment(SearchAggregateResult input)
    {
        TableColumn[] columns =
        [
            new() { Text = "Name", IsRowHeader = true },
            new() { Text = "Value" }
        ];

        GovUkTableBuilder builder = GovUkTableBuilder
            .Create()
            .WithCaption(
                input.Name.Value,
                $"/establishments/{input.UniqueIdentifier.Value}")
            .WithColumns(columns);

        builder.AddRow(
            new TableCell { Text = "URN" },
            new TableCell { Text = input.UniqueIdentifier.Value });

        builder.AddRow(
            new TableCell { Text = "Type" },
            new TableCell { Text = input.Type?.Name });

        builder.AddRow(
            new TableCell { Text = "Address" },
            new TableCell { Text = input.Address.FullAddress });

        builder.AddRow(
            new TableCell { Text = "Local authority" },
            new TableCell { Text = input.LocalAuthority?.Name });

        builder.AddRow(
            new TableCell { Text = "Part of a group" },
            new TableCell
            {
                Text = input.Group?.PartOfName,
                Href = MappingHelpers.CreateLinkUrl(
                    "/groups/",
                    input.Group?.PartOfCode)
            });

        return builder.Build();
    }

    private static GovUkTable MapGroup(SearchAggregateResult input)
    {
        TableColumn[] columns =
        [
            new() { Text = "Name", IsRowHeader = true },
            new() { Text = "Value" }
        ];

        GovUkTableBuilder builder = GovUkTableBuilder
            .Create()
            .WithCaption(
                input.Name.Value,
                $"/groups/{input.UniqueIdentifier.Value}")
            .WithColumns(columns);

        builder.AddRow(
            new TableCell { Text = "Group ID" },
            new TableCell { Text = input.UniqueIdentifier.Value });

        builder.AddRow(
            new TableCell { Text = "Type" },
            new TableCell { Text = input.Type?.Name });

        builder.AddRow(
            new TableCell { Text = "Address" },
            new TableCell { Text = input.Address?.FullAddress });

        builder.AddRow(
           new TableCell { Text = "Academies" },
           new TableCell { Text = $"{input.AcademyCount}" });

        return builder.Build();
    }
}