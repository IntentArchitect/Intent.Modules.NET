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
    public class TaskAssignedEventHandler : IIntegrationEventHandler<TaskAssignedEvent>
    {
        private readonly IShipTaskRepository _shipTaskRepository;

        [IntentManaged(Mode.Merge)]
        public TaskAssignedEventHandler(IShipTaskRepository shipTaskRepository)
        {
            _shipTaskRepository = shipTaskRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(TaskAssignedEvent message, CancellationToken cancellationToken = default)
        {
            var updateTask = await _shipTaskRepository.FindByIdAsync(message.Id, cancellationToken);
            if (updateTask is null)
            {
                throw new NotFoundException($"Could not find ShipTask '{message.Id}'");
            }

            updateTask.Priority = message.Priority;
        }
    }
}