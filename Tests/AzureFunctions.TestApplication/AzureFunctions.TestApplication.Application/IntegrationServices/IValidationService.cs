using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.IntegrationServices.Contracts.Services.Validation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace AzureFunctions.TestApplication.Application.IntegrationServices
{
    public interface IValidationService : IDisposable
    {
        Task InboundComValAsync(InboundComValCommand command, CancellationToken cancellationToken = default);
    }
}