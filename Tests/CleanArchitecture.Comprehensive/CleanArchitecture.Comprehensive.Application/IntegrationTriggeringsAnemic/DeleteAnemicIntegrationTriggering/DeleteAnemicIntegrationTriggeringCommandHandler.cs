using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.Common.Eventing;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.ConventionBasedEventPublishing;
using CleanArchitecture.Comprehensive.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.IntegrationTriggeringsAnemic.DeleteAnemicIntegrationTriggering
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteAnemicIntegrationTriggeringCommandHandler : IRequestHandler<DeleteAnemicIntegrationTriggeringCommand>
    {
        private readonly IIntegrationTriggeringRepository _integrationTriggeringRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public DeleteAnemicIntegrationTriggeringCommandHandler(IIntegrationTriggeringRepository integrationTriggeringRepository,
            IEventBus eventBus)
        {
            _integrationTriggeringRepository = integrationTriggeringRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteAnemicIntegrationTriggeringCommand request, CancellationToken cancellationToken)
        {
            var existingIntegrationTriggering = await _integrationTriggeringRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingIntegrationTriggering is null)
            {
                throw new NotFoundException($"Could not find IntegrationTriggering '{request.Id}'");
            }

            _integrationTriggeringRepository.Remove(existingIntegrationTriggering);
            _eventBus.Publish(new IntegrationTriggeringDeletedEvent
            {
                Id = request.Id
            });

        }
    }
}