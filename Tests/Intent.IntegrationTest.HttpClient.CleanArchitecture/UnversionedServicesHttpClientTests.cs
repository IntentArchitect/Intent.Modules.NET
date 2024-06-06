using System.IdentityModel.Tokens.Jwt;
using CleanArchitecture.Comprehensive.Api.Configuration;
using CleanArchitecture.Comprehensive.Api.Controllers;
using CleanArchitecture.Comprehensive.Api.Services;
using CleanArchitecture.Comprehensive.Application;
using CleanArchitecture.Comprehensive.Application.Common.Eventing;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using CleanArchitecture.Comprehensive.Application.IntegrationServices;
using CleanArchitecture.Comprehensive.Application.Unversioned.Test;
using CleanArchitecture.Comprehensive.Domain.Common.Interfaces;
using Intent.IntegrationTest.HttpClient.CleanArchitecture.TestUtils;
using Intent.IntegrationTest.HttpClient.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;
using TestCommand = CleanArchitecture.Comprehensive.Application.IntegrationServices.Services.Unversioned.TestCommand;

namespace Intent.IntegrationTest.HttpClient.CleanArchitecture;

[Collection("UnversionedTests")]
public class UnversionedServicesHttpClientTests
{
    public UnversionedServicesHttpClientTests(ITestOutputHelper outputHelper)
    {
        OutputHelper = outputHelper;
    }

    private ITestOutputHelper OutputHelper { get; }

    private const int ApiPortNumber = 5003;

    [Fact]
    public async Task Test_TestCommand()
    {
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(),
            typeof(UnversionedController).Assembly, ApiPortNumber);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var service = sp.GetService<ITestUnversionedProxy>()!;
        await service.TestAsync(new TestCommand { Value = TestCommandHandler.ExpectedInput });
    }

    [Fact]
    public async Task Test_TestQuery()
    {
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(),
            typeof(UnversionedController).Assembly, ApiPortNumber);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var service = sp.GetService<ITestUnversionedProxy>()!;
        var result = await service.TestAsync("789");
        Assert.Equal(789, result);
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