using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Providers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Pipeline;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Pipeline.Steps;
using DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Infrastructure.Pipeline.Steps.TestDoubles;
using Moq;
using System.Collections.ObjectModel;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Infrastructure.Pipeline.Steps;

public sealed class FacetQueryDispatchStepUnitTests
{
    [Fact]
    public async Task Execute_Throws_WhenIdsMissing()
    {
        // arrange
        Mock<IFacetProvider<Establishment>> providerMock =
            FacetProviderTestDouble.Mock();
        FacetQueryDispatchStep step = new(providerMock.Object);

        SearchPipelineContext context =
            SearchPipelineContextBuilder.BuildContext(null, ["phase"]);

        // act
        Func<Task> act = () => Task.Run(() =>
            step.Execute(context, CancellationToken.None));

        // assert
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Contains("PipelineContext does not contain a value of type", ex.Message);
    }

    [Fact]
    public async Task Execute_Throws_WhenFacetNamesMissing()
    {
        // arrange
        Mock<IFacetProvider<Establishment>> providerMock =
            FacetProviderTestDouble.Mock();
        FacetQueryDispatchStep step = new(providerMock.Object);
        ReadOnlyCollection<string> ids = new(["10001"]);
        SearchPipelineContext context =
            SearchPipelineContextBuilder.BuildContext(ids, null);

        // act
        Func<Task> act = () => Task.Run(() =>
            step.Execute(context, CancellationToken.None));

        // assert
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Contains("PipelineContext does not contain a value of type", ex.Message);
    }

    [Fact]
    public async Task Execute_Throws_WhenFacetNameIsEmpty()
    {
        // arrange
        Mock<IFacetProvider<Establishment>> providerMock =
            FacetProviderTestDouble.Mock();
        FacetQueryDispatchStep step = new(providerMock.Object);
        ReadOnlyCollection<string> ids = new(["10001"]);
        List<string> facetNames = [""];
        SearchPipelineContext context =
            SearchPipelineContextBuilder.BuildContext(ids, facetNames);

        // act
        Func<Task> act = () => Task.Run(() =>
            step.Execute(context, CancellationToken.None));

        // assert
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Contains("Facet name cannot be null or empty", ex.Message);
    }

    [Fact]
    public async Task Execute_Throws_WhenCancellationRequested()
    {
        // arrange
        Mock<IFacetProvider<Establishment>> providerMock =
            FacetProviderTestDouble.Mock();
        FacetQueryDispatchStep step = new(providerMock.Object);
        ReadOnlyCollection<string> ids = new(["10001"]);
        List<string> facetNames = ["phase"];
        SearchPipelineContext context =
            SearchPipelineContextBuilder.BuildContext(ids, facetNames);
        CancellationTokenSource cts = new();
        cts.Cancel();

        // act
        Func<Task> act = () => Task.Run(() =>
            step.Execute(context, cts.Token));

        // assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task Execute_SetsEmptyTaskList_WhenFacetNamesEmpty()
    {
        // arrange
        Mock<IFacetProvider<Establishment>> providerMock =
            FacetProviderTestDouble.Mock();
        FacetQueryDispatchStep step = new(providerMock.Object);
        ReadOnlyCollection<string> ids = new(["10001"]);
        List<string> facetNames = [];
        SearchPipelineContext context =
            SearchPipelineContextBuilder.BuildContext(ids, facetNames);

        // act
        await Task.Run(() =>
            step.Execute(
                context, CancellationToken.None),
                TestContext.Current.CancellationToken);

        // assert
        List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)> tasks =
            context.Get<List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)>>();

        Assert.Empty(tasks);
    }

    [Fact]
    public async Task Execute_DispatchesFacetTasksCorrectly()
    {
        // arrange
        Mock<IFacetProvider<Establishment>> providerMock =
            FacetProviderTestDouble.MockFor([new FacetResult("Primary", 10)]);
        FacetQueryDispatchStep step = new(providerMock.Object);
        ReadOnlyCollection<string> ids = new(["10001"]);
        List<string> facetNames = ["phase", "type"];

        SearchPipelineContext context =
            SearchPipelineContextBuilder.BuildContext(ids, facetNames);

        // act
        await Task.Run(() =>
            step.Execute(
                context, CancellationToken.None),
                TestContext.Current.CancellationToken);

        // assert
        List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)> tasks =
            context.Get<List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)>>();

        Assert.Equal(2, tasks.Count);
        Assert.Equal("phase", tasks[0].FacetName);
        Assert.Equal("type", tasks[1].FacetName);

        IReadOnlyList<FacetResult> results0 = await tasks[0].Task;
        IReadOnlyList<FacetResult> results1 = await tasks[1].Task;

        Assert.Single(results0);
        Assert.Single(results1);
        Assert.Equal("Primary", results0[0].Value);
        Assert.Equal(10, results0[0].Count);
    }
}
