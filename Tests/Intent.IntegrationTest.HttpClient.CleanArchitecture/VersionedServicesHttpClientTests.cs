using CleanArchitecture.Comprehensive.Api.Configuration;
using CleanArchitecture.Comprehensive.Api.Controllers;
using CleanArchitecture.Comprehensive.Api.Services;
using CleanArchitecture.Comprehensive.Application;
using CleanArchitecture.Comprehensive.Application.Common.Eventing;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using CleanArchitecture.Comprehensive.Application.IntegrationServices;
using CleanArchitecture.Comprehensive.Application.Versioned.TestCommandV1;
using CleanArchitecture.Comprehensive.Application.Versioned.TestCommandV2;
using CleanArchitecture.Comprehensive.Domain.Common.Interfaces;
using Intent.IntegrationTest.HttpClient.CleanArchitecture.TestUtils;
using Intent.IntegrationTest.HttpClient.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;
using TestCommandV1 = CleanArchitecture.Comprehensive.Application.IntegrationServices.Services.Versioned.TestCommandV1;
using TestCommandV2 = CleanArchitecture.Comprehensive.Application.IntegrationServices.Services.Versioned.TestCommandV2;

namespace Intent.IntegrationTest.HttpClient.CleanArchitecture;

[Collection("VersionedTests")]
public class VersionedServicesHttpClientTests
{
    public VersionedServicesHttpClientTests(ITestOutputHelper outputHelper)
    {
        OutputHelper = outputHelper;
    }

    private ITestOutputHelper OutputHelper { get; }

    private const int ApiPortNumber = 5005;

    [Fact]
    public async Task Test_TestCommandV1()
    {
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(VersionedController).Assembly, ApiPortNumber);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var service = sp.GetService<ITestVersionedProxy>()!;
        await service.TestCommandV1Async(new TestCommandV1 { Value = TestCommandV1Handler.ExpectedInput });
    }

    [Fact]
    public async Task Test_TestQueryV1()
    {
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(VersionedController).Assembly, ApiPortNumber);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var service = sp.GetService<ITestVersionedProxy>()!;
        var result = await service.TestQueryV1Async("123");
        Assert.Equal(123, result);
    }

    [Fact]
    public async Task Test_TestCommandV2()
    {
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(VersionedController).Assembly, ApiPortNumber);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var service = sp.GetService<ITestVersionedProxy>()!;
        await service.TestCommandV2Async(new TestCommandV2 { Value = TestCommandV2Handler.ExpectedInput });
    }

    [Fact]
    public async Task Test_TestQueryV2()
    {
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(VersionedController).Assembly, ApiPortNumber);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var service = sp.GetService<ITestVersionedProxy>()!;
        var result = await service.TestQueryV2Async("123");
        Assert.Equal(128, result);
    }

    private static Action<IServiceCollection> GetDiServices()
    {
        var mockUnitOfWork = new MockUnitOfWork();
        var mockEventBus = new MockEventBus();
        return x => x.AddApplication(Substitute.For<IConfiguration>())
            .AddTransient<IUnitOfWork>(_ => mockUnitOfWork)
            .AddTransient<IEventBus>(_ => mockEventBus)
            .AddTransient<ICurrentUserService, CurrentUserService>()
            .AddHttpContextAccessor()
            .ConfigureApiVersioning();
    }

    public class MockUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(0);
        }
    }

    public class MockEventBus : IEventBus
    {
        public void Publish<T>(T message) where T : class { }

        public Task FlushAllAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Send<T>(T message) where T : class
        {
            
        }

        public void Send<T>(T message, Uri address) where T : class
        {
            
        }
    }
}