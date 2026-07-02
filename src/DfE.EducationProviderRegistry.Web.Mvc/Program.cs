using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Web.Mvc.ApplicationDtos;
using DfE.EducationProviderRegistry.Web.Mvc.Extensions;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Establishments;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Groups;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search;
using DfE.EducationProviderRegistry.Web.Mvc.ViewModels.Pages;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllersWithViews()
    .AddRazorOptions((options) =>
    {
        options.ViewLocationExpanders.Add(new FeatureViewLocationExpander());
    })
    .AddApplicationPart(typeof(DfE.EducationProviderRegistry.Web.ViewComponents.Table.SharedGovUkTableViewComponent).Assembly);


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
    IMapper<EstablishmentBasicDetailsDto, DfE.EducationProviderRegistry.Web.Mvc.ViewComponents.GovUkTable>,
    EstablishmentDetailsBasicDetailsTableMapper>();
builder.Services.AddTransient<
    IMapper<List<EstablishmentGovernorDto>, DfE.EducationProviderRegistry.Web.Mvc.ViewComponents.GovUkTable>,
    EstablishmentDetailsGovernorsTableMapper>();
builder.Services.AddTransient<
    IMapper<List<EstablishmentHistoryDto>, DfE.EducationProviderRegistry.Web.Mvc.ViewComponents.GovUkTable>,
    EstablishmentDetailsHistoryTableMapper>();


builder.Services.AddDbContext<EducationProviderRegistryDbContext>(
    (options) =>
    {
        string connectionString = builder.Configuration["eprweb-eprdat-dotnet-db-connection"]
            ?? throw new InvalidOperationException(
                "Database connection string not configured.");

        options.UseNpgsql(connectionString);
    });

builder.Services
    // Group registrations
    .AddGroups()
    // Search registrations.
    .AddSearch(builder.Configuration);

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
