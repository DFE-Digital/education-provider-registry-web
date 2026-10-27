using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.IntegrationTests.Abstractions;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishmentByUrn;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Establishments;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestDoubles;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.EstablishmentDetails;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.PageComponents;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Establishments.Details;

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

        EstablishmentDetailsReadModel readModel = new()
        {
            Establishment = establishment
        };

        UseCaseResponse<EstablishmentDetailsReadModel> stubResponse =
            UseCaseResponse<EstablishmentDetailsReadModel>.Success(readModel);

        UseCaseStub<GetEstablishmentByUrnRequest, EstablishmentDetailsReadModel> stubUseCase = new(stubResponse);

        using WebApplicationFactory<Program> factory = WebApplicationFactoryProvider.StubUseCase(stubUseCase);

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

    [Fact]
    public async Task Website_Row_Is_Not_Displayed_When_Website_Is_Not_Present()
    {
        // Arrange
        EstablishmentDetailsModel establishment =
            EstablishmentDetailsBuilder.Create()
                .WithUrn(100000)
                .WithWebsite(null)
                .Build();

        EstablishmentDetailsReadModel readModel = new()
        {
            Establishment = establishment
        };

        UseCaseResponse<EstablishmentDetailsReadModel> stubResponse =
            UseCaseResponse<EstablishmentDetailsReadModel>.Success(readModel);

        UseCaseStub<GetEstablishmentByUrnRequest, EstablishmentDetailsReadModel> stubUseCase = new(stubResponse);

        using WebApplicationFactory<Program> factory =
            WebApplicationFactoryProvider.StubUseCase(stubUseCase);

        using HttpClient client = factory.CreateClient();

        // Act
        using HttpResponseMessage response =
            await client.GetAsync(
                EstablishmentDetailsRoute.ForUrn("100000"),
                TestContext.Current.CancellationToken);

        // Assert
        using IHtmlDocument document =
            await response.AssertSuccessfulHtmlResponseAsync();

        EstablishmentDetailsPage page = new(document);
        GovUkTable table = page.GetEstablishmentDetailsTable();

        Assert.NotEmpty(table.Rows);
        Assert.False(table.Rows.ContainsKey("Website"));
    }
}
