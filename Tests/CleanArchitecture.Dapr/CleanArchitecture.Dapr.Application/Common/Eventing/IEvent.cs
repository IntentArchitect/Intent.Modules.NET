using CleanArchitecture.Dapr.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.Pubsub.EventInterface", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.Common.Eventing
{
    public interface IEvent : IRequest, ICommand
    {
        string PubsubName { get; }
        string TopicName { get; }
    }
}