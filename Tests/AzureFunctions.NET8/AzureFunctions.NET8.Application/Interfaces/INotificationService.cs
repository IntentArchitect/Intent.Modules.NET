using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.NET8.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AzureFunctions.NET8.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendNotification<T>(Guid entityId, string subject, string template, T model, DomainNotificationType type, CancellationToken cancellationToken = default);
    }
}