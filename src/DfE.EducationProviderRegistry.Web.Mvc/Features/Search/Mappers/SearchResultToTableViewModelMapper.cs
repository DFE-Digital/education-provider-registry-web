using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;

public class SearchResultToTableViewModelMapper
    : IMapper<EstablishmentSearchResult, GovUkTable>
{
    public GovUkTable Map(EstablishmentSearchResult input)
    {
        GovUkTable table = new()
        {
            Caption = input.Name.Value,
            CaptionLinkUrl = $"establishment/{input.Urn.Value}"
        };

        table.AddRow("URN", input.Urn.Value);
        table.AddRow("Type", input.Type.Value);

        table.AddRow(
            "Address",
            $"{input.Address.Street} {input.Address.County} {input.Address.Postcode}"
        );

        table.AddRow(
            "Local authority",
            input.LocalAuthority.Name,
            $"/la/{input.LocalAuthority.Code}"
        );

        table.AddRow(
            "Part of",
            input.Group.PartOfName,
            $"/group/{input.Group.PartOfCode}"
        );

        return table;
    }
}
