using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

namespace DfE.EducationProviderRegistry.Web.SharedTests.TestDoubles.Establishments;

public static class EstablishmentDetailsReadModelTestDoubles
{
    public static EstablishmentDetailsReadModel WithEstablishment(EstablishmentDetailsModel? response)
    {
        return new()
        {
            Establishment = response
        };
    }
}
