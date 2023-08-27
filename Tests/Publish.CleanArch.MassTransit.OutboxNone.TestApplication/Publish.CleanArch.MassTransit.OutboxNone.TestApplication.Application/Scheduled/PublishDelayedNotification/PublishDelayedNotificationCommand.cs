using Intent.RoslynWeaver.Attributes;
using MediatR;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Scheduled.PublishDelayedNotification
{
    public class PublishDelayedNotificationCommand : IRequest, ICommand
    {
        public PublishDelayedNotificationCommand(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}