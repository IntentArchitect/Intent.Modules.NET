using CleanArchitecture.TestApplication.Api.Configuration;
using CleanArchitecture.TestApplication.Api.Controllers;
using CleanArchitecture.TestApplication.Api.Services;
using CleanArchitecture.TestApplication.Application;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using CleanArchitecture.TestApplication.Application.IntegrationServices.TestVersionedProxy;
using CleanArchitecture.TestApplication.Application.Versioned.TestCommandV1;
using CleanArchitecture.TestApplication.Application.Versioned.TestCommandV2;
using CleanArchitecture.TestApplication.Application.Versioned.TestQueryV1;
using CleanArchitecture.TestApplication.Domain.Common.Interfaces;
using Intent.IntegrationTest.HttpClient.CleanArchitecture.TestUtils;
using Intent.IntegrationTest.HttpClient.Common;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using TestCommandV1 = CleanArchitecture.TestApplication.Application.IntegrationServices.TestVersionedProxy.TestCommandV1;
using TestCommandV2 = CleanArchitecture.TestApplication.Application.IntegrationServices.TestVersionedProxy.TestCommandV2;
using TestQueryV1 = CleanArchitecture.TestApplication.Application.IntegrationServices.TestVersionedProxy.TestQueryV1;

namespace Intent.IntegrationTest.HttpClient.CleanArchitecture;

public class VersionedServicesHttpClientTests : SystemTestCollectionDefinition
{
    public VersionedServicesHttpClientTests(ITestOutputHelper outputHelper)
    {
        OutputHelper = outputHelper;
    }

    private ITestOutputHelper OutputHelper { get; }
    
    [Fact]
    public async Task Test_TestCommandV1()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(VersionedController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var service = sp.GetService<ITestVersionedProxyClient>()!;
        await service.TestCommandV1Async(new TestCommandV1 { Value = TestCommandV1Handler.ExpectedInput });
    }
    
    [Fact]
    public async Task Test_TestQueryV1()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(VersionedController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var service = sp.GetService<ITestVersionedProxyClient>()!;
        var result = await service.TestQueryV1Async(new TestQueryV1 { Value = "123" });
        Assert.Equal(123, result);
    }
    
    [Fact]
    public async Task Test_TestCommandV2()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(VersionedController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var service = sp.GetService<ITestVersionedProxyClient>()!;
        await service.TestCommandV2Async(new TestCommandV2 { Value = TestCommandV2Handler.ExpectedInput });
    }
    
    [Fact]
    public async Task Test_TestQueryV2()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(VersionedController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var service = sp.GetService<ITestVersionedProxyClient>()!;
        var result = await service.TestQueryV2Async(new TestQueryV2 { Value = "123" });
        Assert.Equal(128, result);
    }

    private static Action<IServiceCollection> GetDiServices()
    {
        var mockUnitOfWork = new MockUnitOfWork();
        return x => x.AddApplication()
            .AddTransient<IUnitOfWork>(_ => mockUnitOfWork)
            .AddTransient<ICurrentUserService, CurrentUserService>()
            .ConfigureApiVersioning();
    }

    public class MockUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(0);
        }
    }
}