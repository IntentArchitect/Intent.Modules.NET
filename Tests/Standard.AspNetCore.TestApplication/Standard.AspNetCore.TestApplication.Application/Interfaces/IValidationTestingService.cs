using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Application.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Interfaces
{
    public interface IValidationTestingService : IDisposable
    {
        Task InboundValidationDtoAction(InboundValidationDto dto, CancellationToken cancellationToken = default);
    }
}