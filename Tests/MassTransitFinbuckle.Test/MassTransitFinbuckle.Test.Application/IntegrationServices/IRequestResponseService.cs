using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransitFinbuckle.Test.Application.IntegrationServices.Contracts.Services.RequestResponse;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.ClientContracts.ServiceContract", Version = "2.0")]

namespace MassTransitFinbuckle.Test.Application.IntegrationServices
{
    public interface IRequestResponseService : IDisposable
    {
        Task TestAsync(TestCommand command, CancellationToken cancellationToken = default);
    }
}