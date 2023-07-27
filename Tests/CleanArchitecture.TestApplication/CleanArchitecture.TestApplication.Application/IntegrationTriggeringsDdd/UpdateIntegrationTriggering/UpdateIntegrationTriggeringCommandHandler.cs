using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Application.Common.Eventing;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Repositories.ConventionBasedEventPublishing;
using CleanArchitecture.TestApplication.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.IntegrationTriggeringsDdd.UpdateIntegrationTriggering
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateIntegrationTriggeringCommandHandler : IRequestHandler<UpdateIntegrationTriggeringCommand>
    {
        private readonly IIntegrationTriggeringRepository _integrationTriggeringRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public UpdateIntegrationTriggeringCommandHandler(IIntegrationTriggeringRepository integrationTriggeringRepository,
            IEventBus eventBus)
        {
            _integrationTriggeringRepository = integrationTriggeringRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateIntegrationTriggeringCommand request, CancellationToken cancellationToken)
        {
            var existingIntegrationTriggering = await _integrationTriggeringRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingIntegrationTriggering is null)
            {
                throw new NotFoundException($"Could not find IntegrationTriggering '{request.Id}'");
            }

            existingIntegrationTriggering.Update(request.Value);
            _eventBus.Publish(existingIntegrationTriggering.MapToIntegrationTriggeringUpdatedEvent());

        }
    }
}