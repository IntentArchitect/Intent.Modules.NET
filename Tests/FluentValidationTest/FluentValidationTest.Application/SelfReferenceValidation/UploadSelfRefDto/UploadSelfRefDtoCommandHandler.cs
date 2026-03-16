using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.SelfReferenceValidation.UploadSelfRefDto
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UploadSelfRefDtoCommandHandler : IRequestHandler<UploadSelfRefDtoCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UploadSelfRefDtoCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(UploadSelfRefDtoCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (UploadSelfRefDtoCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}