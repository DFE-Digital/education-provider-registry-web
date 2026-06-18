using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Mappers;

public class SearchResultToTableViewModelMapper : IMapper<EstablishmentSearchResult, GovUkTable>
{
    public GovUkTable Map(EstablishmentSearchResult input)
    {
        GovUkTable govUkTable = new ();

        govUkTable.Rows.Add(new GovUkTableRow
        {
            Cells =
            [
                new GovUkTableCell { Text = "Name", IsBold = true },
                new GovUkTableCell { Text = input.Name }
            ]
        });

        return govUkTable;
    }
}
