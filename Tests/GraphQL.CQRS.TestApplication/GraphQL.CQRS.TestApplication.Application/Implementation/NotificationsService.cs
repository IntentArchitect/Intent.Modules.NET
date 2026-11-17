using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.CQRS.TestApplication.Application.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class NotificationsService : INotificationsService
    {
        [IntentManaged(Mode.Merge)]
        public NotificationsService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task SendNotification(string recipient, string message, CancellationToken cancellationToken = default)
        {
            // TODO: Implement SendNotification (NotificationsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}