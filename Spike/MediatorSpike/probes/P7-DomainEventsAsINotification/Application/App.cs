using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Probe.Domain;

namespace Probe.Application.Common.Interfaces
{
    // Intent-style marker interfaces (same simple names as Mediator.ICommand / Mediator.IQuery)
    public interface ICommand { }
    public interface IQuery { }
}

namespace Probe.Application
{
    public class Log { public static List<string> Lines = new(); }
    public class FakeDbContext { public Guid InstanceId { get; } = Guid.NewGuid(); public int Saves; }

    public record CreateOrderCommand(string Name) : IRequest<Guid>, Probe.Application.Common.Interfaces.ICommand;
    public class CreateOrderCommandHandler(FakeDbContext db) : IRequestHandler<CreateOrderCommand, Guid>
    {
        public async ValueTask<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            Log.Lines.Add($"handler CreateOrder db={db.InstanceId}");
            await Task.Yield();
            return Guid.NewGuid();
        }
    }

    public record DeleteOrderCommand(Guid Id) : IRequest, Probe.Application.Common.Interfaces.ICommand;
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
    {
        public ValueTask<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            Log.Lines.Add("handler DeleteOrder");
            return ValueTask.FromResult(Unit.Value);
        }
    }

    public record GetOrderQuery(Guid Id) : IRequest<string>, Probe.Application.Common.Interfaces.IQuery;
    public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, string>
    {
        public ValueTask<string> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            Log.Lines.Add("handler GetOrder");
            return ValueTask.FromResult("order");
        }
    }

    // Variant A: domain events implement INotification directly (Domain references Mediator.Abstractions)
    public class OrderCreatedHandler : INotificationHandler<OrderCreatedDomainEvent>
    {
        public ValueTask Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            Log.Lines.Add($"notification handler OrderCreated {notification.Id}");
            return default;
        }
    }
    public class OrderCreatedHandler2 : INotificationHandler<OrderCreatedDomainEvent>
    {
        public ValueTask Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            Log.Lines.Add($"notification handler2 OrderCreated {notification.Id}");
            return default;
        }
    }

    public class LoggingBehaviour<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse> where TMessage : IMessage
    {
        public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
        {
            Log.Lines.Add($"logging> {typeof(TMessage).Name}");
            var r = await next(message, cancellationToken);
            Log.Lines.Add($"logging< {typeof(TMessage).Name}");
            return r;
        }
    }
    public class UnitOfWorkBehaviour<TMessage, TResponse>(FakeDbContext db) : IPipelineBehavior<TMessage, TResponse>
        where TMessage : IMessage, Probe.Application.Common.Interfaces.ICommand
    {
        public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
        {
            Log.Lines.Add($"uow> {typeof(TMessage).Name} db={db.InstanceId}");
            var r = await next(message, cancellationToken);
            db.Saves++;
            Log.Lines.Add($"uow< {typeof(TMessage).Name}");
            return r;
        }
    }

    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<FakeDbContext>();
            services.AddMediator((MediatorOptions options) =>
            {
                options.ServiceLifetime = ServiceLifetime.Scoped;
                options.Assemblies = [typeof(DependencyInjection), typeof(DomainEvent)];
                options.PipelineBehaviors = [typeof(LoggingBehaviour<,>), typeof(UnitOfWorkBehaviour<,>)];
            });
            return services;
        }
    }
}
