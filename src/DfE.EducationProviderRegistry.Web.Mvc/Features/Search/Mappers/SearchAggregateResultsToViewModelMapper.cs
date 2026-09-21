using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;

public sealed class SearchAggregateResultsToViewModelMapper
    : IMapper<IReadOnlyCollection<SearchAggregateResult>, List<GovUkTable>>
{
    private readonly IReadOnlyCollection<ISearchAggregateCategoryToModelMapper> _mappers;

    public SearchAggregateResultsToViewModelMapper(IEnumerable<ISearchAggregateCategoryToModelMapper> mappers)
    {
        _mappers = mappers.ToList();
    }

    public List<GovUkTable> Map(
        IReadOnlyCollection<SearchAggregateResult> input)
    {
        return input
            .Select(MapItem)
            .ToList();
    }

    private GovUkTable MapItem(SearchAggregateResult input)
    {
        ISearchAggregateCategoryToModelMapper mapper =
            _mappers.SingleOrDefault(x => x.CanMap(input))
            ?? throw new InvalidOperationException(
                $"No mapper registered for {input.ProviderCategory.Category}");

        return mapper.Map(input);
    }
}



public interface ISearchAggregateCategoryToModelMapper
{
    bool CanMap(SearchAggregateResult input);

    GovUkTable Map(SearchAggregateResult input);
}

public sealed class SearchAggregateCategoryEstablishmentMapper : ISearchAggregateCategoryToModelMapper
{
    public bool CanMap(SearchAggregateResult input) =>
        string.Equals(input.ProviderCategory.Category, "Establishment", StringComparison.OrdinalIgnoreCase);

    public GovUkTable Map(SearchAggregateResult input)
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
}

public sealed class SearchAggregateCategoryGroupMapper : ISearchAggregateCategoryToModelMapper
{
    public bool CanMap(SearchAggregateResult input) =>
        string.Equals(input.ProviderCategory.Category, "Group", StringComparison.OrdinalIgnoreCase);

    public GovUkTable Map(SearchAggregateResult input)
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