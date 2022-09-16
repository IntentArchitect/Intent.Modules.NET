using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IntegrationHttpClientTestSuite.IntentGenerated.Exceptions;
using IntegrationHttpClientTestSuite.IntentGenerated.HttpClients;
using IntegrationHttpClientTestSuite.IntentGenerated.TestUtils;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using Backend = IntegrationHttpClientTestSuite.IntentGenerated.ServiceContracts;
using BackendDto = IntegrationHttpClientTestSuite.IntentGenerated.ServiceContracts.Invoices;
using Client = IntegrationHttpClientTestSuite.IntentGenerated.ClientContracts;

namespace IntegrationHttpClientTestSuite.IntentGenerated;

public class IntegrationHttpClientTests
{
    public IntegrationHttpClientTests(ITestOutputHelper outputHelper)
    {
        OutputHelper = outputHelper;
    }

    private ITestOutputHelper OutputHelper { get; }

    [Fact]
    public async Task TestCreateOperation()
    {
        var serviceMock = new MockInvoiceService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, x => x.AddTransient<Backend.IInvoiceService>(_ => serviceMock));
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<Client.InvoiceProxy.IInvoiceProxyClient>()!;
        await invoiceService.Create(Client.InvoiceProxy.InvoiceCreateDTO.Create(MockInvoiceService.ReferenceNumber));
    }

    [Fact]
    public async Task TestDeleteOperation()
    {
        var serviceMock = new MockInvoiceService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, x => x.AddTransient<Backend.IInvoiceService>(_ => serviceMock));
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<Client.InvoiceProxy.IInvoiceProxyClient>()!;
        await invoiceService.Delete(MockInvoiceService.DefaultId);
    }

    [Fact]
    public async Task TestUpdateOperation()
    {
        var serviceMock = new MockInvoiceService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, x => x.AddTransient<Backend.IInvoiceService>(_ => serviceMock));
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<Client.InvoiceProxy.IInvoiceProxyClient>()!;
        await invoiceService.Update(MockInvoiceService.DefaultId, Client.InvoiceProxy.InvoiceUpdateDTO.Create(MockInvoiceService.ReferenceNumber));
    }

    [Fact]
    public async Task TestFindByIdOperation()
    {
        var serviceMock = new MockInvoiceService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, x => x.AddTransient<Backend.IInvoiceService>(_ => serviceMock));
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<Client.InvoiceProxy.IInvoiceProxyClient>()!;
        var result = await invoiceService.FindById(MockInvoiceService.DefaultId);
        Assert.Equal(MockInvoiceService.DefaultId, result.Id);
        Assert.Equal(MockInvoiceService.ReferenceNumber, result.Reference);
    }

    [Fact]
    public async Task TestFindAllOperation()
    {
        var serviceMock = new MockInvoiceService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, x => x.AddTransient<Backend.IInvoiceService>(_ => serviceMock));
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<Client.InvoiceProxy.IInvoiceProxyClient>()!;
        var result = await invoiceService.FindAll();
        Assert.NotEmpty(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task TestQueryParamOperation()
    {
        var serviceMock = new MockInvoiceService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, x => x.AddTransient<Backend.IInvoiceService>(_ => serviceMock));
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<Client.InvoiceProxy.IInvoiceProxyClient>()!;
        await invoiceService.QueryParamOp(MockInvoiceService.DefaultString, MockInvoiceService.DefaultInt);
    }

    [Fact]
    public async Task TestBodyParamOperation()
    {
        var serviceMock = new MockInvoiceService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, x => x.AddTransient<Backend.IInvoiceService>(_ => serviceMock));
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<Client.InvoiceProxy.IInvoiceProxyClient>()!;
        await invoiceService.BodyParamOp(Client.InvoiceProxy.InvoiceDTO.Create(MockInvoiceService.DefaultId, MockInvoiceService.ReferenceNumber));
    }

    [Fact]
    public async Task TestFormParamOperation()
    {
        var serviceMock = new MockInvoiceService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, x => x.AddTransient<Backend.IInvoiceService>(_ => serviceMock));
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<Client.InvoiceProxy.IInvoiceProxyClient>()!;
        await invoiceService.FormParamOp(MockInvoiceService.DefaultString, MockInvoiceService.DefaultInt);
    }

    [Fact]
    public async Task TestHeaderParamOperation()
    {
        var serviceMock = new MockInvoiceService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, x => x.AddTransient<Backend.IInvoiceService>(_ => serviceMock));
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<Client.InvoiceProxy.IInvoiceProxyClient>()!;
        await invoiceService.HeaderParamOp(MockInvoiceService.DefaultString);
    }

    [Fact]
    public async Task TestRouteParamOperation()
    {
        var serviceMock = new MockInvoiceService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, x => x.AddTransient<Backend.IInvoiceService>(_ => serviceMock));
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<Client.InvoiceProxy.IInvoiceProxyClient>()!;
        await invoiceService.RouteParamOp(MockInvoiceService.DefaultString);
    }

    [Fact]
    public async Task ThrowsExceptionOperation()
    {
        var serviceMock = new MockInvoiceService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, x => x.AddTransient<Backend.IInvoiceService>(_ => serviceMock));
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<Client.InvoiceProxy.IInvoiceProxyClient>()!;
        var exception = await Assert.ThrowsAsync<HttpClientRequestException>(async () => { await invoiceService.ThrowsException(); });
        Assert.NotEmpty(exception.Message);
        Assert.Contains(MockInvoiceService.ExceptionMessage, exception.ResponseContent);
    }

    [Fact]
    public async Task TestGetPrimitiveGuid()
    {
        var serviceMock = new MockInvoiceService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, x => x.AddTransient<Backend.IInvoiceService>(_ => serviceMock));
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<Client.InvoiceProxy.IInvoiceProxyClient>()!;
        var result = await invoiceService.GetWrappedPrimitiveGuid();
        Assert.Equal(MockInvoiceService.DefaultGuid, result);
        result = await invoiceService.GetPrimitiveGuid();
        Assert.Equal(MockInvoiceService.DefaultGuid, result);
    }

    [Fact]
    public async Task TestGetPrimitiveString()
    {
        var serviceMock = new MockInvoiceService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, x => x.AddTransient<Backend.IInvoiceService>(_ => serviceMock));
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<Client.InvoiceProxy.IInvoiceProxyClient>()!;
        var result = await invoiceService.GetWrappedPrimitiveString();
        Assert.Equal(MockInvoiceService.DefaultString, result);
        result = await invoiceService.GetPrimitiveString();
        Assert.Equal(MockInvoiceService.DefaultString, result);
    }

    [Fact]
    public async Task TestGetPrimitiveInt()
    {
        var serviceMock = new MockInvoiceService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, x => x.AddTransient<Backend.IInvoiceService>(_ => serviceMock));
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<Client.InvoiceProxy.IInvoiceProxyClient>()!;
        var result = await invoiceService.GetWrappedPrimitiveInt();
        Assert.Equal(MockInvoiceService.DefaultInt, result);
        result = await invoiceService.GetPrimitiveInt();
        Assert.Equal(MockInvoiceService.DefaultInt, result);
    }

    [Fact]
    public async Task TestGetPrimitiveStringList()
    {
        var serviceMock = new MockInvoiceService();

        using var identityServer = await TestIdentityHost.SetupIdentityServer(OutputHelper);
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, x => x.AddTransient<Backend.IInvoiceService>(_ => serviceMock));
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<Client.InvoiceProxy.IInvoiceProxyClient>()!;
        var result = await invoiceService.GetPrimitiveStringList();
        Assert.Equal(new List<string> { MockInvoiceService.DefaultString }, result);
    }

    public class MockInvoiceService : Backend.IInvoiceService
    {
        public const string ReferenceNumber = "refnumber_1234";
        public static readonly Guid DefaultId = Guid.Parse("762602ee-d55e-4211-b9b3-e273c3214443");
        public const string DefaultString = "string value";
        public const int DefaultInt = 55;
        public const string ExceptionMessage = "Some exception message";
        public static readonly Guid DefaultGuid = Guid.Parse("b7698947-5237-4686-9571-442335426771");

        public void Dispose()
        {
        }

        public async Task Create(BackendDto.InvoiceCreateDTO dto)
        {
            Assert.Equal(ReferenceNumber, dto.Reference);
        }

        public async Task<BackendDto.InvoiceDTO> FindById(Guid id)
        {
            Assert.Equal(DefaultId, id);
            return BackendDto.InvoiceDTO.Create(id, ReferenceNumber);
        }

        public async Task<List<BackendDto.InvoiceDTO>> FindAll()
        {
            return new List<BackendDto.InvoiceDTO>
            {
                BackendDto.InvoiceDTO.Create(DefaultId, ReferenceNumber)
            };
        }

        public async Task Update(Guid id, BackendDto.InvoiceUpdateDTO dto)
        {
            Assert.Equal(DefaultId, id);
            Assert.Equal(ReferenceNumber, dto.Reference);
        }

        public async Task Delete(Guid id)
        {
            Assert.Equal(DefaultId, id);
        }

        public async Task<BackendDto.InvoiceDTO> QueryParamOp(string param1, int param2)
        {
            Assert.Equal(DefaultString, param1);
            Assert.Equal(DefaultInt, param2);
            return BackendDto.InvoiceDTO.Create(DefaultId, ReferenceNumber);
        }

        public async Task HeaderParamOp(string param1)
        {
            Assert.Equal(DefaultString, param1);
        }

        public async Task FormParamOp(string param1, int param2)
        {
            Assert.Equal(DefaultString, param1);
            Assert.Equal(DefaultInt, param2);
        }

        public async Task RouteParamOp(string param1)
        {
            Assert.Equal(DefaultString, param1);
        }

        public async Task BodyParamOp(BackendDto.InvoiceDTO param1)
        {
            Assert.Equal(DefaultId, param1.Id);
            Assert.Equal(ReferenceNumber, param1.Reference);
        }

        public async Task ThrowsException()
        {
            throw new Exception(ExceptionMessage);
        }

        public Task<Guid> GetWrappedPrimitiveGuid()
        {
            return Task.FromResult(DefaultGuid);
        }

        public Task<string> GetWrappedPrimitiveString()
        {
            return Task.FromResult(DefaultString);
        }

        public Task<int> GetWrappedPrimitiveInt()
        {
            return Task.FromResult(DefaultInt);
        }

        public Task<Guid> GetPrimitiveGuid()
        {
            return Task.FromResult(DefaultGuid);
        }

        public Task<string> GetPrimitiveString()
        {
            return Task.FromResult(DefaultString);
        }

        public Task<int> GetPrimitiveInt()
        {
            return Task.FromResult(DefaultInt);
        }

        public Task<List<string>> GetPrimitiveStringList()
        {
            return Task.FromResult(new List<string> { DefaultString });
        }
    }
}