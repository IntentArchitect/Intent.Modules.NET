using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.NET8.Application.Interfaces;
using AzureFunctions.NET8.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AzureFunctions.NET8.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class NotificationService : INotificationService
    {
        [IntentManaged(Mode.Merge)]
        public NotificationService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task SendNotification<T>(
            Guid entityId,
            string subject,
            string template,
            T model,
            DomainNotificationType type,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement SendNotification (NotificationService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}