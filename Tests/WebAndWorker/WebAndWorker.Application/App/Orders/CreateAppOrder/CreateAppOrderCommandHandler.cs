using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace WebAndWorker.Application.App.Orders.CreateAppOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAppOrderCommandHandler : IRequestHandler<CreateAppOrderCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateAppOrderCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(CreateAppOrderCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CreateAppOrderCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}