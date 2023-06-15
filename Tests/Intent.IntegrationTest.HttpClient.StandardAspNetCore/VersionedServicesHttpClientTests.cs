﻿using Intent.IntegrationTest.HttpClient.Common;
using Intent.IntegrationTest.HttpClient.StandardAspNetCore.TestUtils;
using Microsoft.Extensions.DependencyInjection;
using Standard.AspNetCore.TestApplication.Api.Controllers;
using Standard.AspNetCore.TestApplication.Application.Implementation;
using Standard.AspNetCore.TestApplication.Application.IntegrationServices.IntegrationServiceProxy;
using Standard.AspNetCore.TestApplication.Application.IntegrationServices.MultiVersionServiceProxy;
using Standard.AspNetCore.TestApplication.Application.IntegrationServices.VersionOneServiceProxy;
using Standard.AspNetCore.TestApplication.Application.Interfaces;
using Standard.AspNetCore.TestApplication.Domain.Common.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace Intent.IntegrationTest.HttpClient.StandardAspNetCore;

[Collection("VersionedTests")]
public class VersionedServicesHttpClientTests
{
    public VersionedServicesHttpClientTests(ITestOutputHelper outputHelper)
    {
        OutputHelper = outputHelper;
    }

    private const int ApiPortNumber = 5014;
    
    private ITestOutputHelper OutputHelper { get; }
    
    [Fact]
    public async Task Test_VersionOne_OperationForVersionOne()
    {
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetVersionOneDiServices(), typeof(VersionOneController).Assembly, ApiPortNumber);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var service = sp.GetService<IVersionOneServiceProxyClient>()!;
        await service.OperationForVersionOneAsync(VersionOneService.ReferenceNumber);
    }
    
    [Fact]
    public async Task Test_Multi_OperationForVersionOne()
    {
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetMultiDiServices(), typeof(MultiVersionController).Assembly, ApiPortNumber);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var service = sp.GetService<IMultiVersionServiceProxyClient>()!;
        await service.OperationForVersionOneAsync();
    }
    
    [Fact]
    public async Task Test_Multi_OperationForVersionTwo()
    {
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetMultiDiServices(), typeof(MultiVersionController).Assembly, ApiPortNumber);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var service = sp.GetService<IMultiVersionServiceProxyClient>()!;
        await service.OperationForVersionTwoAsync();
    }
    
    private static Action<IServiceCollection> GetVersionOneDiServices()
    {
        var serviceMock = new VersionOneService();
        var mockUnitOfWork = new MockUnitOfWork();
        return x => x.AddTransient<IVersionOneService>(_ => serviceMock)
            .AddTransient<IUnitOfWork>(_ => mockUnitOfWork);
    }
    
    private static Action<IServiceCollection> GetMultiDiServices()
    {
        var serviceMock = new MultiVersionService();
        var mockUnitOfWork = new MockUnitOfWork();
        return x => x.AddTransient<IMultiVersionService>(_ => serviceMock)
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