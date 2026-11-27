using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments.Mappers;

public class EstablishmentDetailsPageViewModelMapper : IMapper<EstablishmentDetailsModel, EstablishmentDetailsPageViewModel>
{
    private readonly IMapper<EstablishmentDetailsModel, GovUkTable> _basicToTableMapper;
    private readonly TimeProvider _timeProvider;

    public EstablishmentDetailsPageViewModelMapper(
        IMapper<EstablishmentDetailsModel, GovUkTable> basicMapper,
        TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(basicMapper);
        ArgumentNullException.ThrowIfNull(timeProvider);

        _basicToTableMapper = basicMapper;
        _timeProvider = timeProvider;
    }

    public EstablishmentDetailsPageViewModel Map(
        EstablishmentDetailsModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DateOnly today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().DateTime);
        DateOnly twelveMonthsAgo = today.AddYears(-1);

        IReadOnlyCollection<GovernorModel> activeGovernors = model.Governors
            .Where(g => g.StartDate <= today && (g.EndDate is null || g.EndDate >= today))
            .OrderBy(g => g.RoleOrder)
            .ToArray();

        IReadOnlyCollection<GovernorModel> historicGovernors = model.Governors
            .Where(g => g.EndDate >= twelveMonthsAgo && g.EndDate < today)
            .OrderBy(g => g.RoleOrder)
            .ToArray();

        return new EstablishmentDetailsPageViewModel
        {
            Heading = model.Name?.Value,
            BasicDetailsTable = _basicToTableMapper.Map(model),
            GovernorsTable = CreateGovenorsTable(activeGovernors, "Board of governors"),
            HistoricGovernorsTable = CreateGovenorsTable(historicGovernors, "Governors who have left within the last 12 months")
        };
    }


    private static GovUkTable CreateGovenorsTable(IEnumerable<GovernorModel> govenors, string caption)
    {
        GovUkTableBuilder builder = GovUkTableBuilder
            .Create()
            .WithCaption(caption)
            .WithColumns(
                new TableColumn { Text = "Name and role" },
                new TableColumn { Text = "Governor ID" },
                new TableColumn { Text = "Appointed by" },
                new TableColumn { Text = "Term" });

        foreach (GovernorModel g in govenors)
        {
            builder.AddRow(
                new TableCell { Text = $"{g.Title} {g.Firstname} {g.Lastname} ({g.Role})" },
                new TableCell { Text = g.Identifier.Value },
                new TableCell { Text = g.AppointedBy },
                new TableCell { Text = FormatTerm(g.StartDate, g.EndDate) });
        }

        return builder.Build();
    }

    private static string FormatTerm(DateOnly startDate, DateOnly? endDate)
    {
        string formattedStartDate = startDate.ToString("d MMMM yyyy");

        return endDate.HasValue
            ? $"{formattedStartDate} to {endDate.Value.ToString("d MMMM yyyy")}"
            : $"From {formattedStartDate}";
    }
}
