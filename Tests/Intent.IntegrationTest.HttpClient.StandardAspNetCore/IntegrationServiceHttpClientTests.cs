using IdentityServer4.Models;
using Intent.IntegrationTest.HttpClient.Common;
using Intent.IntegrationTest.HttpClient.StandardAspNetCore.TestUtils;
using Microsoft.Extensions.DependencyInjection;
using Standard.AspNetCore.TestApplication.Api.Controllers;
using Standard.AspNetCore.TestApplication.Application.Common.Exceptions;
using Standard.AspNetCore.TestApplication.Application.Implementation;
using Standard.AspNetCore.TestApplication.Application.IntegrationServices.IntegrationServiceProxy;
using Standard.AspNetCore.TestApplication.Application.Interfaces;
using Standard.AspNetCore.TestApplication.Domain.Common.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace Intent.IntegrationTest.HttpClient.StandardAspNetCore;

public class IntegrationServiceHttpClientTests : SystemTestCollectionDefinition
{
    public IntegrationServiceHttpClientTests(ITestOutputHelper outputHelper)
    {
        OutputHelper = outputHelper;
    }

    private ITestOutputHelper OutputHelper { get; }
    
    [Fact]
    public async Task TestQueryParamOperation()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var integrationService = sp.GetService<IIntegrationServiceProxyClient>()!;
        await integrationService.QueryParamOpAsync(IntegrationService.DefaultString, IntegrationService.DefaultInt);
    }

    [Fact]
    public async Task TestBodyParamOperation()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var integrationService = sp.GetService<IIntegrationServiceProxyClient>()!;
        await integrationService.BodyParamOpAsync(CustomDTO.Create(IntegrationService.ReferenceNumber));
    }

    [Fact]
    public async Task TestFormParamOperation()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var integrationService = sp.GetService<IIntegrationServiceProxyClient>()!;
        await integrationService.FormParamOpAsync(IntegrationService.DefaultString, IntegrationService.DefaultInt);
    }

    [Fact]
    public async Task TestHeaderParamOperation()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var integrationService = sp.GetService<IIntegrationServiceProxyClient>()!;
        await integrationService.HeaderParamOpAsync(IntegrationService.DefaultString);
    }

    [Fact]
    public async Task TestRouteParamOperation()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var integrationService = sp.GetService<IIntegrationServiceProxyClient>()!;
        await integrationService.RouteParamOpAsync(IntegrationService.DefaultString);
    }

    [Fact]
    public async Task ThrowsExceptionOperation()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var integrationService = sp.GetService<IIntegrationServiceProxyClient>()!;
        var exception = await Assert.ThrowsAsync<HttpClientRequestException>(async () => { await integrationService.ThrowsExceptionAsync(); });
        Assert.NotEmpty(exception.Message);
        Assert.Contains(IntegrationService.ExceptionMessage, exception.ResponseContent);
    }
    
    [Fact]
    public async Task TestGetPrimitiveGuid()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var integrationService = sp.GetService<IIntegrationServiceProxyClient>()!;
        var result = await integrationService.GetWrappedPrimitiveGuidAsync();
        Assert.Equal(IntegrationService.DefaultGuid, result);
        result = await integrationService.GetPrimitiveGuidAsync();
        Assert.Equal(IntegrationService.DefaultGuid, result);
    }

    [Fact]
    public async Task TestGetPrimitiveString()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var integrationService = sp.GetService<IIntegrationServiceProxyClient>()!;
        var result = await integrationService.GetWrappedPrimitiveStringAsync();
        Assert.Equal(IntegrationService.DefaultString, result);
        result = await integrationService.GetPrimitiveStringAsync();
        Assert.Equal(IntegrationService.DefaultString, result);
    }

    [Fact]
    public async Task TestGetPrimitiveInt()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var integrationService = sp.GetService<IIntegrationServiceProxyClient>()!;
        var result = await integrationService.GetWrappedPrimitiveIntAsync();
        Assert.Equal(IntegrationService.DefaultInt, result);
        result = await integrationService.GetPrimitiveIntAsync();
        Assert.Equal(IntegrationService.DefaultInt, result);
    }

    [Fact]
    public async Task TestGetPrimitiveStringList()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var integrationService = sp.GetService<IIntegrationServiceProxyClient>()!;
        var result = await integrationService.GetPrimitiveStringListAsync();
        Assert.Equal(new List<string> { IntegrationService.DefaultString }, result);
    }
    
    [Fact]
    public async Task TestGetInvoiceOpWithReturnTypeWrapped()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var integrationService = sp.GetService<IIntegrationServiceProxyClient>()!;
        var result = await integrationService.GetInvoiceOpWithReturnTypeWrappedAsync();
        Assert.Equal(IntegrationService.ReferenceNumber, result.ReferenceNumber);
    }
    
    private static Action<IServiceCollection> GetDiServices()
    {
        var serviceMock = new IntegrationService();
        var mockUnitOfWork = new MockUnitOfWork();
        return x => x.AddTransient<IIntegrationService>(_ => serviceMock)
                .AddTransient<IUnitOfWork>(_ => mockUnitOfWork);
    }

    public class MockUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(0);
        }
    }
}