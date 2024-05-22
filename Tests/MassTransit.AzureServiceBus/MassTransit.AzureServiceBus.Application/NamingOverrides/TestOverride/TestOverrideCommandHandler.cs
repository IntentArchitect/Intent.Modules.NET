using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Application.Common.Eventing;
using MassTransit.AzureServiceBus.Services;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace MassTransit.AzureServiceBus.Application.NamingOverrides.TestOverride
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestOverrideCommandHandler : IRequestHandler<TestOverrideCommand>
    {
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public TestOverrideCommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(TestOverrideCommand request, CancellationToken cancellationToken)
        {
            _eventBus.Publish(new StandardMessageCustomSubscribeEvent
            {
                Message = request.Message
            });
            _eventBus.Publish(new OverrideMessageStandardSubscribeEvent
            {
                Message = request.Message
            });
            _eventBus.Publish(new OverrideMessageCustomSubscribeEvent
            {
                Message = request.Message
            });
        }
    }
}