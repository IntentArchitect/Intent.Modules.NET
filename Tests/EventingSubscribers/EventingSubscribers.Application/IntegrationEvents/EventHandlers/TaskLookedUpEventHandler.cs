using EventingSubscribers.Application.Common.Eventing;
using EventingSubscribers.Domain.Common.Exceptions;
using EventingSubscribers.Domain.Repositories;
using EventingSubscribers.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace EventingSubscribers.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TaskLookedUpEventHandler : IIntegrationEventHandler<TaskLookedUpEvent>
    {
        private readonly IShipTaskRepository _shipTaskRepository;

        [IntentManaged(Mode.Merge)]
        public TaskLookedUpEventHandler(IShipTaskRepository shipTaskRepository)
        {
            _shipTaskRepository = shipTaskRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(TaskLookedUpEvent message, CancellationToken cancellationToken = default)
        {
            var queryTask = await _shipTaskRepository.FindAsync(x => x.Priority == message.Priority, cancellationToken);
            if (queryTask is null)
            {
                throw new NotFoundException($"Could not find ShipTask '{message.Priority}'");
            }
        }
    }
}