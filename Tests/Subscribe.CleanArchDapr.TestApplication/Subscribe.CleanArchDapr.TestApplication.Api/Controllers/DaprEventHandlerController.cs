using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Dapr;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Publish.CleanArchDapr.TestApplication.Eventing.Messages;
using Subscribe.CleanArchDapr.TestApplication.Application.Common.Eventing;
using Subscribe.CleanArchDapr.TestApplication.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.Pubsub.DaprEventHandlerController", Version = "1.0")]

namespace Subscribe.CleanArchDapr.TestApplication.Api.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class DaprEventHandlerController : ControllerBase
    {
        private readonly ISender _mediatr;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventBus _eventBus;
        private readonly IDaprStateStoreUnitOfWork _daprStateStoreUnitOfWork;
        private readonly IUnitOfWork _unitOfWork;

        public DaprEventHandlerController(ISender mediatr,
            IServiceProvider serviceProvider,
            IEventBus eventBus,
            IDaprStateStoreUnitOfWork daprStateStoreUnitOfWork,
            IUnitOfWork unitOfWork)
        {
            _mediatr = mediatr;
            _serviceProvider = serviceProvider;
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _daprStateStoreUnitOfWork = daprStateStoreUnitOfWork ?? throw new ArgumentNullException(nameof(daprStateStoreUnitOfWork));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [HttpPost]
        [Topic(CustomerCreatedEvent.PubsubName, CustomerCreatedEvent.TopicName)]
        public async Task HandleCustomerCreatedEvent(CustomerCreatedEvent @event, CancellationToken cancellationToken)
        {
            var handler = _serviceProvider.GetRequiredService<IIntegrationEventHandler<CustomerCreatedEvent>>();

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await handler.HandleAsync(@event, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }

            await _daprStateStoreUnitOfWork.SaveChangesAsync(cancellationToken);
            await _eventBus.FlushAllAsync(cancellationToken);
        }

        [HttpPost]
        [Topic(OrderCreatedEvent.PubsubName, OrderCreatedEvent.TopicName)]
        public async Task HandleOrderCreatedEvent(OrderCreatedEvent @event, CancellationToken cancellationToken)
        {
            var handler = _serviceProvider.GetRequiredService<IIntegrationEventHandler<OrderCreatedEvent>>();

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await handler.HandleAsync(@event, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }

            await _daprStateStoreUnitOfWork.SaveChangesAsync(cancellationToken);
            await _eventBus.FlushAllAsync(cancellationToken);
        }
    }
}