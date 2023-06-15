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

public class IntegrationHttpClientTests
{
    public IntegrationHttpClientTests(ITestOutputHelper outputHelper)
    {
        OutputHelper = outputHelper;
    }

    private ITestOutputHelper OutputHelper { get; }
    
    [Fact]
    public async Task TestQueryParamOperation()
    {
        var serviceMock = new IntegrationService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<IIntegrationServiceProxyClient>()!;
        await invoiceService.QueryParamOpAsync(IntegrationService.DefaultString, IntegrationService.DefaultInt);
    }

    [Fact]
    public async Task TestBodyParamOperation()
    {
        var serviceMock = new IntegrationService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<IIntegrationServiceProxyClient>()!;
        await invoiceService.BodyParamOpAsync(CustomDTO.Create(IntegrationService.ReferenceNumber));
    }

    [Fact]
    public async Task TestFormParamOperation()
    {
        var serviceMock = new IntegrationService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<IIntegrationServiceProxyClient>()!;
        await invoiceService.FormParamOpAsync(IntegrationService.DefaultString, IntegrationService.DefaultInt);
    }

    [Fact]
    public async Task TestHeaderParamOperation()
    {
        var serviceMock = new IntegrationService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<IIntegrationServiceProxyClient>()!;
        await invoiceService.HeaderParamOpAsync(IntegrationService.DefaultString);
    }

    [Fact]
    public async Task TestRouteParamOperation()
    {
        var serviceMock = new IntegrationService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<IIntegrationServiceProxyClient>()!;
        await invoiceService.RouteParamOpAsync(IntegrationService.DefaultString);
    }

    [Fact]
    public async Task ThrowsExceptionOperation()
    {
        var serviceMock = new IntegrationService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<IIntegrationServiceProxyClient>()!;
        var exception = await Assert.ThrowsAsync<HttpClientRequestException>(async () => { await invoiceService.ThrowsExceptionAsync(); });
        Assert.NotEmpty(exception.Message);
        Assert.Contains(IntegrationService.ExceptionMessage, exception.ResponseContent);
    }
    
    [Fact]
    public async Task TestGetPrimitiveGuid()
    {
        var serviceMock = new IntegrationService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<IIntegrationServiceProxyClient>()!;
        var result = await invoiceService.GetWrappedPrimitiveGuidAsync();
        Assert.Equal(IntegrationService.DefaultGuid, result);
        result = await invoiceService.GetPrimitiveGuidAsync();
        Assert.Equal(IntegrationService.DefaultGuid, result);
    }

    [Fact]
    public async Task TestGetPrimitiveString()
    {
        var serviceMock = new IntegrationService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<IIntegrationServiceProxyClient>()!;
        var result = await invoiceService.GetWrappedPrimitiveStringAsync();
        Assert.Equal(IntegrationService.DefaultString, result);
        result = await invoiceService.GetPrimitiveStringAsync();
        Assert.Equal(IntegrationService.DefaultString, result);
    }

    [Fact]
    public async Task TestGetPrimitiveInt()
    {
        var serviceMock = new IntegrationService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<IIntegrationServiceProxyClient>()!;
        var result = await invoiceService.GetWrappedPrimitiveIntAsync();
        Assert.Equal(IntegrationService.DefaultInt, result);
        result = await invoiceService.GetPrimitiveIntAsync();
        Assert.Equal(IntegrationService.DefaultInt, result);
    }

    [Fact]
    public async Task TestGetPrimitiveStringList()
    {
        var serviceMock = new IntegrationService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<IIntegrationServiceProxyClient>()!;
        var result = await invoiceService.GetPrimitiveStringListAsync();
        Assert.Equal(new List<string> { IntegrationService.DefaultString }, result);
    }
    
    [Fact]
    public async Task TestGetInvoiceOpWithReturnTypeWrapped()
    {
        var serviceMock = new IntegrationService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<IIntegrationServiceProxyClient>()!;
        var result = await invoiceService.GetInvoiceOpWithReturnTypeWrappedAsync();
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