using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Solace.Tests.Application.Common.Eventing;
using Solace.Tests.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Solace.Tests.Application.NotMapped.NotMappedTest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class NotMappedTestCommandHandler : IRequestHandler<NotMappedTestCommand>
    {
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public NotMappedTestCommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(NotMappedTestCommand request, CancellationToken cancellationToken)
        {
            _eventBus.Publish(new NotMappedEvent
            {
            });
            _eventBus.Send(new NotMappedIC
            {
            });
        }
    }
}