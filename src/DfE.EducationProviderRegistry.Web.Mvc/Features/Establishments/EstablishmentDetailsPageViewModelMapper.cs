using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Web.Mvc.ApplicationDtos;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using DfE.EducationProviderRegistry.Web.Mvc.ViewModels.Pages;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments;

// Establishment details page mappers
public class EstablishmentDetailsPageViewModelMapper :
    IMapper<EstablishmentDto, EstablishmentDetailsPageViewModel>
{
    private readonly IMapper<EstablishmentBasicDetailsDto, GovUkTable> _basicMapper;
    private readonly IMapper<List<EstablishmentGovernorDto>, GovUkTable> _governorMapper;
    private readonly IMapper<List<EstablishmentHistoryDto>, GovUkTable> _historyMapper;

    public EstablishmentDetailsPageViewModelMapper(
        IMapper<EstablishmentBasicDetailsDto, GovUkTable> basicMapper,
        IMapper<List<EstablishmentGovernorDto>, GovUkTable> governorMapper,
        IMapper<List<EstablishmentHistoryDto>, GovUkTable> historyMapper)
    {
        _basicMapper = basicMapper;
        _governorMapper = governorMapper;
        _historyMapper = historyMapper;
    }

    public EstablishmentDetailsPageViewModel Map(EstablishmentDto dto)
    {
        return new EstablishmentDetailsPageViewModel
        {
            Heading = dto.BasicDetails.Name,
            BasicDetails = _basicMapper.Map(dto.BasicDetails),
            Governors = _governorMapper.Map(dto.Governors),
            History = _historyMapper.Map(dto.History)
        };
    }
}

public class EstablishmentDetailsBasicDetailsTableMapper :
    IMapper<EstablishmentBasicDetailsDto, GovUkTable>
{
    public GovUkTable Map(EstablishmentBasicDetailsDto dto)
    {
        GovUkTableBuilder builder = GovUkTableBuilder
            .Create();

        builder.AddRow(new GovUkTableCell { Text = "URN", IsBold = true },
                       new GovUkTableCell { Text = dto.Urn });

        builder.AddRow(new GovUkTableCell { Text = "UKPRN", IsBold = true },
                       new GovUkTableCell { Text = dto.Ukprn });

        builder.AddRow(new GovUkTableCell { Text = "DfE number", IsBold = true },
                       new GovUkTableCell { Text = dto.DfeNumber });

        builder.AddRow(new GovUkTableCell { Text = "Status", IsBold = true },
                       new GovUkTableCell { Text = dto.Status });

        builder.AddRow(new GovUkTableCell { Text = "Address", IsBold = true },
                       new GovUkTableCell { Text = dto.Address });

        builder.AddRow(new GovUkTableCell { Text = "Local authority", IsBold = true },
                       new GovUkTableCell { Text = dto.LocalAuthority, LinkUrl = $"/la/{dto.LocalAuthorityCode}" });

        if (!string.IsNullOrWhiteSpace(dto.PartOfName))
        {
            builder.AddRow(
                new GovUkTableCell { Text = "Part of", IsBold = true },
                new GovUkTableCell { Text = dto.PartOfName, LinkUrl = $"/group/{dto.PartOfCode}" }
            );
        }

        builder.AddRow(new GovUkTableCell { Text = "Type", IsBold = true },
                       new GovUkTableCell { Text = dto.Type });

        builder.AddRow(new GovUkTableCell { Text = "Phase of education", IsBold = true },
                       new GovUkTableCell { Text = dto.PhaseOfEducation });

        builder.AddRow(new GovUkTableCell { Text = "Age range", IsBold = true },
                       new GovUkTableCell { Text = dto.AgeRange });

        builder.AddRow(new GovUkTableCell { Text = "Gender", IsBold = true },
                       new GovUkTableCell { Text = dto.Gender });

        builder.AddRow(new GovUkTableCell { Text = "Religious character", IsBold = true },
                       new GovUkTableCell { Text = dto.ReligiousCharacter });

        builder.AddRow(new GovUkTableCell { Text = "Ofsted rating", IsBold = true },
                       new GovUkTableCell { Text = dto.OfstedLastReported, LinkUrl = dto.OfstedLastReportedUrl });

        return builder.Build();
    }
}

public class EstablishmentDetailsGovernorsTableMapper :
IMapper<List<EstablishmentGovernorDto>, GovUkTable>
{
    public GovUkTable Map(List<EstablishmentGovernorDto> dto)
    {
        GovUkTableBuilder builder = GovUkTableBuilder
            .Create()
            .WithCaption("Governors")
            .WithHeaders("Name", "Governor ID", "Start date");

        foreach (EstablishmentGovernorDto g in dto)
        {
            builder.AddRow(
                new GovUkTableCell { Text = g.Name },
                new GovUkTableCell { Text = g.GovernorId },
                new GovUkTableCell { Text = g.StartDate }
            );
        }

        return builder.Build();
    }
}

public class EstablishmentDetailsHistoryTableMapper :
    IMapper<List<EstablishmentHistoryDto>, GovUkTable>
{
    public GovUkTable Map(List<EstablishmentHistoryDto> dto)
    {
        GovUkTableBuilder builder = GovUkTableBuilder
            .Create()
            .WithCaption("History");

        foreach (var h in dto)
        {
            builder.AddRow(
                new GovUkTableCell { Text = h.Name, LinkUrl = $"/establishment/{h.Urn}" },
                new GovUkTableCell { Text = h.Status },
                new GovUkTableCell { Text = $"URN {h.Urn}" }
            );
        }

        return builder.Build();
    }
}