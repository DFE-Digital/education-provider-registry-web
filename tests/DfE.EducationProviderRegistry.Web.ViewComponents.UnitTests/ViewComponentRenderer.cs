using DfE.EducationProviderRegistry.Web.ViewComponents.SummaryList;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.Diagnostics;

namespace DfE.EducationProviderRegistry.Web.ViewComponents.UnitTests;

internal sealed class ViewComponentRenderer : IDisposable
{
    private readonly ServiceProvider _services;

    public ViewComponentRenderer()
    {
        ServiceCollection services = new();

        services.AddLogging();

        services.AddSingleton<IWebHostEnvironment>(new TestHostingEnvironment
        {
            ApplicationName = typeof(GovUkSummaryListViewComponent).Assembly.FullName!,
            ContentRootPath = AppContext.BaseDirectory,
            WebRootPath = AppContext.BaseDirectory,
            ContentRootFileProvider = new PhysicalFileProvider(AppContext.BaseDirectory),
            WebRootFileProvider = new PhysicalFileProvider(AppContext.BaseDirectory)
        });

        DiagnosticListener listener = new("Test");
        services.AddSingleton<DiagnosticSource>(listener);
        services.AddSingleton(listener);

        services
            .AddMvc()
            .AddRazorRuntimeCompilation();

        _services = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        _services.Dispose();
    }

    public async Task<string> RenderAsync<TModel>(string viewPath, TModel? model) where TModel : class
    {
        DefaultHttpContext httpContext = new()
        {
            RequestServices = _services
        };

        using StringWriter writer = new();

        ActionContext actionContext = new(
            httpContext,
            new RouteData(),
            new ActionDescriptor()
        );

        ICompositeViewEngine viewEngine = _services.GetRequiredService<ICompositeViewEngine>();
        ViewEngineResult result = viewEngine.GetView(null, viewPath, false);

        if (!result.Success)
        {
            throw new InvalidOperationException(
                $"View not found: {string.Join(", ", result.SearchedLocations ?? [])}");
        }

        ViewContext viewContext = new(
            actionContext,
            result.View,
            new ViewDataDictionary(
                new EmptyModelMetadataProvider(),
                new ModelStateDictionary())
            {
                Model = model
            },
            new TempDataDictionary(
                httpContext,
                _services.GetRequiredService<ITempDataProvider>()),
            writer,
            new HtmlHelperOptions()
        );

        await result.View.RenderAsync(viewContext);

        return writer.ToString().Trim();
    }
}

public class TestHostingEnvironment : IWebHostEnvironment
{
    public string ApplicationName { get; set; } = default!;
    public IFileProvider ContentRootFileProvider { get; set; } = default!;
    public string ContentRootPath { get; set; } = default!;
    public string EnvironmentName { get; set; } = "Development";
    public IFileProvider WebRootFileProvider { get; set; } = default!;
    public string WebRootPath { get; set; } = default!;
}