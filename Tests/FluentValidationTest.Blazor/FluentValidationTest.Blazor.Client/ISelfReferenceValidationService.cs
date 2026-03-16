using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidationTest.Blazor.Client.Contracts.Services.SelfReferenceValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.ServiceContract", Version = "2.0")]

namespace FluentValidationTest.Blazor.Client
{
    public interface ISelfReferenceValidationService : IDisposable
    {
        Task UploadSelfRefDtoAsync(UploadSelfRefDtoCommand command, CancellationToken cancellationToken = default);
    }
}