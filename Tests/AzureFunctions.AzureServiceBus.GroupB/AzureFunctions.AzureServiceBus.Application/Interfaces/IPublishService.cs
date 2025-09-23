using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Application.Interfaces
{
    public interface IPublishService
    {
        Task PublishEvent(PayloadDto payload, CancellationToken cancellationToken = default);
    }
}