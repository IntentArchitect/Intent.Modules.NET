using FluentValidationTest.Application.Interfaces.SelfReferenceValidation;
using FluentValidationTest.Application.SelfReferenceValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace FluentValidationTest.Application.Implementation.SelfReferenceValidation
{
    [IntentManaged(Mode.Merge)]
    public class SelfRefDtoService : ISelfRefDtoService
    {
        [IntentManaged(Mode.Merge)]
        public SelfRefDtoService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Upload(UploadWrapperDto param, CancellationToken cancellationToken = default)
        {
            // TODO: Implement Upload (SelfRefDtoService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Upload2(SelfRefWithCompDto param, CancellationToken cancellationToken = default)
        {
            // TODO: Implement Upload2 (SelfRefDtoService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}