using System.IdentityModel.Tokens.Jwt;
using CleanArchitecture.TestApplication.Api.Configuration;
using CleanArchitecture.TestApplication.Api.Controllers;
using CleanArchitecture.TestApplication.Api.Services;
using CleanArchitecture.TestApplication.Application;
using CleanArchitecture.TestApplication.Application.Common.Eventing;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using CleanArchitecture.TestApplication.Application.IntegrationServices;
using CleanArchitecture.TestApplication.Application.Unversioned.Test;
using CleanArchitecture.TestApplication.Domain.Common.Interfaces;
using Intent.IntegrationTest.HttpClient.CleanArchitecture.TestUtils;
using Intent.IntegrationTest.HttpClient.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;
using TestCommand = CleanArchitecture.TestApplication.Application.IntegrationServices.CleanArchitecture.TestApplication.Services.Unversioned.TestCommand;

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

        var service = sp.GetService<ITestUnversionedService>()!;
        await service.TestAsync(new TestCommand { Value = TestCommandHandler.ExpectedInput });
    }

    [Fact]
    public async Task Test_TestQuery()
    {
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(),
            typeof(UnversionedController).Assembly, ApiPortNumber);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var service = sp.GetService<ITestUnversionedService>()!;
        var result = await service.TestAsync("789");
        Assert.Equal(789, result);
    }

    private static Action<IServiceCollection> GetDiServices()
    {
        var mockUnitOfWork = new MockUnitOfWork();
        var mockEventBus = new MockEventBus();
        return x => x.AddApplication()
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
    }
}