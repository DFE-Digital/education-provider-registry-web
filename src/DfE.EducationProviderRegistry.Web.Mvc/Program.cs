using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Web.Mvc.ApplicationDtos;
using DfE.EducationProviderRegistry.Web.Mvc.Extensions;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Groups;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using DfE.EducationProviderRegistry.Web.Mvc.ViewModels.Pages;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllersWithViews()
    .AddRazorOptions((options) =>
    {
        options.ViewLocationExpanders.Add(new FeatureViewLocationExpander());
    });

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


builder.Services
    // Group registrations
    .AddGroups()
    // Search registrations.
    .AddSearch();

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
