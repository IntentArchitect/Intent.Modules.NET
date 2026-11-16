using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Application.Interfaces
{
    public interface INotificationsService
    {
        Task SendNotification(string recipient, string message, CancellationToken cancellationToken = default);
    }
}