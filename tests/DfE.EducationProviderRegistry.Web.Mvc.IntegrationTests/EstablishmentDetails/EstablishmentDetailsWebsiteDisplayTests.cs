using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.IntegrationTests.Abstractions;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.EstablishmentDetails;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishmentById;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.EstablishmentDetails;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.PageComponents;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.EstablishmentDetails;

public sealed class EstablishmentDetailsWebsiteDisplayTests : IntegrationTestsBase
{
    [Theory]
    [InlineData("https://test.example", "https://test.example")]
    [InlineData("school.sh.example", "https://school.sh.example")]
    public async Task Website_Protocol_Is_Normalised(string seededWebsite, string linkedWebsite)
    {
        EstablishmentDetailsModel establishment =
            EstablishmentDetailsBuilder.Create()
                .WithUrn(100000)
                .WithWebsite(seededWebsite)
                .Build();

        EstablishmentDetailsReadModel readModel = new() { Establishment = establishment };

        UseCaseResponse<EstablishmentDetailsReadModel> stubResponse =
            UseCaseResponse<EstablishmentDetailsReadModel>.Success(readModel);

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

        TextContent websiteValue = page.GetEstablishmentDetailsTable().Rows["Website"];

        Assert.Equal(seededWebsite, websiteValue.Text);

        Assert.NotNull(websiteValue.Link);
        Assert.Equal(linkedWebsite, websiteValue.Link.Url);
        Assert.True(websiteValue.Link.OpensInNewWindow);

        string[] externalLinkSecurityAttributes = ["noreferrer", "noopener"];

        Assert.Equivalent(externalLinkSecurityAttributes, websiteValue.Link.SecurityAttributes);
    }
}
