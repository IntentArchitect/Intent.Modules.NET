using System;
using System.Threading;
using System.Threading.Tasks;
using AwsLambdaFunction.Sqs.GroupA.Application.Common.Eventing;
using AwsLambdaFunction.Sqs.GroupA.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AwsLambdaFunction.Sqs.GroupA.Application.CreateOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand>
    {
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public CreateOrderCommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            _eventBus.Send(new Eventing.Messages.CreateOrderCommand
            {
            });
        }
    }
}