using System;
using System.Collections.Generic;
using System.Text;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Breadcrumbs.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Shared.Breadcrumbs.Stores;
using Microsoft.AspNetCore.Http;
using Moq;
using Org.BouncyCastle.Tls;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Shared.Breadcrumbs;

public sealed class SessionBreadcrumbJourneyStoreTests
{
    private readonly SessionBreadcrumbJourneyStore _sut;

    public SessionBreadcrumbJourneyStoreTests()
    {
        DefaultHttpContext context = new();
        context.Session = new SessionTestDouble();

        Mock<IHttpContextAccessor> accessor = new();

        accessor
            .Setup(x => x.HttpContext)
            .Returns(context);

        _sut = new SessionBreadcrumbJourneyStore(
            accessor.Object);
    }

    [Fact]
    public void Get_WhenNothingStored_ReturnsEmptyJourney()
    {
        BreadcrumbJourney result = _sut.Get();

        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }

    // get returns journey that was previously saved
    [Fact]
    public void Save_ThenGet_ReturnsStoredJourney()
    {
        BreadcrumbJourney journey =
            new()
            {
                Items =
                [
                    new BreadcrumbItem("Home", "/"),
                    new BreadcrumbItem("Search", "/search")
                ]
            };

        _sut.Save(journey);
        BreadcrumbJourney result = _sut.Get();

        Assert.Equal(2, result.Items.Count);
        Assert.Equal("Home", result.Items[0].Title);
        Assert.Equal("/", result.Items[0].Url);
        Assert.Equal("Search", result.Items[1].Title);
        Assert.Equal("/search", result.Items[1].Url);
    }

    // save calls stores the correct key and serialized value in session
    [Fact]
    public void Save_CallsSessionSetWithCorrectKeyAndSerializedValue()
    {
        BreadcrumbJourney journey =
            new()
            {
                Items =
                [
                    new BreadcrumbItem("Home", "/"),
                    new BreadcrumbItem("Search", "/search")
                ]
            };

        _sut.Save(journey);
        BreadcrumbJourney result = _sut.Get();

        Assert.NotEmpty(result.Items);
    }

    [Fact]
    public void Clear_RemovesStoredJourney()
    {
        BreadcrumbJourney journey =
            new()
            {
                Items =
                [
                    new BreadcrumbItem("Home", "/")
                ]
            };

        _sut.Save(journey);

        _sut.Clear();

        BreadcrumbJourney result = _sut.Get();

        Assert.Empty(result.Items);
    }

    private sealed class SessionTestDouble : ISession
    {
        private readonly Dictionary<string, byte[]> _values = [];

        public string Id { get; } = Guid.NewGuid().ToString();

        public bool IsAvailable => true;

        public IEnumerable<string> Keys => _values.Keys;

        public void Clear()
        {
            _values.Clear();
        }

        public Task CommitAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task LoadAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Remove(string key)
        {
            _values.Remove(key);
        }

        public void Set(
            string key,
            byte[] value)
        {
            _values[key] = value;
        }

        public bool TryGetValue(
            string key,
            out byte[] value)
        {
            return _values.TryGetValue(
                key,
                out value!);
        }
    }
}
