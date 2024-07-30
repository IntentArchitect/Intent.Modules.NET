using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Eventing;
using AdvancedMappingCrud.Repositories.Tests.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.IntegrationEventHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.IntegrationEvents.EventHandlers.EnumMessage
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EnumSampleHandler : IIntegrationEventHandler<EnumSampleEvent>
    {
        [IntentManaged(Mode.Merge)]
        public EnumSampleHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(EnumSampleEvent message, CancellationToken cancellationToken = default)
        {
        }
    }
}