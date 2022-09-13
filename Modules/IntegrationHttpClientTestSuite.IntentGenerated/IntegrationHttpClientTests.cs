using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IntegrationHttpClientTestSuite.IntentGenerated.ClientContracts.Invoices;
using IntegrationHttpClientTestSuite.IntentGenerated.TestUtils;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;
using Backend = IntegrationHttpClientTestSuite.IntentGenerated.ServiceContracts;
using BackendDto = IntegrationHttpClientTestSuite.IntentGenerated.ServiceContracts.Invoices;
using Client = IntegrationHttpClientTestSuite.IntentGenerated.ClientContracts;

namespace IntegrationHttpClientTestSuite.IntentGenerated;

public class IntegrationHttpClientTests
{
    [Fact]
    public async Task TestCreateOperation()
    {
        const string refNum = "abc";

        var serviceMock = CreateMock<Backend.IInvoiceService, BackendDto.InvoiceCreateDTO>(
            (service, request) => service.Create(request),
            r => Assert.Equal(refNum, r.Reference));

        using var identityServer = await TestIdentityHost.SetupIdentityServer();
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(x => x.AddTransient(_ => serviceMock));
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<Client.IInvoiceService>()!;
        await invoiceService.Create(InvoiceCreateDTO.Create(refNum));
    }

    [Fact]
    public async Task TestDeleteOperation()
    {
        var id = Guid.NewGuid();

        var serviceMock = CreateMock<Backend.IInvoiceService, Guid>(
            (service, request) => service.Delete(request),
            r => Assert.Equal(id, r));

        using var identityServer = await TestIdentityHost.SetupIdentityServer();
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(x => x.AddTransient(_ => serviceMock));
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<Client.IInvoiceService>()!;
        await invoiceService.Delete(id);
    }

    [Fact]
    public async Task TestUpdateOperation()
    {
        var id = Guid.NewGuid();
        const string refNum = "abc";

        var serviceMock = CreateMock<Backend.IInvoiceService, Guid, BackendDto.InvoiceUpdateDTO>(
            (service, r1, r2) => service.Update(r1, r2),
            (r1, r2) =>
            {
                Assert.Equal(id, r1);
                Assert.Equal(refNum, r2.Reference);
            });

        using var identityServer = await TestIdentityHost.SetupIdentityServer();
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(x => x.AddTransient(_ => serviceMock));
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<Client.IInvoiceService>()!;
        await invoiceService.Update(id, InvoiceUpdateDTO.Create(refNum));
    }

    [Fact]
    public async Task TestFindByIdOperation()
    {
        var id = Guid.NewGuid();
        const string refNo = "abc";

        var serviceMock = Substitute.For<Backend.IInvoiceService>();
        serviceMock.FindById(Arg.Any<Guid>()).Returns(BackendDto.InvoiceDTO.Create(id, refNo));

        using var identityServer = await TestIdentityHost.SetupIdentityServer();
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(x => x.AddTransient(_ => serviceMock));
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<Client.IInvoiceService>()!;
        var result = await invoiceService.FindById(id);
        Assert.Equal(id, result.Id);
        Assert.Equal(refNo, result.Reference);
    }

