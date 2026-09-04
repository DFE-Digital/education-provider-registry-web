using DfE.EducationProviderRegistry.Web.Mvc.Middleware;
using DfE.EducationProviderRegistry.Web.Mvc.SecurityPolicies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Extensions
{
    public class AddContetnSecurityPolicyTestHeader
    {
        [Fact]
        public async Task UseSecurityHeaders_AddsContentSecurityPolicyReportOnlyHeader()
        {
            // Arrange
            ServiceCollection services = new();
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            ApplicationBuilder appBuilder = new(serviceProvider);

            appBuilder.UseSecurityHeaders();

            RequestDelegate app = appBuilder.Build();

            DefaultHttpContext context = new();

            // Act
            await app(context);

            // Assert
            Assert.Equal(
                ContentSecurityPolicy.Value,
                context.Response.Headers.ContentSecurityPolicyReportOnly);
        }

        [Fact]
        public async Task UseSecurityHeaders_CallsNextMiddleware()
        {
            // Arrange
            ServiceCollection services = new();
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            ApplicationBuilder appBuilder = new(serviceProvider);

            bool nextMiddlewareCalled = false;

            appBuilder.UseSecurityHeaders();

            appBuilder.Run(context =>
            {
                nextMiddlewareCalled = true;
                return Task.CompletedTask;
            });

            RequestDelegate app = appBuilder.Build();

            DefaultHttpContext context = new();

            // Act
            await app(context);

            // Assert
            Assert.True(nextMiddlewareCalled);
        }
    }
}