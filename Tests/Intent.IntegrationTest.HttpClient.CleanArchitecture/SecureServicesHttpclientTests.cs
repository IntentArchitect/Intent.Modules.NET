﻿using System.IdentityModel.Tokens.Jwt;
using CleanArchitecture.TestApplication.Api.Configuration;
using CleanArchitecture.TestApplication.Api.Controllers;
using CleanArchitecture.TestApplication.Api.Services;
using CleanArchitecture.TestApplication.Application;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using CleanArchitecture.TestApplication.Application.IntegrationServices.SecureServicesService;
using CleanArchitecture.TestApplication.Application.IntegrationServices.TestUnversionedProxy;
using CleanArchitecture.TestApplication.Application.Unversioned.Test;
using CleanArchitecture.TestApplication.Domain.Common.Interfaces;
using Intent.IntegrationTest.HttpClient.CleanArchitecture.TestUtils;
using Intent.IntegrationTest.HttpClient.Common;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using TestCommand = CleanArchitecture.TestApplication.Application.IntegrationServices.TestUnversionedProxy.TestCommand;

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

        var service = sp.GetService<ISecureServicesClient>()!;
        await service.SecureAsync(new SecureCommand() { Message = "123" });
    }
    
    private static Action<IServiceCollection> GetDiServices()
    {
        var mockUnitOfWork = new MockUnitOfWork();
        return x => x.AddApplication()
            .AddTransient<IUnitOfWork>(_ => mockUnitOfWork)
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
}