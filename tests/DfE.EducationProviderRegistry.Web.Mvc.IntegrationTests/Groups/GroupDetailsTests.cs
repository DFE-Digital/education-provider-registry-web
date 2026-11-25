using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Document.Anglesharp;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.GroupDetails;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Response;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestDoubles;
using DfE.EducationProviderRegistry.Web.PageComponents.Pages;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.Groups;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Groups;

public sealed class GroupDetailsTests
{
    [Fact]
    public async Task GetGroupDetails_NoLinkedEstablishments_Displayed_No_Establishments()
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

        Assert.Empty(page.LinkedEstablishments());
        Assert.Equal("No centres are currently recorded for this group.", page.NoEstablishmentsMessage());
    }
}
