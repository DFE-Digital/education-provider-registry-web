using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Establishments;

internal static class EstablishmentDetailsReadModelTestDoubles
{
    internal static EstablishmentDetailsReadModel Failure()
    {
        return new EstablishmentDetailsReadModel
        {
            Establishment = null
        };
    }

    internal static EstablishmentDetailsReadModel Stub()
    {
        return new EstablishmentDetailsReadModel
        {
            Establishment =
                EstablishmentDetailsModelTestDoubles.Stub()
        };
    }
}

internal static class EstablishmentDetailsModelTestDoubles
{
    internal static EstablishmentDetailsModel Stub(
        string? name = null,
        string? urn = null)
    {
        return new EstablishmentDetailsModel
        {
            Urn = new EstablishmentUrnModel(new UniqueReferenceNumber(urn ?? "123456")),
            Name = new EstablishmentNameModel(name ?? "Test Academy"),
            Number = new EstablishmentNumberModel("1000"),
            Status = new EstablishmentStatusModel("Open"),
            Type = new EstablishmentTypeModel("Academy"),
        };
    }
}
