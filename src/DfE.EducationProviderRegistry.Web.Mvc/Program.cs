using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Web.Mvc.Extensions;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Groups;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
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

builder.Services.AddDbContextFactory<EducationProviderRegistryDbContext>(options =>
{
    string connectionString = builder.Configuration["eprweb_eprdat_dotnet_db_connection"]
            ?? throw new InvalidOperationException(
                "Database connection string not configured.");

    options.UseNpgsql(connectionString)
           .EnableSensitiveDataLogging()
           .EnableDetailedErrors()
           .LogTo(Console.WriteLine, LogLevel.Information);
});

// Search registrations.
builder.Services
    .AddSingleton<IMapper<
        UseCaseResponse<SearchResponse>, SearchResultsViewModel>, SearchResultsToViewModelMapper>()
    .AddSingleton<IMapper<
        IReadOnlyCollection<SearchFacet>, List<FacetViewModel>>, FacetResultsToViewModelMapper>()
    .AddSingleton<IMapper<
        IReadOnlyCollection<EstablishmentSearchResult>, List<GovUkTable>>, EstablishmentSearchResultsToViewModelMapper>()
    .AddSingleton<IMapper<
        Dictionary<string, List<string>>?, ReadOnlyCollection<FilterRequest>>, SelectedFacetsToFilterRequestsMapper>();
builder.Services.AddSearchDependencies();

// Bind search criteria configuration options.
builder.Services.AddOptions<SearchCriteria>()
    .Configure<IConfiguration>((settings, configuration) =>
        configuration
            .GetSection(nameof(SearchCriteria))
            .Bind(settings));

builder.Services
    .AddEstablishments()
    .AddGroups()
    .AddSearchDependencies()
    .AddInfraSearchDependencies(builder.Configuration)
    .AddInfraSearchFilterDependencies(builder.Configuration);

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