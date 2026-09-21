using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.DownloadDatasets.Application.UseCases;
using DfE.EducationProviderRegistry.Core.Query.DownloadDatasets.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.DownloadDatasets.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Web.Mvc.Extensions;
using DfE.EducationProviderRegistry.Web.Mvc.Features.DownloadDatasets.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.DownloadDatasets.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Groups;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search;
using DfE.EducationProviderRegistry.Web.Mvc.Middleware;
using DfE.EducationProviderRegistry.Web.Mvc.Settings;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.ObjectModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllersWithViews()
    .AddRazorOptions((options) =>
    {
        options.ViewLocationExpanders.Add(new FeatureViewLocationExpander());
    })
    .AddApplicationPart(typeof(
        DfE.EducationProviderRegistry.Web.ViewComponents.Table.SharedGovUkTableViewComponent).Assembly);


builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.Always;
});

builder.Services
    .AddEstablishments()
    .AddGroups()
    .AddSearch(builder.Configuration)
    .AddPostgresDatabase(builder.Configuration);

// TODO: temp addition web side!-----------------------------------------------------
builder.Services
    .AddScoped<IUseCase<
        DownloadDatasetsRequest,
        UseCaseResponse<DownloadDatasetsResponse>>, DownloadDatasetsUseCase>();

builder.Services
    .TryAddSingleton<IMapper<
        DownloadDatasetsResponse,
        DownloadedDatasetViewModel>,
        DownloadResponseToViewModelMapper>();

//----------------------------------------------------------------------------------


builder.Services.Configure<ClaritySettings>(
    builder.Configuration.GetSection(nameof(ClaritySettings))
);

builder.Services.Configure<GoogleAnalyticsSettings>(
    builder.Configuration.GetSection(nameof(GoogleAnalyticsSettings))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseStatusCodePagesWithReExecute("/not-found");

app.UseSecurityHeaders();

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    // Remove server header
    context.Response.Headers.Remove("Server");

    // Basic hardening headers
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

    // Restrict browser features ect
    context.Response.Headers["Permissions-Policy"] =
        "geolocation=(), microphone=(), camera=(), browsing-topics=()";

    await next();
});

app.UseCookiePolicy();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();