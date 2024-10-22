using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SharedKernel.Consumer.Tests.Application.Common.Models;
using SharedKernel.Consumer.Tests.Domain.Events;
using SharedKernel.Kernel.Tests.Domain.Events;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DomainEventHandler", Version = "1.0")]

namespace SharedKernel.Consumer.Tests.Application.EventHandlers.Countries
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CountryCreatedHandler : INotificationHandler<SharedKernel.Kernel.Tests.Application.Common.Models.DomainEventNotification<CountryCreated>>
    {
        [IntentManaged(Mode.Merge)]
        public CountryCreatedHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(
            SharedKernel.Kernel.Tests.Application.Common.Models.DomainEventNotification<CountryCreated> notification,
            CancellationToken cancellationToken)
        {
            notification.DomainEvent.Country.Code = "Comsumer";
        }
    }
}