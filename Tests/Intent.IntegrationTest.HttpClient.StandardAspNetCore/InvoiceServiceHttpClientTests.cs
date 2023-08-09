using AutoMapper;
using Intent.IntegrationTest.HttpClient.Common;
using Intent.IntegrationTest.HttpClient.StandardAspNetCore.TestUtils;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Standard.AspNetCore.TestApplication.Api.Configuration;
using Standard.AspNetCore.TestApplication.Api.Controllers;
using Standard.AspNetCore.TestApplication.Application;
using Standard.AspNetCore.TestApplication.Application.Implementation;
using Standard.AspNetCore.TestApplication.Application.IntegrationServices;
using Standard.AspNetCore.TestApplication.Application.IntegrationServices.Standard.AspNetCore.TestApplication.Services.Invoices;
using Standard.AspNetCore.TestApplication.Application.Interfaces;
using Standard.AspNetCore.TestApplication.Domain.Common.Interfaces;
using Standard.AspNetCore.TestApplication.Domain.Entities;
using Standard.AspNetCore.TestApplication.Domain.Repositories;
using Xunit;
using Xunit.Abstractions;

namespace Intent.IntegrationTest.HttpClient.StandardAspNetCore;

[Collection("InvoiceTests")]
public class InvoiceServiceHttpClientTests
{
    public InvoiceServiceHttpClientTests(ITestOutputHelper outputHelper)
    {
        OutputHelper = outputHelper;
    }

    private ITestOutputHelper OutputHelper { get; }
    
    private const int ApiPortNumber = 5012;
    
    [Fact]
    public async Task TestCreateInvoice()
    {
        var invoiceRepository = Substitute.For<IInvoiceRepository>();
        Invoice? entity = null;
        invoiceRepository.When(x => x.Add(Arg.Any<Invoice>())).Do(ci => entity = ci.Arg<Invoice>());
        invoiceRepository.UnitOfWork.When(x => x.SaveChangesAsync()).Do(ci => entity!.Id = Guid.NewGuid());
        
        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(invoiceRepository), typeof(IntegrationController).Assembly, ApiPortNumber);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<IInvoiceServiceProxy>()!;
        var result = await invoiceService.CreateInvoiceAsync(InvoiceCreateDto.Create("123"));
        Assert.Equal(entity!.Id, result);
    }

    [Fact]
    public async Task TestFindInvoiceById()
    {
        var id = new Guid("DDFEE1C9-766A-4C11-9C97-DFB7B820D9DE");
        const string number = "123";
        var invoiceRepository = Substitute.For<IInvoiceRepository>();
        invoiceRepository.FindByIdAsync(id, Arg.Any<CancellationToken>())!.Returns((ci) => Task.FromResult(new Invoice { Id = id, Number = number }));

        using var backendServer = await TestAspNetCoreHost.SetupApiServer(OutputHelper, GetDiServices(invoiceRepository), typeof(IntegrationController).Assembly, ApiPortNumber);
        var sp = TestIntegrationHttpClient.SetupServiceProvider();

        var invoiceService = sp.GetService<IInvoiceServiceProxy>()!;
        var result = await invoiceService.FindInvoiceByIdAsync(id);
        Assert.Equal(id, result.Id);
        Assert.Equal(number, result.Number);
    }
    
    private static Action<IServiceCollection> GetDiServices(IInvoiceRepository invoiceRepository)
    {
        var mapperConfiguration = new MapperConfiguration(config => config.AddMaps(typeof(InvoicesService)));
        var mapper = mapperConfiguration.CreateMapper();
        var serviceMock = new InvoicesService(invoiceRepository, mapper);
        return x => x.AddTransient<IInvoicesService>(_ => serviceMock)
            .AddTransient<IUnitOfWork>(_ => invoiceRepository.UnitOfWork)
            .AddTransient<IValidationService, ValidationService>()
            .ConfigureApiVersioning();
    }
}