using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Resolvers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Store;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.Factories;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Filters.Breadcrumbs.Strategies.JourneyRules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Filters.Breadcrumbs;

public sealed class BreadcrumbCompositionRootTests
{
    [Fact]
    public void AddBreadcrumbDependencies_ShouldReturnServices()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();

        IConfiguration configuration =
            new ConfigurationBuilder().Build();

        // Act
        IServiceCollection result =
            services.AddBreadcrumbDependencies(configuration);

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddBreadcrumbDependencies_ShouldRegisterRouteResolver()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();

        IConfiguration configuration =
            new ConfigurationBuilder().Build();

        // Act
        services.AddBreadcrumbDependencies(configuration);

        // Assert
        ServiceDescriptor? descriptor =
            services.FirstOrDefault(
                descriptor =>
                    descriptor.ServiceType ==
                    typeof(IBreadcrumbRouteResolver));

        Assert.NotNull(descriptor);
        Assert.Equal(
            typeof(BreadcrumbRouteResolver),
            descriptor.ImplementationType);

        Assert.Equal(
            ServiceLifetime.Singleton,
            descriptor.Lifetime);
    }

    [Fact]
    public void AddBreadcrumbDependencies_ShouldRegisterTitleResolver()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();

        IConfiguration configuration =
            new ConfigurationBuilder().Build();

        // Act
        services.AddBreadcrumbDependencies(configuration);

        // Assert
        ServiceDescriptor? descriptor =
            services.FirstOrDefault(
                descriptor =>
                    descriptor.ServiceType ==
                    typeof(IBreadcrumbTitleResolver));

        Assert.NotNull(descriptor);
        Assert.Equal(
            typeof(BreadcrumbTitleResolver),
            descriptor.ImplementationType);

        Assert.Equal(
            ServiceLifetime.Singleton,
            descriptor.Lifetime);
    }

    [Fact]
    public void AddBreadcrumbDependencies_ShouldRegisterJourneyStore()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();

        IConfiguration configuration =
            new ConfigurationBuilder().Build();

        // Act
        services.AddBreadcrumbDependencies(configuration);

        // Assert
        ServiceDescriptor? descriptor =
            services.FirstOrDefault(
                descriptor =>
                    descriptor.ServiceType ==
                    typeof(IBreadcrumbJourneyStore));

        Assert.NotNull(descriptor);
        Assert.Equal(
            typeof(SessionBreadcrumbJourneyStore),
            descriptor.ImplementationType);

        Assert.Equal(
            ServiceLifetime.Scoped,
            descriptor.Lifetime);
    }

    [Fact]
    public void AddBreadcrumbDependencies_ShouldRegisterJourneyRulesInOrder()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();

        IConfiguration configuration =
            new ConfigurationBuilder().Build();

        // Act
        services.AddBreadcrumbDependencies(configuration);

        // Assert
        ServiceDescriptor[] descriptors =
            services
                .Where(
                    descriptor =>
                        descriptor.ServiceType ==
                        typeof(IJourneyRule))
                .ToArray();

        Assert.Collection(
            descriptors,
            descriptor =>
                Assert.Equal(
                    typeof(EmptyJourneyRule),
                    descriptor.ImplementationType),
            descriptor =>
                Assert.Equal(
                    typeof(ExactUrlMatchRule),
                    descriptor.ImplementationType),
            descriptor =>
                Assert.Equal(
                    typeof(LastPathMatchRule),
                    descriptor.ImplementationType),
            descriptor =>
                Assert.Equal(
                    typeof(ExistingPathMatchRule),
                    descriptor.ImplementationType),
            descriptor =>
                Assert.Equal(
                    typeof(DefaultFallbackRule),
                    descriptor.ImplementationType));
    }

    [Fact]
    public void AddBreadcrumbDependencies_ShouldRegisterStrategyFactory()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();

        IConfiguration configuration =
            new ConfigurationBuilder().Build();

        // Act
        services.AddBreadcrumbDependencies(configuration);

        // Assert
        ServiceDescriptor? descriptor =
            services.FirstOrDefault(
                descriptor =>
                    descriptor.ServiceType ==
                    typeof(IBreadcrumbStrategyFactory));

        Assert.NotNull(descriptor);
        Assert.Equal(
            typeof(BreadcrumbStrategyFactory),
            descriptor.ImplementationType);

        Assert.Equal(
            ServiceLifetime.Scoped,
            descriptor.Lifetime);
    }

    [Fact]
    public void AddBreadcrumbDependencies_ShouldExecuteSuccessfully()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();

        IConfiguration configuration =
            new ConfigurationBuilder().Build();

        // Act
        Exception? exception =
            Record.Exception(
                () => services.AddBreadcrumbDependencies(configuration));

        // Assert
        Assert.Null(exception);
    }
}
