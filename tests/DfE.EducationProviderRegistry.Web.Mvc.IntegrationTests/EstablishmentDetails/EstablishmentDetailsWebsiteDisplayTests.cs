using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.IntegrationTests.Abstractions;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.EstablishmentDetails;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishmentById;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.EstablishmentDetails;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.EstablishmentDetails;

public sealed class EstablishmentDetailsWebsiteDisplayTests : IntegrationTestsBase
{
    [Fact]
    public async Task Website_With_No_Protocol()
    {
        const string noProtocolWebsite = "test.example";

        EstablishmentDetailsModel establishment =
            EstablishmentDetailsBuilder.Create()
                .WithUrn(100000)
                .WithWebsite(noProtocolWebsite)
                .Build();

        // 

        UseCaseResponse<EstablishmentDetailsModel> stubResponse =
            UseCaseResponse<EstablishmentDetailsReadModel>.Success(establishment);

        UseCaseStub<GetEstablishmentByIdRequest, EstablishmentDetailsReadModel> stubUseCase = new(stubResponse);

        using WebApplicationFactory<Program> factory = WebApplicationFactoryProvider.CreateFactory(stubUseCase);

        using HttpClient client = factory.CreateClient();

        // Act
        using HttpResponseMessage httpResponseMessage =
            await client.GetAsync(
                EstablishmentDetailsRoute.ForUrn("100000"),
                TestContext.Current.CancellationToken);

        // Assert

        using IHtmlDocument document = await httpResponseMessage.AssertSuccessfulHtmlResponseAsync();
        EstablishmentDetailsPage page = new(document);

        Assert.Equal("https://test.example", page.GetEstablishmentDetailsTable().Rows["Website"]);
    }
}
