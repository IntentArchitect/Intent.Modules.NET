using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace WebAndWorker.Application.Mobile.Orders.CreateMobileOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateMobileOrderCommandHandler : IRequestHandler<CreateMobileOrderCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateMobileOrderCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(CreateMobileOrderCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CreateMobileOrderCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}