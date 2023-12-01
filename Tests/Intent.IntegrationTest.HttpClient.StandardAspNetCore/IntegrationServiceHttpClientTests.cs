using IdentityServer4.Models;
using Intent.IntegrationTest.HttpClient.Common;
using Intent.IntegrationTest.HttpClient.StandardAspNetCore.TestUtils;
using Microsoft.Extensions.DependencyInjection;
using Standard.AspNetCore.TestApplication.Api.Configuration;
using Standard.AspNetCore.TestApplication.Api.Controllers;
using Standard.AspNetCore.TestApplication.Application;
using Standard.AspNetCore.TestApplication.Application.Common.Exceptions;
using Standard.AspNetCore.TestApplication.Application.Implementation;
using Standard.AspNetCore.TestApplication.Application.IntegrationServices.Services.Integration;
using Standard.AspNetCore.TestApplication.Domain.Common.Interfaces;
using Xunit;
using Xunit.Abstractions;
using IIntegrationServiceProxy = Standard.AspNetCore.TestApplication.Application.IntegrationServices.IIntegrationServiceProxy;
namespace Intent.IntegrationTest.HttpClient.StandardAspNetCore;

[Collection("IntegrationTests")]
public class IntegrationServiceHttpClientTests
{
    public IntegrationServiceHttpClientTests(ITestOutputHelper outputHelper)
    {
        OutputHelper = outputHelper;
    }

    private ITestOutputHelper OutputHelper { get; }

    private const int IdPortNumber = 5009;
    private const int ApiPortNumber = 5010;
    
    [Fact]
    public async Task TestQueryParamOperation()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper, IdPortNumber);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly, ApiPortNumber, IdPortNumber);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var integrationService = sp.GetService<IIntegrationServiceProxy>()!;
        await integrationService.QueryParamOpAsync(IntegrationService.DefaultString, IntegrationService.DefaultInt);
    }

    [Fact]
    public async Task TestBodyParamOperation()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper, IdPortNumber);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly, ApiPortNumber, IdPortNumber);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var integrationService = sp.GetService<IIntegrationServiceProxy>()!;
        await integrationService.BodyParamOpAsync(CustomDTO.Create(IntegrationService.ReferenceNumber));
    }

    [Fact]
    public async Task TestFormParamOperation()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper, IdPortNumber);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly, ApiPortNumber, IdPortNumber);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var integrationService = sp.GetService<IIntegrationServiceProxy>()!;
        await integrationService.FormParamOpAsync(IntegrationService.DefaultString, IntegrationService.DefaultInt);
    }

    [Fact]
    public async Task TestHeaderParamOperation()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper, IdPortNumber);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly, ApiPortNumber, IdPortNumber);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var integrationService = sp.GetService<IIntegrationServiceProxy>()!;
        await integrationService.HeaderParamOpAsync(IntegrationService.DefaultString);
    }

    [Fact]
    public async Task TestRouteParamOperation()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper, IdPortNumber);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly, ApiPortNumber, IdPortNumber);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var integrationService = sp.GetService<IIntegrationServiceProxy>()!;
        await integrationService.RouteParamOpAsync(IntegrationService.DefaultString);
    }

    [Fact]
    public async Task ThrowsExceptionOperation()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper, IdPortNumber);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly, ApiPortNumber, IdPortNumber);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var integrationService = sp.GetService<IIntegrationServiceProxy>()!;
        var exception = await Assert.ThrowsAsync<HttpClientRequestException>(async () => { await integrationService.ThrowsExceptionAsync(); });
        Assert.NotEmpty(exception.Message);
        Assert.Contains(IntegrationService.ExceptionMessage, exception.ResponseContent);
    }
    
    [Fact]
    public async Task TestGetPrimitiveGuid()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper, IdPortNumber);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly, ApiPortNumber, IdPortNumber);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var integrationService = sp.GetService<IIntegrationServiceProxy>()!;
        var result = await integrationService.GetWrappedPrimitiveGuidAsync();
        Assert.Equal(IntegrationService.DefaultGuid, result);
        result = await integrationService.GetPrimitiveGuidAsync();
        Assert.Equal(IntegrationService.DefaultGuid, result);
    }

    [Fact]
    public async Task TestGetPrimitiveString()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper, IdPortNumber);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly, ApiPortNumber, IdPortNumber);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var integrationService = sp.GetService<IIntegrationServiceProxy>()!;
        var result = await integrationService.GetWrappedPrimitiveStringAsync();
        Assert.Equal(IntegrationService.DefaultString, result);
        result = await integrationService.GetPrimitiveStringAsync();
        Assert.Equal(IntegrationService.DefaultString, result);
    }

    [Fact]
    public async Task TestGetPrimitiveInt()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper, IdPortNumber);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly, ApiPortNumber, IdPortNumber);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var integrationService = sp.GetService<IIntegrationServiceProxy>()!;
        var result = await integrationService.GetWrappedPrimitiveIntAsync();
        Assert.Equal(IntegrationService.DefaultInt, result);
        result = await integrationService.GetPrimitiveIntAsync();
        Assert.Equal(IntegrationService.DefaultInt, result);
    }

    [Fact]
    public async Task TestGetPrimitiveStringList()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper, IdPortNumber);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly, ApiPortNumber, IdPortNumber);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var integrationService = sp.GetService<IIntegrationServiceProxy>()!;
        var result = await integrationService.GetPrimitiveStringListAsync();
        Assert.Equal(new List<string> { IntegrationService.DefaultString }, result);
    }
    
    [Fact]
    public async Task TestGetInvoiceOpWithReturnTypeWrapped()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper, IdPortNumber);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly, ApiPortNumber, IdPortNumber);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var integrationService = sp.GetService<IIntegrationServiceProxy>()!;
        var result = await integrationService.GetInvoiceOpWithReturnTypeWrappedAsync();
        Assert.Equal(IntegrationService.ReferenceNumber, result.ReferenceNumber);
    }
    
    [Fact]
    public async Task TestGetItems()
    {
        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper, IdPortNumber);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(), typeof(IntegrationController).Assembly, ApiPortNumber, IdPortNumber);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var integrationService = sp.GetService<IIntegrationServiceProxy>()!;
        var result = await integrationService.GetItemsAsync(new List<string> { "1", "2", "5" });
        Assert.Equivalent(new[]
        {
            new CustomDTO { ReferenceNumber = "1" },
            new CustomDTO { ReferenceNumber = "2" },
            new CustomDTO { ReferenceNumber = "5" }
        }, result);
    }
    
    private static Action<IServiceCollection> GetDiServices()
    {
        var serviceMock = new IntegrationService();
        var mockUnitOfWork = new MockUnitOfWork();
        return x => x.AddTransient<Standard.AspNetCore.TestApplication.Application.Interfaces.IIntegrationService>(_ => serviceMock)
                .AddTransient<IUnitOfWork>(_ => mockUnitOfWork)
                .AddTransient<IValidationService, ValidationService>()
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