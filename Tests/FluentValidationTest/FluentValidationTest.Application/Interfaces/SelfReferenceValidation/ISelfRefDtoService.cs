using FluentValidationTest.Application.SelfReferenceValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace FluentValidationTest.Application.Interfaces.SelfReferenceValidation
{
    public interface ISelfRefDtoService
    {
        Task Upload(UploadWrapperDto param, CancellationToken cancellationToken = default);
        Task Upload2(SelfRefWithCompDto param, CancellationToken cancellationToken = default);
    }
}