    [Fact]
    public async Task TestFindAllOperation()
    {
        var serviceMock = Substitute.For<Backend.IInvoiceService>();
        serviceMock.FindAll()
            .Returns(new List<BackendDto.InvoiceDTO> { BackendDto.InvoiceDTO.Create(Guid.NewGuid(), "item1") });

        using var identityServer = await TestIdentityHost.SetupIdentityServer();
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(x => x.AddTransient(_ => serviceMock));
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<Client.IInvoiceService>()!;
        var result = await invoiceService.FindAll();
        Assert.NotEmpty(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task TestQueryParamOperation()
    {
        const string param1 = "value";
        const int param2 = 55;

        var serviceMock = CreateMock<Backend.IInvoiceService, string, int>(
            (service, r1, r2) => service.QueryParamOp(r1, r2),
            (r1, r2) =>
            {
                Assert.Equal(param1, r1);
                Assert.Equal(param2, r2);
            });

        using var identityServer = await TestIdentityHost.SetupIdentityServer();
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(x => x.AddTransient(_ => serviceMock));
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<Client.IInvoiceService>()!;
        await invoiceService.QueryParamOp(param1, param2);
    }

    [Fact]
    public async Task TestBodyParamOperation()
    {
        var param1 = Guid.NewGuid();
        const string param2 = "abc";

        var serviceMock = CreateMock<Backend.IInvoiceService, BackendDto.InvoiceDTO>(
            (service, r) => service.BodyParamOp(r),
            (r) =>
            {
                Assert.Equal(param1, r.Id);
                Assert.Equal(param2, r.Reference);
            });

        using var identityServer = await TestIdentityHost.SetupIdentityServer();
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(x => x.AddTransient(_ => serviceMock));
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<Client.IInvoiceService>()!;
        await invoiceService.BodyParamOp(InvoiceDTO.Create(param1, param2));
    }
    
    [Fact]
    public async Task TestFormParamOperation()
    {
        const string param1 = "value";
        const int param2 = 55;

        var serviceMock = CreateMock<Backend.IInvoiceService, string, int>(
            (service, r1, r2) => service.FormParamOp(r1, r2),
            (r1, r2) =>
            {
                Assert.Equal(param1, r1);
                Assert.Equal(param2, r2);
            });

        using var identityServer = await TestIdentityHost.SetupIdentityServer();
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(x => x.AddTransient(_ => serviceMock));
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<Client.IInvoiceService>()!;
        await invoiceService.FormParamOp(param1, param2);
    }
    
    [Fact]
    public async Task TestHeaderParamOperation()
    {
        const string param1 = "value";

        var serviceMock = CreateMock<Backend.IInvoiceService, string>(
            (service, r1) => service.HeaderParamOp(r1),
            (r1) =>
            {
                Assert.Equal(param1, r1);
            });

        using var identityServer = await TestIdentityHost.SetupIdentityServer();
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(x => x.AddTransient(_ => serviceMock));
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<Client.IInvoiceService>()!;
        await invoiceService.HeaderParamOp(param1);
    }
    
    [Fact]
    public async Task TestRouteParamOperation()
    {
        const string param1 = "value";

        var serviceMock = CreateMock<Backend.IInvoiceService, string>(
            (service, r1) => service.RouteParamOp(r1),
            (r1) =>
            {
                Assert.Equal(param1, r1);
            });

        using var identityServer = await TestIdentityHost.SetupIdentityServer();
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(x => x.AddTransient(_ => serviceMock));
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<Client.IInvoiceService>()!;
        await invoiceService.RouteParamOp(param1);
    }
    
    
    private static TService CreateMock<TService, TRequest>(
        Func<TService, TRequest, Task> serviceOperationTestFunc,
        Action<TRequest> verifyServiceOperationAction)
        where TService : class
    {
        var mock = Substitute.For<TService>();

        mock.WhenForAnyArgs(s => serviceOperationTestFunc(s, Arg.Any<TRequest>()))
            .Do(x => verifyServiceOperationAction(x.ArgAt<TRequest>(0)));
        return mock;
    }

    private static TService CreateMock<TService, TRequest1, TRequest2>(
        Func<TService, TRequest1, TRequest2, Task> serviceOperationTestFunc,
        Action<TRequest1, TRequest2> verifyServiceOperationAction)
        where TService : class
    {
        var mock = Substitute.For<TService>();

        mock.WhenForAnyArgs(s => serviceOperationTestFunc(s, Arg.Any<TRequest1>(), Arg.Any<TRequest2>()))
            .Do(x => verifyServiceOperationAction(x.ArgAt<TRequest1>(0), x.ArgAt<TRequest2>(1)));
        return mock;
    }
}