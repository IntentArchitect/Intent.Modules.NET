using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Pub = CqrsSpike.Publisher.Application;
using PubBehaviours = CqrsSpike.Publisher.Application.Common.Behaviours;
using Sub = CqrsSpike.Subscriber.Application;
using SubBehaviours = CqrsSpike.Subscriber.Application.Common.Behaviours;

namespace CqrsSpike.Scenarios;

/// <summary>
/// Pins the pipeline the Mediator source generator builds from the literal
/// <c>options.PipelineBehaviors</c> array. A dropped entry or a non-literal array still builds
/// clean and silently removes behaviours (P5); only the generated registrations show it.
/// </summary>
public class PipelineShapeChecks
{
    private static readonly IConfiguration Configuration = new ConfigurationBuilder().Build();

    [Fact]
    public void Publisher_CommandsGetAllSevenBehaviours_AndQueriesSkipUnitOfWork()
    {
        var services = new ServiceCollection();
        CqrsSpike.Publisher.Infrastructure.DependencyInjection.AddInfrastructure(services, Configuration);

        Assert.Equal(
            new[]
            {
                typeof(PubBehaviours.LoggingBehaviour<,>),
                typeof(PubBehaviours.UnhandledExceptionBehaviour<,>),
                typeof(PubBehaviours.PerformanceBehaviour<,>),
                typeof(PubBehaviours.AuthorizationBehaviour<,>),
                typeof(PubBehaviours.MessageBusPublishBehaviour<,>),
                typeof(PubBehaviours.ValidationBehaviour<,>),
                typeof(PubBehaviours.UnitOfWorkBehaviour<,>)
            },
            PipelineOf<Pub.Orders.CreateOrder.CreateOrderCommand, Guid>(services));

        Assert.Equal(
            new[]
            {
                typeof(PubBehaviours.LoggingBehaviour<,>),
                typeof(PubBehaviours.UnhandledExceptionBehaviour<,>),
                typeof(PubBehaviours.PerformanceBehaviour<,>),
                typeof(PubBehaviours.AuthorizationBehaviour<,>),
                typeof(PubBehaviours.MessageBusPublishBehaviour<,>),
                typeof(PubBehaviours.ValidationBehaviour<,>)
            },
            PipelineOf<Pub.Orders.GetOrders.GetOrdersQuery, List<Pub.Orders.OrderDto>>(services));
    }

    [Fact]
    public void Subscriber_CommandsGetAllSevenBehaviours_AndQueriesSkipUnitOfWork()
    {
        var services = new ServiceCollection();
        CqrsSpike.Subscriber.Infrastructure.DependencyInjection.AddInfrastructure(services, Configuration);

        Assert.Equal(
            new[]
            {
                typeof(SubBehaviours.LoggingBehaviour<,>),
                typeof(SubBehaviours.UnhandledExceptionBehaviour<,>),
                typeof(SubBehaviours.PerformanceBehaviour<,>),
                typeof(SubBehaviours.AuthorizationBehaviour<,>),
                typeof(SubBehaviours.MessageBusPublishBehaviour<,>),
                typeof(SubBehaviours.ValidationBehaviour<,>),
                typeof(SubBehaviours.UnitOfWorkBehaviour<,>)
            },
            PipelineOf<Sub.Shipments.CreateShipment.CreateShipmentCommand, Guid>(services));

        Assert.Equal(
            new[]
            {
                typeof(SubBehaviours.LoggingBehaviour<,>),
                typeof(SubBehaviours.UnhandledExceptionBehaviour<,>),
                typeof(SubBehaviours.PerformanceBehaviour<,>),
                typeof(SubBehaviours.AuthorizationBehaviour<,>),
                typeof(SubBehaviours.MessageBusPublishBehaviour<,>),
                typeof(SubBehaviours.ValidationBehaviour<,>)
            },
            PipelineOf<Sub.Shipments.GetShipments.GetShipmentsQuery, List<Sub.Shipments.ShipmentDto>>(services));
    }

    [Fact]
    public void Publisher_NotificationsUseTheStopOnFirstFailurePublisher()
    {
        var services = new ServiceCollection();
        CqrsSpike.Publisher.Infrastructure.DependencyInjection.AddInfrastructure(services, Configuration);

        Assert.IsType<CqrsSpike.Publisher.Infrastructure.Configuration.SequentialNotificationPublisher>(NotificationPublisherOf(services));
    }

    [Fact]
    public void Subscriber_NotificationsUseTheStopOnFirstFailurePublisher()
    {
        var services = new ServiceCollection();
        CqrsSpike.Subscriber.Infrastructure.DependencyInjection.AddInfrastructure(services, Configuration);

        Assert.IsType<CqrsSpike.Subscriber.Infrastructure.Configuration.SequentialNotificationPublisher>(NotificationPublisherOf(services));
    }

    [Fact]
    public void Publisher_HandlersAndBehavioursAreTransient()
    {
        var services = new ServiceCollection();
        CqrsSpike.Publisher.Infrastructure.DependencyInjection.AddInfrastructure(services, Configuration);

        AssertTransient<Pub.Orders.CreateOrder.CreateOrderCommand, Guid>(services);
    }

    [Fact]
    public void Subscriber_HandlersAndBehavioursAreTransient()
    {
        var services = new ServiceCollection();
        CqrsSpike.Subscriber.Infrastructure.DependencyInjection.AddInfrastructure(services, Configuration);

        AssertTransient<Sub.Shipments.CreateShipment.CreateShipmentCommand, Guid>(services);
    }

    // A fresh handler and behaviour per dispatch, as in MediatR and Wolverine (D23).
    private static void AssertTransient<TCommand, TResponse>(IServiceCollection services)
        where TCommand : ICommand<TResponse>
    {
        var registrations = services
            .Where(d => d.ServiceType == typeof(ICommandHandler<TCommand, TResponse>)
                || d.ServiceType == typeof(IPipelineBehavior<TCommand, TResponse>))
            .ToList();

        Assert.Equal(8, registrations.Count);
        Assert.All(registrations, d => Assert.Equal(ServiceLifetime.Transient, d.Lifetime));
    }

    // Mediator's built-in publishers both keep running handlers after a failure and wrap it in an
    // AggregateException, which the API exception mapping turns into a 500 (D22).
    private static INotificationPublisher NotificationPublisherOf(IServiceCollection services)
    {
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<INotificationPublisher>();
    }

    // The generator registers one closed IPipelineBehavior<TMessage, TResponse> per array entry,
    // in array order (first = outermost).
    private static Type[] PipelineOf<TMessage, TResponse>(IServiceCollection services)
        where TMessage : notnull, IMessage =>
        services
            .Where(d => d.ServiceType == typeof(IPipelineBehavior<TMessage, TResponse>))
            .Select(d => d.ImplementationType!.GetGenericTypeDefinition())
            .ToArray();
}
