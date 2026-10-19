using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;
using TestMediator.NoEventing.Application.Common.Behaviours;
using TestMediator.NoEventing.Application.Products.ChangeProductPrice;
using TestMediator.NoEventing.Application.Products.CreateProduct;
using TestMediator.NoEventing.Application.Products.GetProductPriceHistory;
using TestMediator.NoEventing.Infrastructure.Persistence;

namespace TestMediator.NoEventing.IntegrationTests;

/// <summary>
/// An application with the Mediator modules and domain events but no eventing module has no message bus interface, so
/// the message-bus parts of the Mediator stack must not be generated: no MessageBusFlushOwner, no
/// MessageBusPublishBehaviour, no flush step in the pipeline. The same gate MediatR.Behaviours and Wolverine use.
/// </summary>
public class NoMessageBusChecks
{
    private static readonly string[] MessageBusTypeNames = ["IMessageBus", "IEventBus", "MessageBusFlushOwner", "MessageBusPublishBehaviour`2"];

    [Fact]
    public void GeneratedAssemblies_ContainNoMessageBusTypes()
    {
        var assemblies = new[]
        {
            typeof(Application.DependencyInjection).Assembly,
            typeof(Infrastructure.DependencyInjection).Assembly,
            typeof(Domain.Common.DomainEvent).Assembly,
        };

        var found = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => MessageBusTypeNames.Contains(t.Name))
            .Select(t => t.FullName)
            .ToList();

        Assert.Empty(found);
    }

    [Fact]
    public async Task Pipeline_HasNoFlushStep_OnlyValidationThenUnitOfWork()
    {
        await using var provider = InProcessApp.Build();
        await using var scope = provider.CreateAsyncScope();

        var pipeline = scope.ServiceProvider
            .GetServices<IPipelineBehavior<CreateProductCommand, Guid>>()
            .Select(b => b.GetType().GetGenericTypeDefinition())
            .ToList();

        Assert.Equal([typeof(ValidationBehaviour<,>), typeof(UnitOfWorkBehaviour<,>)], pipeline);
    }

    [Fact]
    public async Task ChangingAPrice_RecordsHistoryThroughTheDomainEventHandler()
    {
        await using var provider = InProcessApp.Build();

        var productId = await SendAsync(provider, new CreateProductCommand("Espresso grinder", 249.00m));
        await SendAsync(provider, new ChangeProductPriceCommand(productId, 229.00m));
        // An unchanged price raises no event, so it adds no history.
        await SendAsync(provider, new ChangeProductPriceCommand(productId, 229.00m));

        var history = await SendAsync(provider, new GetProductPriceHistoryQuery(productId));

        var entry = Assert.Single(history);
        Assert.Equal(249.00m, entry.OldPrice);
        Assert.Equal(229.00m, entry.NewPrice);
    }

    [Fact]
    public async Task CreateProduct_IsRejectedByTheValidationBehaviour_WhenTheNameIsEmpty()
    {
        await using var provider = InProcessApp.Build();

        await Assert.ThrowsAsync<ValidationException>(() => SendAsync(provider, new CreateProductCommand("", 10m)));

        await using var scope = provider.CreateAsyncScope();
        Assert.False(await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Products.AnyAsync());
    }

    private static async Task<TResponse> SendAsync<TResponse>(IServiceProvider provider, ICommand<TResponse> command)
    {
        await using var scope = provider.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<ISender>().Send(command);
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
        var database = $"no-eventing-{Guid.NewGuid():N}";
        services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
        services.RemoveAll<IDbContextOptionsConfiguration<ApplicationDbContext>>();
        services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(database).UseLazyLoadingProxies());

        return services.BuildServiceProvider();
    }
}
