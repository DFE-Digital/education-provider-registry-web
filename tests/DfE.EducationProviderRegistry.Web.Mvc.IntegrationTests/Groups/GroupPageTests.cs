using Bogus;
using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Document.Anglesharp;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.GroupDetails;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Response;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestDoubles;
using DfE.EducationProviderRegistry.Web.PageComponents.Pages;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.Groups;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Groups;

public sealed class GroupPageTests
{
    [Fact]
    public async Task GetGroupPage_NoLinkedEstablishments_Displayed_No_Establishments()
    {
        // Arrange
        GroupReadModel readModel =
            GroupReadModelBuilder.Create().Build();

        UseCaseResponse<GroupReadModelResponse> stubResponse =
                UseCaseResponse<GroupReadModelResponse>.Success(
                    new GroupReadModelResponse()
                    {
                        Group = readModel
                    });

        UseCaseStub<GetGroupByGroupUidRequest, GroupReadModelResponse> stubUseCase = new(stubResponse);

        using WebApplicationFactory<Program> factory = WebApplicationFactoryProvider.StubUseCase(stubUseCase);

        using HttpClient client = factory.CreateClient();

        // Act
        using HttpResponseMessage httpResponseMessage =
            await client.GetAsync(
                requestUri: GroupRoutes.Details(readModel.GroupUID),
                cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        using IHtmlDocument document = await httpResponseMessage.AssertSuccessfulHtmlResponseAsync();
        GroupPage page = new(SearchContext.From(document));

        Assert.Empty(page.Establishments());
        Assert.Equal("No centres are currently recorded for this group.", page.NoEstablishmentsMessage());
    }

    [Fact]
    public async Task GetGroupPage_DisplaysLinkedEstablishments()
    {
        // Arrange
        IReadOnlyList<EstablishmentReadModel> stubbedResponseModels =
            EstablishmentReadModelTestDoubles.Create(count: 3);


        GroupReadModel readModel =
            GroupReadModelBuilder.Create()
            .WithEstablishment(stubbedResponseModels[0])
            .WithEstablishment(stubbedResponseModels[1])
            .WithEstablishment(stubbedResponseModels[2])
            .Build();

        UseCaseResponse<GroupReadModelResponse> stubResponse =
                UseCaseResponse<GroupReadModelResponse>.Success(
                    new GroupReadModelResponse()
                    {
                        Group = readModel
                    });

        UseCaseStub<GetGroupByGroupUidRequest, GroupReadModelResponse> stubUseCase = new(stubResponse);

        using WebApplicationFactory<Program> factory = WebApplicationFactoryProvider.StubUseCase(stubUseCase);

        using HttpClient client = factory.CreateClient();

        // Act
        using HttpResponseMessage httpResponseMessage =
            await client.GetAsync(
                requestUri: GroupRoutes.Details(readModel.GroupUID),
                cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        using IHtmlDocument document = await httpResponseMessage.AssertSuccessfulHtmlResponseAsync();
        GroupPage page = new(SearchContext.From(document));

        IReadOnlyCollection<GroupEstablishment> displayedEstablishments = page.Establishments();

        Assert.Equal(stubbedResponseModels.Count, displayedEstablishments.Count);

        foreach (EstablishmentReadModel expected in stubbedResponseModels)
        {
            GroupEstablishment actual =
                Assert.Single(
                    displayedEstablishments,
                    establishment => establishment.Urn == expected.Urn);

            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Urn, actual.Urn);

            Assert.Equal(
                expected: EstablishmentRoutes.Details(expected.Urn).ToString(),
                actual: actual.Link.Url);
            Assert.False(actual.Link.OpensInNewWindow);
            Assert.Empty(actual.Link.SecurityAttributes);
        }
    }
}


public static class EstablishmentReadModelTestDoubles
{
    public static IReadOnlyList<EstablishmentReadModel> Create(int count = 1)
    {
        Faker faker = new();
        List<EstablishmentReadModel> establishments = new(count);

        for (int i = 0; i < count; i++)
        {
            establishments.Add(
                new EstablishmentReadModel
                {
                    Name = faker.Company.CompanyName(),
                    Urn = faker.Random.Number(100000, 999999).ToString()
                });
        }

        return establishments;
    }
}
