using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.Common.Eventing;
using CleanArchitecture.Comprehensive.Domain.Entities.ConventionBasedEventPublishing;
using CleanArchitecture.Comprehensive.Domain.Repositories.ConventionBasedEventPublishing;
using CleanArchitecture.Comprehensive.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.IntegrationTriggeringsAnemic.CreateAnemicIntegrationTriggering
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAnemicIntegrationTriggeringCommandHandler : IRequestHandler<CreateAnemicIntegrationTriggeringCommand, Guid>
    {
        private readonly IIntegrationTriggeringRepository _integrationTriggeringRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public CreateAnemicIntegrationTriggeringCommandHandler(IIntegrationTriggeringRepository integrationTriggeringRepository,
            IEventBus eventBus)
        {
            _integrationTriggeringRepository = integrationTriggeringRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(
            CreateAnemicIntegrationTriggeringCommand request,
            CancellationToken cancellationToken)
        {
            var newIntegrationTriggering = new IntegrationTriggering
            {
                Value = request.Value,
            };

            _integrationTriggeringRepository.Add(newIntegrationTriggering);
            await _integrationTriggeringRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            _eventBus.Publish(newIntegrationTriggering.MapToIntegrationTriggeringCreatedEvent());
            return newIntegrationTriggering.Id;
        }
    }
}