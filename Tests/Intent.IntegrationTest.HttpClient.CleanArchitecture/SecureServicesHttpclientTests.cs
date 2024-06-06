using CleanArchitecture.Comprehensive.Api.Configuration;
using CleanArchitecture.Comprehensive.Api.Controllers;
using CleanArchitecture.Comprehensive.Api.Services;
using CleanArchitecture.Comprehensive.Application;
using CleanArchitecture.Comprehensive.Application.Common.Eventing;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using CleanArchitecture.Comprehensive.Application.IntegrationServices;
using CleanArchitecture.Comprehensive.Application.IntegrationServices.Services.SecureServices;
using CleanArchitecture.Comprehensive.Domain.Common.Interfaces;
using Intent.IntegrationTest.HttpClient.CleanArchitecture.TestUtils;
using Intent.IntegrationTest.HttpClient.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace Intent.IntegrationTest.HttpClient.CleanArchitecture;

[Collection("SecureServicesTests")]
public class SecureServicesHttpclientTests
{
    public SecureServicesHttpclientTests(ITestOutputHelper outputHelper)
    {
        OutputHelper = outputHelper;
    }

    private ITestOutputHelper OutputHelper { get; }

    private const int IdPortNumber = 5007;
    private const int ApiPortNumber = 5008;

    [Fact]
    public async Task Test_SecureCommand()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper, IdPortNumber);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(),
            typeof(SecureServicesController).Assembly, ApiPortNumber, IdPortNumber);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var service = sp.GetService<ISecureServicesService>()!;
        await service.SecureAsync(new SecureCommand() { Message = "123" });
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