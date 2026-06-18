using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.ApplicationDtos;
using DfE.EducationProviderRegistry.Web.Mvc.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Search.TempInfrastructureDELETE;
using DfE.EducationProviderRegistry.Web.Mvc.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using DfE.EducationProviderRegistry.Web.Mvc.ViewModels.Pages;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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


builder.Services.AddTransient<
    IMapper<List<EstablishmentSearchResultDto>, SearchResultsPageViewModel>,
    SearchResultsPageViewModelMapper>();
builder.Services.AddTransient<
    IMapper<EstablishmentSearchResultDto, GovUkTable>,
    SearchResultsEstablishmentSummaryTableMapper>();

builder.Services.AddTransient<
    IMapper<EstablishmentDto, EstablishmentDetailsPageViewModel>,
    EstablishmentDetailsPageViewModelMapper>();
builder.Services.AddTransient<
    IMapper<EstablishmentBasicDetailsDto, GovUkTable>,
    EstablishmentDetailsBasicDetailsTableMapper>();
builder.Services.AddTransient<
    IMapper<List<EstablishmentGovernorDto>, GovUkTable>,
    EstablishmentDetailsGovernorsTableMapper>();
builder.Services.AddTransient<
    IMapper<List<EstablishmentHistoryDto>, GovUkTable>,
    EstablishmentDetailsHistoryTableMapper>();

builder.Services.AddTransient<
    IMapper<GroupDto, GroupDetailsPageViewModel>,
    GroupDetailsPageViewModelMapper>();
builder.Services.AddTransient<
    IMapper<GroupBasicDetailsDto, GovUkTable>,
    GroupDetailsBasicDetailsTableMapper>();
builder.Services.AddTransient<
    IMapper<List<GroupAcademiesDto>, GovUkTable>,
    GroupDetailsAcademiesTableMapper>();
builder.Services.AddTransient<
    IMapper<List<GroupTrusteesDto>, GovUkTable>,
    GroupDetailsTrusteesTableMapper>();
builder.Services.AddTransient<
    IMapper<List<GroupMembersDto>, GovUkTable>,
    GroupDetailsMembersTableMapper>();

// Temp registratons for search.
builder.Services
    .AddScoped<ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets>, DummySearchServiceAdapter>()
    .AddSingleton<IMapper<EstablishmentSearchResults, SearchResultsViewModel>, SearchResultsToViewModelMapper>()
    .AddSingleton<IMapper<EstablishmentSearchResult, GovUkTable>, SearchResultToTableViewModelMapper>()
    .AddScoped<IUseCase<SearchRequest, UseCaseResponse<SearchResponse>>, SearchUseCase>();

// Bind search criteria configuration options.
builder.Services.AddOptions<SearchCriteria>()
    .Configure<IConfiguration>((settings, configuration) =>
        configuration
            .GetSection(nameof(SearchCriteria))
            .Bind(settings));

// Register strongly typed configuration instances.
builder.Services.AddSingleton(serviceProvider =>
    serviceProvider.GetRequiredService<IOptions<SearchCriteria>>().Value);

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
