using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Web.Mvc.ApplicationDtos;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;

// Search results mapper
public class SearchResultsPageViewModelMapper :
    IMapper<List<EstablishmentSearchResultDto>, SearchResultsViewModel>
{
    private readonly IMapper<EstablishmentSearchResultDto, GovUkTable> _establishmentSummaryTableMapper;
    public SearchResultsPageViewModelMapper(
        IMapper<EstablishmentSearchResultDto, GovUkTable> establishmentSummaryTableMapper)
    {
        _establishmentSummaryTableMapper = establishmentSummaryTableMapper;
    }
    public SearchResultsViewModel Map(List<EstablishmentSearchResultDto> dto)
    {
        return new SearchResultsViewModel
        {
            EstablishmentResults = dto.Select(_establishmentSummaryTableMapper.Map).ToList()
        };
    }
}

public class SearchResultsEstablishmentSummaryTableMapper :
    IMapper<EstablishmentSearchResultDto, GovUkTable>
{
    public GovUkTable Map(EstablishmentSearchResultDto dto)
    {
        GovUkTableBuilder builder = GovUkTableBuilder
            .Create()
            .WithCaption(dto.Name, $"establishment/{dto.Urn}");

        builder.AddRow(
            new GovUkTableCell { Text = "URN", IsBold = true },
            new GovUkTableCell { Text = dto.Urn }
        );

        builder.AddRow(
            new GovUkTableCell { Text = "Type", IsBold = true },
            new GovUkTableCell { Text = dto.Type }
        );

        builder.AddRow(
            new GovUkTableCell { Text = "Address", IsBold = true },
            new GovUkTableCell { Text = dto.Address }
        );

        builder.AddRow(
            new GovUkTableCell { Text = "Local authority", IsBold = true },
            new GovUkTableCell { Text = dto.LocalAuthorityName, LinkUrl = $"/la/{dto.LocalAuthorityCode}" }
        );

        if (!string.IsNullOrWhiteSpace(dto.PartOfName))
        {
            builder.AddRow(
                new GovUkTableCell { Text = "Part of", IsBold = true },
                new GovUkTableCell { Text = dto.PartOfName, LinkUrl = $"/group/{dto.PartOfCode}" }
            );
        }

        return builder.Build();
    }
}
