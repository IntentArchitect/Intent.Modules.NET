using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.ClientContracts.ServiceContract", Version = "2.0")]

namespace MassTransit.AzureServiceBus.Application.IntegrationServices
{
    public interface ICQRSService : IDisposable
    {
        Task<CommandResponseDto> CommandDtoReturnAsync(CommandDtoReturn command, CancellationToken cancellationToken = default);
    }
}