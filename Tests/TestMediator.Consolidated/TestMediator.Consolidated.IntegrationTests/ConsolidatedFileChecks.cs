using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;
using TestMediator.Consolidated.Application.Customers;
using TestMediator.Consolidated.Domain.Common.Exceptions;
using TestMediator.Consolidated.Infrastructure.Persistence;

namespace TestMediator.Consolidated.IntegrationTests;

/// <summary>
/// With "Consolidate Command/Query associated files into single file" on, each command and query file also holds its
/// handler and validator. These checks prove the Mediator source generator and the validation pipeline still find them
/// there, in the same namespace as the request instead of a per-request sub-namespace.
/// </summary>
public class ConsolidatedFileChecks
{
    [Fact]
    public void HandlersAndValidators_ShareTheRequestsNamespace()
    {
        Type[] types =
        [
            typeof(CreateCustomerCommandHandler), typeof(CreateCustomerCommandValidator),
            typeof(DeleteCustomerCommandHandler), typeof(DeleteCustomerCommandValidator),
            typeof(GetCustomerByIdQueryHandler), typeof(GetCustomerByIdQueryValidator),
            typeof(GetCustomersQueryHandler), typeof(GetCustomersQueryValidator),
        ];

        Assert.All(types, t => Assert.Equal(typeof(CreateCustomerCommand).Namespace, t.Namespace));
    }

    [Fact]
    public async Task Commands_And_Queries_RunThroughTheirConsolidatedHandlers()
    {
        await using var provider = InProcessApp.Build();

        var id = await SendAsync(provider, new CreateCustomerCommand("Ada Lovelace", "ada@example.com"));

        var customer = await SendAsync(provider, new GetCustomerByIdQuery(id));
        Assert.Equal("Ada Lovelace", customer.Name);
        Assert.Equal("ada@example.com", customer.Email);
        Assert.Single(await SendAsync(provider, new GetCustomersQuery()));

        await SendAsync(provider, new DeleteCustomerCommand(id));
        Assert.Empty(await SendAsync(provider, new GetCustomersQuery()));
        await Assert.ThrowsAsync<NotFoundException>(() => SendAsync(provider, new GetCustomerByIdQuery(id)));
    }

    [Fact]
    public async Task CreateCustomer_IsRejectedByTheConsolidatedValidator_WhenTheEmailIsInvalid()
    {
        await using var provider = InProcessApp.Build();

        await Assert.ThrowsAsync<ValidationException>(() => SendAsync(provider, new CreateCustomerCommand("Ada Lovelace", "not-an-email")));

        await using var scope = provider.CreateAsyncScope();
        Assert.False(await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Customers.AnyAsync());
    }

    private static async Task<TResponse> SendAsync<TResponse>(IServiceProvider provider, ICommand<TResponse> command)
    {
        await using var scope = provider.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<ISender>().Send(command);
    }

    private static async Task SendAsync(IServiceProvider provider, ICommand command)
    {
        await using var scope = provider.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<ISender>().Send(command);
    }

    private static async Task<TResponse> SendAsync<TResponse>(IServiceProvider provider, IQuery<TResponse> query)
    {
        await using var scope = provider.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<ISender>().Send(query);
    }
}

/// <summary>
/// The app's real composition (AddApplication + AddInfrastructure) in a plain service provider, on its own EF Core
/// InMemory database per test.
/// </summary>
internal static class InProcessApp
{
    public static ServiceProvider Build()
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(configuration);
        Application.DependencyInjection.AddApplication(services, configuration);
        Infrastructure.DependencyInjection.AddInfrastructure(services, configuration);

        // EF Core 9+ accumulates option configurations per context, so the app's own one must go too.
        var database = $"consolidated-{Guid.NewGuid():N}";
        services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
        services.RemoveAll<IDbContextOptionsConfiguration<ApplicationDbContext>>();
        services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(database).UseLazyLoadingProxies());

        return services.BuildServiceProvider();
    }
}
