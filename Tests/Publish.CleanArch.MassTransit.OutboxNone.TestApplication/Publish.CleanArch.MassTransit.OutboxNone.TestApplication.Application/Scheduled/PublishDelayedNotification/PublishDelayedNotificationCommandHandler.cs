using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.Messages.Shared;
using MediatR;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Scheduled.PublishDelayedNotification
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PublishDelayedNotificationCommandHandler : IRequestHandler<PublishDelayedNotificationCommand>
    {
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public PublishDelayedNotificationCommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(PublishDelayedNotificationCommand request, CancellationToken cancellationToken)
        {
            _eventBus.SchedulePublish(new DelayedNotificationEvent { Message = request.Message }, TimeSpan.FromSeconds(30));
        }
    }
}