using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.Interfaces;
using AzureFunctions.TestApplication.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Implementation
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
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        public void Dispose()
        {
        }
    }
}