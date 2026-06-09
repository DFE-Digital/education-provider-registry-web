using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Web.Mvc.ApplicationDtos;
using DfE.EducationProviderRegistry.Web.Mvc.Mappers;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using Microsoft.AspNetCore.CookiePolicy;

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


builder.Services.AddScoped<
    IMapper<EstablishmentSearchResultDto, GovUkTable>,
    SearchResultsEstablishmentSummaryTableMapper>();
builder.Services.AddScoped<
    IMapper<EstablishmentBasicDetailsDto, GovUkTable>,
    EstablishmentDetailsBasicDetailsTableMapper>();
builder.Services.AddScoped<
    IMapper<List<EstablishmentGovernorDto>, GovUkTable>,
    EstablishmentDetailsGovernorsTableMapper>();
builder.Services.AddScoped<
    IMapper<List<EstablishmentHistoryDto>, GovUkTable>,
    EstablishmentDetailsHistoryTableMapper>();

builder.Services.AddScoped<
    IMapper<GroupBasicDetailsDto, GovUkTable>,
    GroupDetailsBasicDetailsTableMapper>();
builder.Services.AddScoped<
    IMapper<List<GroupAcademiesDto>, GovUkTable>,
    GroupDetailsAcademiesTableMapper>();
builder.Services.AddScoped<
    IMapper<List<GroupTrusteesDto>, GovUkTable>,
    GroupDetailsTrusteesTableMapper>();
builder.Services.AddScoped<
    IMapper<List<GroupMembersDto>, GovUkTable>,
    GroupDetailsMembersTableMapper>();

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
