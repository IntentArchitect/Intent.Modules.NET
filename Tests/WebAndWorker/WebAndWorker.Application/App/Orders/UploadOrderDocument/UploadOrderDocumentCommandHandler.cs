using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace WebAndWorker.Application.App.Orders.UploadOrderDocument
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UploadOrderDocumentCommandHandler : IRequestHandler<UploadOrderDocumentCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UploadOrderDocumentCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(UploadOrderDocumentCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (UploadOrderDocumentCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}